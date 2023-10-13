using Eclipse.Core.Core;

using FluentAssertions;

using System.Reflection;

namespace Eclipse.Pipelines.Tests.Fixture;

public class ResultAssertion<TResult>
    where TResult : IResult
{
    private readonly TResult _result;

    private readonly Type _type;

    public ResultAssertion(TResult result)
    {
        _result = result;
        _type = result.GetType();
    }

    public ResultAssertion<TResult> FieldHasValue<T>(string member, T value)
    {
        var field = _type.GetField(member, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"Field with name {member} not exist");

        field.GetValue(_result).Should().Be(value);

        return this;
    }

    public ResultAssertion<TResult> PropertyHasValue<T>(string member, T value)
    {
        var property = _type.GetProperty(member, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"Property with name {member} not exist");

        property.GetValue(_result).Should().Be(value);

        return this;
    }
}
