namespace Ndumiso_Assessment_2023_05_17.Controllers
{    
    using System.Text;

    using Microsoft.AspNetCore.Mvc;

    using Ndumiso_Assessment_2023_05_17.Models;
    using Newtonsoft.Json;


    public class OpenAIController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public OpenAIController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public IActionResult Index(Question model)
        {
            return View();
        }

        public async Task<IActionResult> Chat(Question model)
        {
            //Validate question using question model
            if (ModelState.IsValid)
            {
                string question = model.UserQuestion.ToString();
                var response = await ChatResponse(question);

                ViewBag.Question = question;
                ViewBag.Response = response;
            }

            return View();
        }

        public async Task<IActionResult> Editing(Text model)
        {
            //Validate question using question model

            if (ModelState.IsValid)
            {
                string text = model.UserText.ToString();

                var instructionText = "";

                switch (model.Task)
                {
                    case "Spelling":
                        instructionText = "Fix the spelling mistakes";
                        break;
                    case "Paraphrase":
                        instructionText = "paraphrase";//return the summary only might need to swap this out for something else
                        break;
                    case "Translate":
                        instructionText = "Translate to English";
                        break;
                }

                var response = await EditText(text, instructionText);

                ViewBag.Text = text;
                ViewBag.Response = response;
            }
            return View();
        }

        public async Task<IActionResult> Image(Request model)
        {
            //Validate model here
            if(ModelState.IsValid)
            {
                string request = model.Description.ToString();
                var response = await GenerateImage(request);

                ViewBag.Description = request;
                ViewBag.Response = response;
            }
            return View();
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
