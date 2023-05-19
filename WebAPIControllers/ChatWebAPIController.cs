namespace Ndumiso_Assessment_2023_05_17.WebAPIControllers
{
    using DevExtreme.AspNet.Data;
    using DevExtreme.AspNet.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Ndumiso_Assessment_2023_05_17.Interfaces;

    public class ChatWebAPIController : Controller
    {
        private readonly IChatService chatService;

        public ChatWebAPIController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [Authorize]
        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            return DataSourceLoader.Load(this.chatService.GetAll(), loadOptions);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult Delete(int key)
        {
            this.chatService.Delete(key);

            return this.Ok();
        }
    }
}
