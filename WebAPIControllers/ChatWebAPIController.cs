namespace Ndumiso_Assessment_2023_05_17.WebAPIControllers
{   
    using System.Text;

    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;


    [Route("api/[controller]")]
    public class ChatWebAPIController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ChatWebAPIController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("{question}")]
        public async Task<string> Get(string question)
        {
            string errorString;

            try
            {
                var httpClient = httpClientFactory.CreateClient("RapidAPI");

                var requestBodyObject = new 
                { 
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "user", content = question }
                    }
                };

                var requestBody = JsonConvert.SerializeObject(requestBodyObject);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("chat/completions", content);

                var jsonResponse = await response.Content.ReadAsStringAsync();

                return jsonResponse;
            }
            catch (Exception e) 
            { 
                errorString = $"There was an error retrieving the response: {e.Message}";
            }

            return errorString;
        }
    }
}
