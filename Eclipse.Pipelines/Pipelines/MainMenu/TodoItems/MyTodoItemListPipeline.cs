using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using System.Text;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("My list", "/todos_my")]
internal class MyTodoItemListPipeline : EclipsePipelineBase
{
    private readonly ITodoItemService _todoItemService;

    public MyTodoItemListPipeline(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendList);
    }

    private IResult SendList(MessageContext context)
    {
        var items = _todoItemService.GetUserItems(context.User.Id);

        var sb = new StringBuilder("📝 Your to dos:")
            .AppendLine()
            .AppendLine();

        foreach (var item in items)
        {
            var info = item.IsFinished
                ? $"✅ {item.Text}"
                : $"⬜️ {item.Text}";

            sb.AppendLine(info);
            sb.AppendLine($"Created at: {item.CreatedAt.ToString("dd.MM, HH:mm")}");

            if (item.IsFinished && item.FinishedAt.HasValue)
            {
                sb.AppendLine($"Finished at: {item.FinishedAt.Value.ToString("dd.MM, HH:mm")}");
            }

            sb.AppendLine();
        }

        return Text(sb.ToString());
    }
}
