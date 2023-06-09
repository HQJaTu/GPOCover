using GPOCover.Cover;
using GPOCover.Cover.Configuration;
using static System.Environment;

namespace GPOCover;


public sealed class WindowsService : BackgroundService
{
    private readonly CoverService _jokeService;
    public readonly uint _jokeLogPercent = 5;
    private readonly ILogger<WindowsService> _logger;

    public WindowsService(
        CoverService jokeService,
        ILogger<WindowsService> logger)
    {
        (_jokeService, _logger) = (jokeService, logger);

        var commonpath = GetFolderPath(SpecialFolder.CommonApplicationData);
        var configPath = Path.Combine(commonpath, @"GPOCover");
        var configDir = new DirectoryInfo(configPath);
        var config = CoverConfigurationReader.Read(configDir, _logger);
        this._jokeService.Configure(config);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var joke = this._jokeService.GetJoke();
                var randomValue = Random.Shared.Next(100);
                if (randomValue < this._jokeLogPercent)
                    this._logger.LogWarning("{Joke}", joke);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    } // end ExecuteAsync

} // end class WindowsService
