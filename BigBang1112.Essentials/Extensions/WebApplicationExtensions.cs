using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace BigBang1112.Extensions;

public static class WebApplicationExtensions
{
    public static void UseEssentials(this WebApplication app, EssentialsOptions options)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapRazorPages();
        app.MapControllers();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentname}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{options.Title} API v1");
            c.InjectStylesheet("/css/SwaggerDark.css");
        });

        app.UseReDoc(c =>
        {

        });
    }
}
