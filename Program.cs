using System.Net.Http.Headers;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Ndumiso_Assessment_2023_05_17.Data;
using Ndumiso_Assessment_2023_05_17.Interfaces;
using Ndumiso_Assessment_2023_05_17.Models;
using Ndumiso_Assessment_2023_05_17.Services;
using Ndumiso_Assessment_2023_05_17.Validators;

using Microsoft.AspNetCore.Identity;
using Ndumiso_Assessment_2023_05_17.Areas.Identity.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = configBuilder.Build();

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("AuthDbContextConnection")));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>();

        builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<ITextService, TextService>();
        builder.Services.AddScoped<IImageService, ImageService>();

        builder.Services.AddScoped<IValidator<Chat>, ChatValidator>();
        builder.Services.AddScoped<IValidator<Text>, TextValidator>();
        builder.Services.AddScoped<IValidator<Image>, ImageValidator>();

        builder.Services.AddMemoryCache();

        builder.Services.AddHttpClient("OpenAI", client => 
        {
            //store this securely
            var apiKey = "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";

            client.BaseAddress = new Uri("https://api.openai.com/v1/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        });

 

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        app.Run();
    }
}
