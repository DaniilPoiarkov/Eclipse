using Eclipse.Application.Contracts.Configuration;
using Eclipse.Common.Background;
using Eclipse.Localization;
using Eclipse.WebAPI.Background;
using Eclipse.WebAPI.Configurations;
using Eclipse.WebAPI.Extensions;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.Health;
using Eclipse.WebAPI.Middlewares;
using Eclipse.WebAPI.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

using System.Text;
using System.Text.Json.Serialization;

using Telegram.Bot.Requests.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.Json;

namespace Eclipse.WebAPI;

/// <summary>
/// Takes responsibility for WebAPI
/// </summary>
public static class EclipseWebApiModule
{
    private static readonly TimeSpan _window = TimeSpan.FromSeconds(10);

    private static readonly int _segmentsPerWindow = 2;

    private static readonly int _permitLimit = 10;

    public static IServiceCollection AddWebApiModule(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(EclipseDefaults.AuthenticationScheme, new JwtBearerOptionsConfiguration(configuration).Configure)
            .AddMicrosoftIdentityWebApi(configuration);

        services.AddAuthorization();

        services
            .AddControllers()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.ConfigureTelegramBotMvc();

        services
            .AddEndpointsApiExplorer();

        services
            .AddScoped<TelegramBotApiSecretTokenAuthorizeAttribute>();

        services.AddSwaggerGen();

        services
            .AddExceptionHandler<ExceptionHandlerMiddleware>()
            .AddProblemDetails();

        services.AddApiVersioning()
            .AddMvc()
            .AddApiExplorer();

        services.AddApplicationInsightsTelemetry();

        services
            .ConfigureOptions<ApiExplorerConfiguration>()
            .ConfigureOptions<ApiVersioningConfiguration>()
            .ConfigureOptions<SwaggerUIConfiguration>()
            .ConfigureOptions<SwaggerGenConfiguration>()
            .ConfigureOptions<ApplicationInsightsConfiguration>()
            .ConfigureOptions<AuthorizationConfiguration>();

        services.Scan(tss => tss.FromAssemblyOf<ImportEntitiesBackgroundJobArgs>()
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options
                .AddIpAddressSlidingWindow(_window, _segmentsPerWindow, _permitLimit)
                .AddIpAddressFiveMinutesWindow();
        });

        services.Configure<AzureOAuthOptions>(
            configuration.GetSection("AzureAd")
        );

        services.Configure<CultureList>(
            configuration.GetSection(nameof(CultureList))
        );

        return services;
    }

    public static WebApplication InitializeWebApiModule(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsDevelopment())
        {
            ///
        }

        app.UseExceptionHandler()
            .UseHttpsRedirection();

        app.UseEclipseHealthChecks();

        app.UseRateLimiter();

        app.UseAuthentication()
            .UseAuthorization();

        app.UseLocalization();

        app.MapControllers();

        return app;
    }

    // TODO: Drop this when fix for Telegram.Bot package will be ready. (version 22.5.*)
    //       Issue lies in message enum deserialization.
    //       The code below taken from official fix used in deprecated lib.
    public static IServiceCollection ConfigureTelegramBotMvc(this IServiceCollection services)
            => services.Configure<MvcOptions>(options =>
            {
                options.InputFormatters.Insert(0, _inputFormatter);
                options.OutputFormatters.Insert(0, _outputFormatter);
            });

    private static readonly TelegramBotInputFormatter _inputFormatter = new();

    private static readonly TelegramBotOutputFormatter _outputFormatter = new();

    private class TelegramBotInputFormatter : TextInputFormatter
    {
        public TelegramBotInputFormatter()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedMediaTypes.Add("application/json");
        }

        protected override bool CanReadType(Type type) => type == typeof(Update);

        public sealed override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var model = await JsonSerializer.DeserializeAsync(context.HttpContext.Request.Body, context.ModelType, JsonBotAPI.Options, context.HttpContext.RequestAborted);
            return await InputFormatterResult.SuccessAsync(model);
        }
    }

    private class TelegramBotOutputFormatter : TextOutputFormatter
    {
        public TelegramBotOutputFormatter()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedMediaTypes.Add("application/json");
        }

        protected override bool CanWriteType(Type? type) => typeof(IRequest).IsAssignableFrom(type);

        public sealed override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var stream = context.HttpContext.Response.Body;
            await JsonSerializer.SerializeAsync(stream, context.Object, JsonBotAPI.Options, context.HttpContext.RequestAborted);
        }
    }
}
