namespace Ndumiso_Assessment_2023_05_17.Controllers
{    
    using System.Net.Http;
    using System.Text;

    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using Ndumiso_Assessment_2023_05_17.Interfaces;
    using Ndumiso_Assessment_2023_05_17.Models;
    using Ndumiso_Assessment_2023_05_17.Validators;
    using Newtonsoft.Json;

    public class ChatController : Controller
    {
        private readonly IChatService chatService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IValidator<Chat> questionValidator;
        private readonly IMemoryCache memoryCache;

        public ChatController(IHttpClientFactory httpClientFactory, IValidator<Chat> questionValidator, IMemoryCache memoryCache, IChatService chatService)
        {
            this.httpClientFactory = httpClientFactory;
            this.questionValidator = questionValidator;
            this.memoryCache = memoryCache;
            this.chatService = chatService;
        }

        [Authorize]
        public async Task<IActionResult> Index(Chat model)
        {
            ValidationResult result = questionValidator.Validate(model);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);

                return View();
            }

            if(model.UserQuestion != null && model.Answer != null)
            {
                ViewBag.Question = model.UserQuestion;
                ViewBag.Response = model.Answer;

                return View();
            }

            var chatCache = memoryCache.Get<Chat>(model.UserQuestion);

            if (chatCache != null && chatCache.UserQuestion == model.UserQuestion)
            {
                ViewBag.Question = chatCache.UserQuestion;
                ViewBag.Response = chatCache.Answer;

                return View();
            }

            var question = model.UserQuestion;
            var response = await ChatResponse(question);

            if (response != null)
            {
                var chat = new Chat();
                
                chat.CreatedAt = DateTime.Now;
                chat.UserQuestion = question;
                chat.Answer = response;

                memoryCache.Set(chat.UserQuestion, chat, TimeSpan.FromDays(1));
                
                chatService.Add(chat);
            }

            ViewBag.Question = question;
            ViewBag.Response = response;

            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int Id)
        {
            var chat = chatService.GetById(Id);

            if (chat == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", chat);
        }

        public async Task<string> ChatResponse(string question)
        {
            string errorString;

            try
            {
                var httpClient = httpClientFactory.CreateClient("OpenAI");

                var requestBodyObject = new
                {
                    model = "text-davinci-003",
                    prompt = question,
                    max_tokens = 50
                };

                var requestBody = JsonConvert.SerializeObject(requestBodyObject);

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("completions", content);

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
