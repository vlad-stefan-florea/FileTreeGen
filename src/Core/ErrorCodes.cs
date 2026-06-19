namespace Core
{
    public class ErrorCode
    {
        public enum Codes
        {
            Success = 0,
            Unknown = 1,

            RootAccessDenied = 100,
            DirectoryNotFound = 101,
            FileNotFound = 102,

            CannotWriteOutput = 200,

            OutOfMemory = 300,
            Canceled = 400,
        }

        public static string GetErrorMessage(ErrorCode.Codes code) =>
            code switch
            {
                Codes.Success => "Report generated successfully.",

                Codes.Unknown => "An unknown error has occurred.",

                Codes.RootAccessDenied => "Access to the target directory was denied.",

                Codes.FileNotFound => "The specified file could not be found.",

                Codes.DirectoryNotFound => "The specified directory could not be found.",

                Codes.CannotWriteOutput => "An error occurred while writing to the output file.",

                Codes.OutOfMemory => "The application ran out of memory.",

                Codes.Canceled => "The operation was canceled.",

                _ => "An unexpected error has occurred.",
            };

        public static CoreException TranslateOSException(Exception ex) =>
            ex switch
            {
                UnauthorizedAccessException => new(Codes.RootAccessDenied, ex.Message),
                FileNotFoundException => new(Codes.FileNotFound, ex.Message),
                DirectoryNotFoundException => new(Codes.DirectoryNotFound, ex.Message),
                OutOfMemoryException => new(Codes.OutOfMemory, ex.Message),
                _ => new(Codes.Unknown, ex.Message),
            };
    }

    public class CoreException : Exception
    {
        public ErrorCode.Codes Code { get; }

        public CoreException(ErrorCode.Codes code, string message)
            : base(message)
        {
            Code = code;
        }
    }
}
