namespace Ndumiso_Assessment_2023_05_17.Services
{
    using Ndumiso_Assessment_2023_05_17.Data;
    using Ndumiso_Assessment_2023_05_17.Interfaces;
    using Ndumiso_Assessment_2023_05_17.Models;

    public class TextService : GenericService<Text>, ITextService
    {
        public TextService(ApplicationDbContext context) : base(context)
        {
        }
    }
}
