using Serilog;

namespace DataServicesNET80;

public class DatabaseOperationExecutor
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static readonly Lazy<DatabaseOperationExecutor> _instance = new Lazy<DatabaseOperationExecutor>(() => new DatabaseOperationExecutor());
    private DatabaseOperationMediator _mediator;

    // Prywatny konstruktor
    private DatabaseOperationExecutor()
    {
           
    }

    public static DatabaseOperationExecutor Instance => _instance.Value;

    public void SetMediator(DatabaseOperationMediator mediator)
    {
           
        _mediator = mediator;
    }

    private async Task ShowConnectionTimeoutWarningAsync(CancellationToken cancellationToken, TimeSpan timeOut)
    {
        await _mediator.TimeoutWarning.ShowAsync(cancellationToken, timeOut).ConfigureAwait(false);
    }

    public enum DatabaseOperationResult
    {
        Success,
        Timeout,
        Error,
        Ongoing
    }

    public async Task<DatabaseOperationResult> ExecuteWithRetryAsync(Func<Task> action, TimeSpan timeOut)
    {
        try
        {
            return await ExecuteAsync(action, timeOut).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
                   
            Log.Error(ex.ToString());
            return DatabaseOperationResult.Error; 
        }
    }


    public async Task<DatabaseOperationResult> ExecuteAsync(Func<Task> action, TimeSpan timeOut)
    {
          
        await _semaphore.WaitAsync().ConfigureAwait(false);

        DatabaseOperationResult result = await Task.Run(async () =>
        {
            using var cts = new CancellationTokenSource();
            var showWarningTask = ShowConnectionTimeoutWarningAsync(cts.Token, timeOut);

            try
            {
                await action().ConfigureAwait(false);
                await cts.CancelAsync();
                return DatabaseOperationResult.Success;
            }
            catch (Exception ex)
            {
                await cts.CancelAsync();
                bool dialogResult = await _mediator.MessageDialog.ShowYesNoDialogAsync(
                    denLanguageResourses.Resources.DatabaseAccessError, denLanguageResourses.Resources.ErrorTitle).ConfigureAwait(false); 
                return dialogResult  ? DatabaseOperationResult.Timeout : DatabaseOperationResult.Error;
            }
            finally
            {
                await showWarningTask.ConfigureAwait(false);
            }
        });

        _semaphore.Release();
        return result;
    }
}