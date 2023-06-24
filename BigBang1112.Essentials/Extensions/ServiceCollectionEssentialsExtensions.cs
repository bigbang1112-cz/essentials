using BigBang1112.Attributes;
using BigBang1112.Data;
using BigBang1112.Services;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BigBang1112.Extensions;

public static class ServiceCollectionEssentialsExtensions
{
    public static void AddEssentials(this IServiceCollection services, EssentialsOptions options)
    {
        var assemblyName = options.Assembly.GetName();

        options.Mapper.Configure();

        services.AddControllers().AddApplicationPart(options.Assembly);
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddRouting(options => options.LowercaseUrls = true);

        services.Configure<ForwardedHeadersOptions>(options => // god bless https://laimis.medium.com/couple-issues-with-https-redirect-asp-net-core-7021cf383e00
        {
            options.ForwardedHeaders = 
                ForwardedHeaders.XForwardedFor | 
                ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddMultiAuth(options.Config);

        services.AddEFSecondLevelCache(options =>
        {
            options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30))
                .DisableLogging(true).UseCacheKeyPrefix("EF_");
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        services.AddScoped<IAccountsUnitOfWork, AccountsUnitOfWork>();
        services.AddScoped<IAccountMergeService, AccountMergeService>();
        services.AddScoped<AccountService>();
        services.AddSingleton<IFileHostService, FileHostService>();

        services.AddScoped<HttpClient>();

        services.AddDbContext2<AccountsContext>(options.Config, "AccountsDb");

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{options.Title} API", Version = "v1" });

            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{assemblyName.Name}.xml"));

            c.CustomSchemaIds(type => type.GetCustomAttribute<SwaggerModelNameAttribute>()?.Name ?? type.Name);
        });
    }

    public static IServiceCollection AddDbContext2<TContext>(this IServiceCollection services, IConfiguration config, string dbName) where TContext : DbContext
    {
        return services.AddDbContext<TContext>(options =>
        {
            var connectionString = config.GetConnectionString(dbName);

            switch (config["DatabaseType"])
            {
                case "MySQL":
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    break;
                case "MSSQL":
                    options.UseSqlServer(connectionString);
                    break;
                default:
                    throw new Exception("Unknown database type.");
            }

            //options.EnableSensitiveDataLogging();
        });
    }
}
