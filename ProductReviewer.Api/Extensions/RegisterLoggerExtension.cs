using Serilog;

namespace ProductReviewer.Api.Extensions
{
    public static class RegisterLoggerExtension
    {
        public static void AddFileLogger(this IHostBuilder hostBuilder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: "logs/log.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 5_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true
                )
                .CreateLogger();

            hostBuilder.UseSerilog();
        }
    }
}
