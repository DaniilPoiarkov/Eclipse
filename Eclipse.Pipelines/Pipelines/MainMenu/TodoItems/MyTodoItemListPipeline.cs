using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Extensions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using System.Text;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("My list", "/todos_my")]
internal class MyTodoItemListPipeline : TodoItemsPipelineBase
{
    private readonly ITodoItemService _todoItemService;

    private static readonly string _errorMessage = "Well, something went wrong. I'll try to figure out what exectly, try again a bit latere please";

    public MyTodoItemListPipeline(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
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

        var buttons = items
            .Select((item, index) => InlineKeyboardButton.WithCallbackData($"{++index}", $"{item.Id}"))
            .Select(button => new InlineKeyboardButton[] { button })
            .ToList();

        buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Go back", "go_back") });

        return Menu(buttons, sb.ToString());
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
            return Menu(TodoItemMenuButtons, "Horray! You are doing great!");
        }
        catch
        {
            return Menu(TodoItemMenuButtons, _errorMessage);
        }
    }
}
