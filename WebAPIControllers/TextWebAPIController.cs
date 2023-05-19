namespace Ndumiso_Assessment_2023_05_17.WebAPIControllers
{
    using DevExtreme.AspNet.Data;
    using DevExtreme.AspNet.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using Ndumiso_Assessment_2023_05_17.Interfaces;

    public class TextWebAPIController : Controller
    {
        private readonly ITextService textService;

        public TextWebAPIController(ITextService textService)
        {
            this.textService = textService;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            return DataSourceLoader.Load(this.textService.GetAll(), loadOptions);
        }

        [HttpDelete]
        public IActionResult Delete(int key)
        {
            this.textService.Delete(key);

            return this.Ok();
        }
    }
}
