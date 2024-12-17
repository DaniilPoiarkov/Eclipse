namespace Eclipse.IntegrationEvents.Users;

public sealed record NewUserRegisteredIntegrationEvent(Guid UserId, string UserName, string FirstName, string LastName, long ChatId);
