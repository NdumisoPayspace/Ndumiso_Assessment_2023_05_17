namespace Ndumiso_Assessment_2023_05_17.WebAPIControllers
{
    using DevExtreme.AspNet.Data;
    using DevExtreme.AspNet.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Ndumiso_Assessment_2023_05_17.Interfaces;

    public class ImageWebAPIController : Controller
    {
        private readonly IImageService imageService;

        public ImageWebAPIController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [Authorize]
        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            return DataSourceLoader.Load(this.imageService.GetAll(), loadOptions);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult Delete(int key)
        {
            this.imageService.Delete(key);

            return this.Ok();
        }
    }
}
