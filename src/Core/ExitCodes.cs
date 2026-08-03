namespace Core
{
    public enum ExitCode
    {
        Success = 0,
        Unknown = 1,

        RootAccessDenied = 100,
        RootIsEmpty = 101,

        DirectoryNotFound = 102,
        FileNotFound = 103,

        InvalidArgument = 200,
        IncompatibleArguments = 201,

        CannotWriteOutput = 300,

        OutOfMemory = 400,

        Canceled = 500,
    }

    public class ExitMessages
    {
        public static string Get(ExitCode code) =>
            code switch
            {
                ExitCode.Success => "Report generated successfully.",
                ExitCode.Unknown => "An unknown error has occurred.",

                ExitCode.RootAccessDenied => "Access to the target directory was denied.",
                ExitCode.RootIsEmpty => "The target directory cannot be empty.",
                ExitCode.DirectoryNotFound => "The specified directory could not be found.",
                ExitCode.FileNotFound => "The specified file could not be found.",

                ExitCode.InvalidArgument => "One or more provided arguments were invalid.",
                ExitCode.IncompatibleArguments =>
                    "Two or more provided arguments are incompatible.",

                ExitCode.CannotWriteOutput => "An error occurred while writing to the output file.",
                ExitCode.OutOfMemory => "The application ran out of memory.",
                ExitCode.Canceled => "The operation was canceled.",

                _ => "An unexpected error has occurred.",
            };

        public static CoreException TranslateOSException(Exception ex) =>
            ex switch
            {
                UnauthorizedAccessException => new(
                    ExitCode.RootAccessDenied,
                    Get(ExitCode.RootAccessDenied),
                    ex
                ),

                FileNotFoundException => new(ExitCode.FileNotFound, Get(ExitCode.FileNotFound), ex),

                DirectoryNotFoundException => new(
                    ExitCode.DirectoryNotFound,
                    Get(ExitCode.DirectoryNotFound),
                    ex
                ),

                OutOfMemoryException => new(ExitCode.OutOfMemory, Get(ExitCode.OutOfMemory), ex),

                _ => new(ExitCode.Unknown, Get(ExitCode.Unknown), ex),
            };
    }

    public class CoreException : Exception
    {
        public ExitCode Code { get; }

        public CoreException(ExitCode code, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            Code = code;
        }
    }
}
