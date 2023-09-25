using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Extensions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.View;

[Route("Menu:AdminMenu:View:Users", "/admin_view_users")]
internal class ViewUsersPipeline : AdminPipelineBase
{
    private readonly ITelegramUserRepository _userRepository;

    public ViewUsersPipeline(ITelegramUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    protected override void Initialize()
    {
        RegisterStage(GetUserInfo);
    }

    private IResult GetUserInfo(MessageContext context)
    {
        var usersInfo = _userRepository.GetAll()
            .Select((user, index) => $"{++index} | {user.Id} | {user.Name} {user.Username.FormattedOrEmpty(s => $"| @{s}")}");

        return Text(string.Join(Environment.NewLine, usersInfo));
    }
}
