namespace PlayScattergories.Server.Helpers
{
    public static class ApplicationLoggingHelper
    {
        internal static ILoggerFactory LoggerFactory;
        public static void Initialize(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }
        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }
}
