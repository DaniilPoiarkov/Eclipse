using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.View;

[Route("Menu:AdminMenu:View:Users", "/admin_view_users")]
internal class ViewUsersPipeline : AdminPipelineBase
{
    private readonly IIdentityUserService _userService;

    public ViewUsersPipeline(IIdentityUserService userService)
    {
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(GetUserInfo);
    }

    private async Task<IResult> GetUserInfo(MessageContext context, CancellationToken cancellationToken = default)
    {
        var usersInfo = (await _userService.GetAllAsync(cancellationToken))
            .Select((user, index) => $"{++index} | {user.ChatId} | {user.Name} {user.Username.FormattedOrEmpty(s => $"| @{s}")}");

        return Text(string.Join(Environment.NewLine, usersInfo));
    }
}
