using TodoApp.Models;

namespace TodoApp.Helpers;

public static class EnumHelper
{
    public static TEnum[] ToArray<TEnum>()
        where TEnum : struct, IComparable, IFormattable, IConvertible
    => (TEnum[])Enum.GetValues(typeof(TEnum));

    public static string StatusToFriendlyString(this Status status) =>
        status switch
        {

            Status.NotStarted => "⛔ Not Started",
            Status.InProgress => "⌛ In Progress",
            Status.Completed => "✅ Completed",
            Status.OnHold => "⏸️ On Hold",
            Status.Cancelled => "❌ Cancelled"
        };

    public static string PriorityToFriendlyString(this Priority priority) =>
        priority switch
        {

            Priority.Low => "🟢 Low",
            Priority.Medium => "🟡 Medium",
            Priority.High => "🔴 High",
            Priority.Critical => "🚨 Critical"

        };
}

