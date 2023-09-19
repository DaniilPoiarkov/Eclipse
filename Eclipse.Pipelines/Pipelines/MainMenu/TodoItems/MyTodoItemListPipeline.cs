using Eclipse.Application.Contracts.Telegram.Messages;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Extensions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using System.Text;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("My list", "/todos_my")]
internal class MyTodoItemListPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private readonly IMessageStore _messageStore;

    private static readonly string _errorMessage = "Well, something went wrong. I'll try to figure out what exactly, meanwhile you can use menu to help yourself go further to your dreams";

    public MyTodoItemListPipeline(ITodoItemService todoItemService, IMessageStore messageStore)
    {
        _todoItemService = todoItemService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(SendList);
        RegisterStage(HandleUpdate);
    }

    private IResult SendList(MessageContext context)
    {
        var items = _todoItemService.GetUserItems(context.User.Id)
            .Where(item => !item.IsFinished)
            .ToList();

        if (items.Count == 0)
        {
            FinishPipeline();

            return Menu(TodoItemMenuButtons, $"🫣 It seems to me that you have no plans!{Environment.NewLine}" +
                $"Anyway, what about to add some?😏");
        }

        var message = BuildListMessage(items);
        var buttons = BuildButtons(items);

        return Menu(buttons, message);
    }

    private IResult HandleUpdate(MessageContext context)
    {
        if (context.Value.Equals("go_back"))
        {
            return Menu(TodoItemMenuButtons, "Whatever you want");
        }

        var id = context.Value.ToGuid();

        if (id == Guid.Empty)
        {
            return Menu(TodoItemMenuButtons, _errorMessage);
        }

        try
        {
            _todoItemService.FinishItem(id);

            var items = _todoItemService.GetUserItems(context.ChatId)
                .Where(item => !item.IsFinished)
                .ToList();

            var message = _messageStore.GetMessage(new MessageKey(context.ChatId));

            if (items.Count == 0)
            {
                return CongratulationsMessage(message);
            }

            RegisterStage(HandleUpdate);

            if (message is null)
            {
                return SendList(context);
            }

            return UpdatedMenu(items, message);
        }
        catch
        {
            return Menu(TodoItemMenuButtons, _errorMessage);
        }
    }

    private static IResult UpdatedMenu(List<TodoItemDto> items, Message message)
    {
        var buttons = BuildButtons(items);
        var text = $"Horray! You are doing great!{Environment.NewLine}{Environment.NewLine}{BuildListMessage(items)}";
        var menu = new InlineKeyboardMarkup(buttons);

        return Edit(message.MessageId, text, menu);
    }

    private static IResult CongratulationsMessage(Message? message)
    {
        var text = $"You did em all!{Environment.NewLine}" +
            $"My congratulations 🥳";

        if (message is null)
        {
            return Text(text);
        }

        var menu = new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>>()
        {
            new[] { InlineKeyboardButton.WithCallbackData("Horray!", "Main Menu") }
        });

        return Edit(message.MessageId, text, menu);
    }

    private static string BuildListMessage(IList<TodoItemDto> items)
    {
        var sb = new StringBuilder("📝 Your to dos:")
            .AppendLine()
            .AppendLine();

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var index = i + 1;

            var info = item.IsFinished
                ? $"✅ {index}. {item.Text}"
                : $"⬜️ {index}. {item.Text}";

            sb.AppendLine(info);
            sb.AppendLine($"Created at: {item.CreatedAt.ToString("dd.MM, HH:mm")}");

            if (item.IsFinished && item.FinishedAt.HasValue)
            {
                sb.AppendLine($"Finished at: {item.FinishedAt.Value.ToString("dd.MM, HH:mm")}");
            }

            sb.AppendLine();
        }

        sb.AppendLine()
            .AppendLine("Select a number to mark item as finished! Or press \'Go back\' button to return 😊");

        return sb.ToString();
    }

    private static IEnumerable<IEnumerable<InlineKeyboardButton>> BuildButtons(IEnumerable<TodoItemDto> items)
    {
        var buttons = items
            .Select((item, index) => InlineKeyboardButton.WithCallbackData($"{++index}", $"{item.Id}"))
            .Select(button => new InlineKeyboardButton[] { button })
            .ToList();

        buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Go back", "go_back") });

        return buttons;
    }
}
