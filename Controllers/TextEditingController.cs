namespace Ndumiso_Assessment_2023_05_17.Controllers
{
    using System;
    using System.Net.Http;
    using System.Text;

    using FluentValidation;
    using FluentValidation.Results;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using Ndumiso_Assessment_2023_05_17.Interfaces;
    using Ndumiso_Assessment_2023_05_17.Models;
    using Ndumiso_Assessment_2023_05_17.Services;
    using Ndumiso_Assessment_2023_05_17.Validators;
    using Newtonsoft.Json;

    public class TextEditingController : Controller
    {
        private readonly ITextService textService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IValidator<Text> textValidator;
        private readonly IMemoryCache memoryCache;

        public TextEditingController(IHttpClientFactory httpClientFactory, IValidator<Text> textValidator, IMemoryCache memoryCache, ITextService textService)
        {
            this.httpClientFactory = httpClientFactory;
            this.textValidator = textValidator;
            this.memoryCache = memoryCache;
            this.textService = textService;
        }

        public async Task<IActionResult> Index(Text model)
        {
            ValidationResult result = textValidator.Validate(model);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);

                return View();
            }

            if (model.UserText != null && model.Answer != null)
            {
                ViewBag.Text = $"{model.Task}:  {model.UserText}";
                ViewBag.Response = model.Answer;

                return View();
            }

            var textCache = memoryCache.Get<Text>(model.UserText);

            if (textCache != null && textCache.UserText == model.UserText && textCache.Task == model.Task)
            {
                ViewBag.Text = $"{textCache.Task}:  {textCache.UserText}";
                ViewBag.Response = textCache.Answer;

                return View();
            }

            var text = model.UserText;

            var instructionText = "";

            switch (model.Task)
            {
                case "Check Spelling":
                    instructionText = "Fix the spelling mistakes";
                    break;
                case "Paraphrase":
                    instructionText = "paraphrase";
                    break;
                case "Translate to English":
                    instructionText = "Translate to English";
                    break;
            }

            var response = await EditText(text, instructionText);

            if(response != null) 
            {
                var textObject = new Text();

                textObject.CreatedAt = DateTime.Now;
                textObject.UserText = text;
                textObject.Task = model.Task;
                textObject.Answer = response;

                memoryCache.Set(textObject.UserText, textObject, TimeSpan.FromDays(1));

                textService.Add(textObject);
            }

            ViewBag.Text = $"{model.Task}:  {text}";
            ViewBag.Response = response;

            return View();
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var text = textService.GetById(Id);

            if (text == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", text);
        }

        public async Task<string> EditText(string text, string command)
        {
            string errorString;

            try
            {
                var httpClient = httpClientFactory.CreateClient("OpenAI");

                var requestBodyObject = new
                {
                    model = "text-davinci-edit-001",
                    input = text,
                    instruction = command
                };

                var requestBody = JsonConvert.SerializeObject(requestBodyObject);

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("edits", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    var responseText = responseObject.choices[0].text.ToString();

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
