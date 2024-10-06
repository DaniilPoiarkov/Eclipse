using Eclipse.Core.Attributes;
using Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu:MyToDos", "/todos")]
internal sealed class TodoItemsPipeline : TodoItemsPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(TodoItemMenuButtons, Localizer["Pipelines:TodoItems"]));
    }
}
