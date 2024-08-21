using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserUpdateDto : IValidatableObject
{
    [Required]
    public string? Name { get; set; }

    public string Surname { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    public string? Culture { get; set; }

    public bool NotificationsEnabled { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var localizer = validationContext.GetRequiredService<IStringLocalizer<UserUpdateDto>>();

        if (Name.IsNullOrEmpty())
        {
            yield return new ValidationResult(localizer["{0}IsRequired", localizer[nameof(Name)]]);
        }

        if (UserName.IsNullOrEmpty())
        {
            yield return new ValidationResult(localizer["{0}IsRequired", localizer[nameof(UserName)]]);
        }
    }
}
