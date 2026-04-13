using Eclipse.Core.Updates;
using Eclipse.Core.Validation;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Eclipse.Pipelines.Validation;

internal sealed class FeatureFlag : ContextValidationAttribute
{
    private readonly string _feature;

    public FeatureFlag(string feature)
    {
        _feature = feature;
    }

    public override ValidationResult Validate(ValidationContext context)
    {
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var options = configuration.GetSection($"Features:{_feature}")
            .Get<FeatureOptions>();

        if (options is null)
        {
            return ValidationResult.Failure($"Feature '{_feature}' is not configured.");
        }

        if (options.IsActive)
        {
            return ValidationResult.Success();
        }

        var user = context.Update.ExtractSender();

        if (options.UserIds.Contains(user.Id))
        {
            return ValidationResult.Success();
        }

        var localizer = context.ServiceProvider.GetRequiredService<IStringLocalizer<FeatureFlag>>();
        var currentCulture = context.ServiceProvider.GetRequiredService<ICurrentCulture>();

        var prefferedCulture = user.LanguageCode
            ?? configuration.GetValue<string>("Localization:DefaultCulture")
            ?? "en";

        using var _ = currentCulture.UsingCulture(prefferedCulture);

        return ValidationResult.Failure(localizer[options.ErrorMessage]);
    }
}
