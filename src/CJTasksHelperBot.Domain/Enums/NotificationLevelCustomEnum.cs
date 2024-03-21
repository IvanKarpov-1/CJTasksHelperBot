namespace CJTasksHelperBot.Domain.Enums;

public record NotificationLevelCustomEnum(int Id, string DisplayName) : Enumeration<NotificationLevelCustomEnum>(Id, DisplayName)
{
    public static readonly NotificationLevelCustomEnum Week = new((int)NotificationLevel.Week, "NotLevelWeek");
    public static readonly NotificationLevelCustomEnum TwoDays = new((int)NotificationLevel.TwoDays, "NotLevelTwoDays");
    public static readonly NotificationLevelCustomEnum Day = new((int)NotificationLevel.Day, "NotLevelDay");
    public static readonly NotificationLevelCustomEnum Never = new((int)NotificationLevel.Never, "NotLevelNever");
}