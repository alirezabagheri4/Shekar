namespace Shekar.Domain.Enums;

/// <summary>باکت کارکرد برای دسته‌بندی هوشمند قیمت (کیلومتر).</summary>
public enum MileageBucket
{
    /// <summary>صفر کیلومتر.</summary>
    Zero = 0,

    /// <summary>۱ تا ۲۰ هزار.</summary>
    To20k = 1,

    /// <summary>۲۰ تا ۵۰ هزار.</summary>
    To50k = 2,

    /// <summary>۵۰ تا ۱۰۰ هزار.</summary>
    To100k = 3,

    /// <summary>۱۰۰ تا ۱۵۰ هزار.</summary>
    To150k = 4,

    /// <summary>بالای ۱۵۰ هزار.</summary>
    Above150k = 5
}
