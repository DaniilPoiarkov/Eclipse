using Eclipse.Core.Attributes;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Pipelines.MainMenu.TodoItems;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("My To dos", "/todos")]
internal class TodoItemsMenuPipeline : TodoItemsPipelineBase
{
    private readonly ILocalizer _localizer;

    public TodoItemsMenuPipeline(ILocalizer localizer)
    {
        _localizer = localizer;
    }

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(TodoItemMenuButtons, _localizer["Pipelines:TodoItems"]));
    }
}
