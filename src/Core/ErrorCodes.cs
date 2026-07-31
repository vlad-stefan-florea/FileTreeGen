namespace Core
{
    public class ExitCodes
    {
        public const int Success = 0,
            Unknown = 1,
            RootAccessDenied = 100,
            RootIsEmpty = 101,
            DirectoryNotFound = 102,
            FileNotFound = 103,
            InvalidArgument = 200,
            CannotWriteOutput = 300,
            OutOfMemory = 400,
            Canceled = 500;

        public static string GetCodeMessage(int code) =>
            code switch
            {
                Success => "Report generated successfully.",
                Unknown => "An unknown error has occurred.",

                RootAccessDenied => "Access to the target directory was denied.",
                RootIsEmpty => "The target directory cannot be empty.",
                DirectoryNotFound => "The specified directory could not be found.",
                FileNotFound => "The specified file could not be found.",

                InvalidArgument => "One or more provided arguments were invalid.",

                CannotWriteOutput => "An error occurred while writing to the output file.",
                OutOfMemory => "The application ran out of memory.",
                Canceled => "The operation was canceled.",

                _ => "An unexpected error has occurred.",
            };

        public static CoreException TranslateOSException(Exception ex) =>
            ex switch
            {
                UnauthorizedAccessException => new(RootAccessDenied, ex.Message),
                FileNotFoundException => new(FileNotFound, ex.Message),
                DirectoryNotFoundException => new(DirectoryNotFound, ex.Message),
                OutOfMemoryException => new(OutOfMemory, ex.Message),
                _ => new(Unknown, ex.Message),
            };
    }

    public class CoreException : Exception
    {
        public int Code { get; }

        public CoreException(int code, string message)
            : base(message)
        {
            Code = code;
        }
    }
}
