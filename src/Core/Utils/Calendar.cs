namespace Core.Utils
{
    public class Calendar
    {
        public static string GetTime() => DateTime.Now.ToString("HH:mm:ss");

        public static string GetDateReversed() => DateTime.Now.ToString("yyyy-MM-dd");

        public static string GetDate() => DateTime.Now.ToString("dd-MM-yyyy");
    }
}
