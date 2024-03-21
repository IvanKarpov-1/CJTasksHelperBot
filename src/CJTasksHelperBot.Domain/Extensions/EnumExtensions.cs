using System.Reflection;

namespace CJTasksHelperBot.Domain.Extensions;

public static class EnumExtensions
{
    public static TEnum GetNextValueUntilLastByDeclarationOrder<TEnum>(this TEnum current) where TEnum : struct, Enum
    {
        var values = typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => (TEnum)field.GetValue(null)!).ToArray();
        
        var index = Array.IndexOf(values, current);
        if (index + 1 < values.Length)
        {
            index++;
        }

        return values[index];
    }
}