using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.TodoItems;

using Microsoft.AspNetCore.Mvc;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MigrationController : ControllerBase
{
    private readonly ITodoItemRepository _repository;

    private readonly IIdentityUserRepository _userRepository;

    public MigrationController(ITodoItemRepository repository, IIdentityUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Migrate()
    {
        var users = await _userRepository.GetAllAsync();
        var items = await _repository.GetAllAsync();

        foreach (var user in users)
        {
            var userItems = items.Where(i => i.TelegramUserId == user.ChatId);

            user.MigrateTodoItems(userItems);

            await _userRepository.UpdateAsync(user);
        }

        return Ok(users);
    }
}
