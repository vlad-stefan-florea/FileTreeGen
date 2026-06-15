namespace Core.Utils
{
    public class Calendar
    {
        public static string GetTime() => DateTime.Now.ToString("HH:mm:ss");

        public static string GetSimpleDate() => DateTime.Now.ToString("yyyy-MM-dd");

        public static string GetFullDate() => DateTime.Now.ToString("ddd-dd-MM-yyyy");
    }
}
