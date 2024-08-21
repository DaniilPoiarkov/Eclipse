using Eclipse.Application.Contracts.Users;

using System.Collections;

namespace Eclipse.Application.Tests.Users.TestData;

public sealed class UserPartialUpdateInvalidTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [
            new UserPartialUpdateDto { NameChanged = true },
            "Name"
        ];

        yield return [
            new UserPartialUpdateDto { NameChanged = true, Name = "NewName", UserNameChanged = true },
            "UserName"
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
