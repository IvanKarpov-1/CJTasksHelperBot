namespace CJTasksHelperBot.Domain.Enums;

public record LanguageCodeCustomEnum(int Id, string DisplayName) : Enumeration<LanguageCodeCustomEnum>(Id, DisplayName)
{
    public static readonly LanguageCodeCustomEnum Uk = new((int)LanguageCode.Uk, "uk");
    public static readonly LanguageCodeCustomEnum EnUs = new((int)LanguageCode.EnUs, "en-US");
}