namespace Ndumiso_Assessment_2023_05_17.Controllers
{
    using System;
    using System.Net.Http;
    using System.Text;

    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using Ndumiso_Assessment_2023_05_17.Interfaces;
    using Ndumiso_Assessment_2023_05_17.Models;
    using Ndumiso_Assessment_2023_05_17.Services;
    using Ndumiso_Assessment_2023_05_17.Validators;
    using Newtonsoft.Json;


    public class ImageController : Controller
    {
        private readonly IImageService imageService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IValidator<Image> imageValidator;
        private readonly IMemoryCache memoryCache;

        public ImageController(IHttpClientFactory httpClientFactory, IValidator<Image> imageValidator, IMemoryCache memoryCache, IImageService imageService)
        {
            this.httpClientFactory = httpClientFactory;
            this.imageValidator = imageValidator;
            this.memoryCache = memoryCache;
            this.imageService = imageService;
        }

        [Authorize]
        public async Task<IActionResult> Index(Image model)
        {
            ValidationResult result = imageValidator.Validate(model);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return View();
            }

            if (model.Description != null && model.Url != null)
            {
                ViewBag.Description = model.Description;
                ViewBag.Response = model.Url;

                return View();
            }

            var imageCache = memoryCache.Get<Image>(model.Description);

            if (imageCache != null && imageCache.Description == model.Description)
            {
                ViewBag.Description = imageCache.Description;
                ViewBag.Response = imageCache.Url;

                return View();
            }

            var request = model.Description;
            var response = await GenerateImage(request);

            if (response != null)
            {
                var image = new Image();
                
                image.CreatedAt = DateTime.Now;
                image.Description = request;
                image.Url = response;

                memoryCache.Set(request, image, TimeSpan.FromDays(1));

                imageService.Add(image);
            }

            ViewBag.Description = request;
            ViewBag.Response = response;

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int Id)
        {
            var image = imageService.GetById(Id);

            if (image == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", image);
        }

        public async Task<string> GenerateImage(string request)
        {
            string errorString;

            try
            {
                var httpClient = httpClientFactory.CreateClient("OpenAI");

                var requestBodyObject = new
                {
                    prompt = request,
                    size = "512x512"
                };

                var requestBody = JsonConvert.SerializeObject(requestBodyObject);

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("images/generations", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    var responseText = responseObject.data[0].url.ToString();

                    return responseText;
                }
                else
                {
                    return $"There was an error retrieving the response: {response.StatusCode.ToString()}";
                }

            }
            catch (Exception e)
            {
                errorString = $"There was an error retrieving the response: {e.Message}";

            }

            return errorString;
        }
    }
}
