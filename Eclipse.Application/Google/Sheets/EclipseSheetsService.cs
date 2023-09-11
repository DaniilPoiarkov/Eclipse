using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Google.Sheets.Parsers;
using Eclipse.Core.Models;
using Eclipse.Domain.Shared.Suggestions;
using Eclipse.Infrastructure.Google.Sheets;

namespace Eclipse.Application.Google.Sheets;

internal class EclipseSheetsService : IEclipseSheetsService
{
    private readonly string _sheetId;

    private readonly string _suggestionsRange;

    private readonly string _usersRange;

    private readonly IGoogleSheetsService _sheetsService;

    public EclipseSheetsService(IGoogleSheetsService sheetsService, string sheetId, string suggestionsRange, string usersRange)
    {
        _sheetsService = sheetsService;
        _sheetId = sheetId;
        _suggestionsRange = suggestionsRange;
        _usersRange = usersRange;
    }

    public IReadOnlyList<SuggestionDto> GetSuggestions()
    {
        return GetObjects(_suggestionsRange, new SuggestionObjectParser());
    }

    public void AddSuggestion(SuggestionDto suggestion)
    {
        AddObject(_suggestionsRange, suggestion, new SuggestionObjectParser());
    }

    public IReadOnlyList<TelegramUser> GetUsers()
    {
        return GetObjects(_usersRange, new TelegramUserObjectParser());
    }

    public void AddUser(TelegramUser user)
    {
        AddObject(_usersRange, user, new TelegramUserObjectParser());
    }

    private void AddObject<TObject>(string range, TObject obj, IObjectParser<TObject> parser)
    {
        _sheetsService.Append(_sheetId, range, obj, parser);
    }

    private IReadOnlyList<TObject> GetObjects<TObject>(string range, IObjectParser<TObject> parser)
    {
        return _sheetsService.Get(_sheetId, range, parser).ToList();
    }
}
