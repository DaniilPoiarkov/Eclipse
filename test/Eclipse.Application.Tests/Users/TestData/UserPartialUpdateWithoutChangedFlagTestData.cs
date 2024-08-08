using Eclipse.Application.Contracts.Users;

using System.Collections;

namespace Eclipse.Application.Tests.Users.TestData;

internal sealed class UserPartialUpdateWithoutChangedFlagTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [
            new UserPartialUpdateDto
            {
                Name = "Test",
                Surname = "Test",
            }
        ];

        yield return [
            new UserPartialUpdateDto()
        ];

        yield return [
            new UserPartialUpdateDto
            {
                UserName = "Test"
            }
        ];

        yield return [
            new UserPartialUpdateDto
            {
                Culture = "uk"
            }
        ];

        yield return [
            new UserPartialUpdateDto
            {
                NotificationsEnabled = true,
            }
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
