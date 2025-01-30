using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System.Reflection;

namespace Eclipse.Infrastructure.Caching;

internal sealed class PrivateMembersContractResolver : DefaultContractResolver
{
    private static readonly Lazy<PrivateMembersContractResolver> _instance = new(() => new());
    internal static PrivateMembersContractResolver Instance => _instance.Value;

    private PrivateMembersContractResolver() { }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (property.Writable)
        {
            return property;
        }

        var info = member as PropertyInfo;
        property.Writable = info?.GetSetMethod(true) is not null;

        return property;
    }
}
