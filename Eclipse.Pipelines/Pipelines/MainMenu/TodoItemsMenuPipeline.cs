using Eclipse.Core.Attributes;
using Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("My To dos", "/todos")]
internal class TodoItemsMenuPipeline : TodoItemsPipelineBase
{
    private static readonly string _message = "Let's take a look what do we have here..";

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(TodoItemMenuButtons, _message));
    }
}
