namespace Ndumiso_Assessment_2023_05_17.Services
{
    using Ndumiso_Assessment_2023_05_17.Data;
    using Ndumiso_Assessment_2023_05_17.Interfaces;
    using Ndumiso_Assessment_2023_05_17.Models;

    public class ImageService : GenericService<Image>, IImageService
    {
        public ImageService(ApplicationDbContext context) : base(context)
        {
        }
    }
}
