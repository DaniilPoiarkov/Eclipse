namespace Eclipse.Infrastructure.Google.Sheets;

public interface IObjectParser<TObject>
{
    TObject Parse(IList<object> values);

    IList<object> Parse(TObject value);
}
