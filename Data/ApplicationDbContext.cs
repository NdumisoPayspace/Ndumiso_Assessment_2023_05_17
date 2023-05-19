namespace Ndumiso_Assessment_2023_05_17.Data
{
    using Microsoft.EntityFrameworkCore;
    using Ndumiso_Assessment_2023_05_17.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chat> Chat { get; set; }
        public DbSet<Text> Text { get; set; }
        public DbSet<Image> Image { get; set; }
    }
}
