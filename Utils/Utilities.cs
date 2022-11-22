using Icos5.core6.DbManager;

namespace Icos5.core6.Utils
{
    public class Utilities
    {
        public static string LeadingZero(int input)
        {
            string output = "";
            output = ((input < 10) ? "0" + input.ToString() : input.ToString());
            return output;
        }
    }
}
