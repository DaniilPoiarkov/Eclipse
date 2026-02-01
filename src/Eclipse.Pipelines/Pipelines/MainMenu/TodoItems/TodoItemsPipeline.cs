using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

[Route("Menu:MainMenu:MyToDos", "/todos")]
internal sealed class TodoItemsPipeline : TodoItemsPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(TodoItemMenuButtons, Localizer["Pipelines:TodoItems"]));
    }
}
