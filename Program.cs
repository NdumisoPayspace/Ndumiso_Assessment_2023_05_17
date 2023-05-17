using System.Net.Http;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

        builder.Services.AddHttpClient("RapidAPI", client => 
        {
            //store this securely
            var apiKey = "2eb9f8c55dmsh44510842ea0a32fp162bb4jsn8efacccb48af";

            //move this to program.cs
            client.BaseAddress = new Uri("https://openai80.p.rapidapi.com/");
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "openai80.p.rapidapi.com");
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

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}