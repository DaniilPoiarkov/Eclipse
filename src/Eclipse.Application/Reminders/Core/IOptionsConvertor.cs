namespace Eclipse.Application.Reminders.Core;

internal interface IOptionsConvertor<TFrom, TTo>
{
    TTo Convert(TFrom from);
}
