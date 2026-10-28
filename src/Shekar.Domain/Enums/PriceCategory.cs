namespace Shekar.Domain.Enums;

/// <summary>
/// دسته‌های قیمتی (میلیون تومان): زیر ۱۰۰۰ | ۱۰۰۰–۱۵۰۰ | ۱۵۰۰–۲۰۰۰ | ۲۰۰۰–۳۰۰۰ |
/// ۳۰۰۰–۴۰۰۰ | ۴۰۰۰–۸۰۰۰ | ۸۰۰۰–۱۶۰۰۰ | بالای ۱۶۰۰۰.
/// </summary>
public enum PriceCategory
{
    Under1B = 1,
    From1To1_5B = 2,
    From1_5To2B = 3,
    From2To3B = 4,
    From3To4B = 5,
    From4To8B = 6,
    From8To16B = 7,
    Above16B = 8
}
