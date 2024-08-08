using Eclipse.Application.Contracts.Users;

using System.Collections;

namespace Eclipse.Application.Tests.Users.TestData;

internal sealed class UserUpdateValidTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [
            new UserUpdateDto
            {
                Name = "new_name",
                Surname = "new_surname",
                UserName = "new_username",
                Culture = "en",
                NotificationsEnabled = true
            }
        ];

        yield return [
            new UserUpdateDto
            {
                Name = "John",
                Surname = "Doe",
                UserName = "JohnDoe",
                Culture = "en",
                NotificationsEnabled = true
            }
        ];
        
        yield return [
            new UserUpdateDto
            {
                Name = "Billy",
                Surname = "Jean",
                UserName = "IsNot_MyLover",
                Culture = "uk",
                NotificationsEnabled = false
            }
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
