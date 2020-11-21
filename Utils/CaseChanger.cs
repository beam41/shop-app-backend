using System.Linq;

namespace ShopAppBackend.Utils
{
    public class CaseChanger
    {
        public static string UnderscoreToPascal(string stringToChange)
        {
            var nextIndexUp = 0;
            var newString = string.Concat(stringToChange.Select((x, i) =>
            {
                if (x != '_') return nextIndexUp == i ? x.ToString().ToUpper() : x.ToString().ToLower();

                nextIndexUp = i + 1;
                return "";
            }));

            return newString;
        }
    }
}
