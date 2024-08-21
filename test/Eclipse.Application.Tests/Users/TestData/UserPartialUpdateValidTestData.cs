using Eclipse.Application.Contracts.Users;

using System.Collections;

namespace Eclipse.Application.Tests.Users.TestData;

public sealed class UserPartialUpdateValidTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [
            new UserPartialUpdateDto
            {
                NameChanged = true,
                Name = "NewName",
            }
        ];

        yield return [
            new UserPartialUpdateDto
            {
                SurnameChanged = true,
                Surname = "NewSurname"
            }
        ];

        yield return [
            new UserPartialUpdateDto
            {
                UserNameChanged = true,
                UserName = "UserName",

                CultureChanged = true,
                Culture = "en"
            }
        ];

        yield return [
            new UserPartialUpdateDto
            {
                Name = "Name",
                Surname = "Surname"
            }
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
