using Eclipse.Domain.Reminders;

namespace Eclipse.Application.Tests.Exporting.Reminders;

internal sealed class ReminderRowComparer : IExportRowComparer<Reminder, ExportedReminder>
{
    public bool Compare(Reminder entity, ExportedReminder row)
    {
        var notifyAt = new TimeOnly(entity.NotifyAt.Hour, entity.NotifyAt.Minute);

        return row.Id == entity.Id
            && row.UserId == entity.UserId
            && row.Text == entity.Text
            && !row.NotifyAt.IsNullOrEmpty()
            && TimeOnly.Parse(row.NotifyAt) == notifyAt;
    }
}
