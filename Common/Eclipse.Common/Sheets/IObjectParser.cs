using Eclipse.Common.Results;

namespace Eclipse.Common.Sheets;

/// <summary>
/// Contains logic to parse <a cref="TObject"></a> to proper list of objects and parse <a cref="IList{object}"></a> to specified type
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IObjectParser<TObject>
{
    Result<TObject> Parse(IList<object> values);

    IList<object> Parse(TObject value);
}
