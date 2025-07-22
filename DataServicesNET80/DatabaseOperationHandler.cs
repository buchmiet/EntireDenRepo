using System.Collections.Concurrent;

namespace DataServicesNET80;

public class DatabaseOperation
{
    public Func<Task> Task { get; set; }
    public TimeSpan DelaySeconds { get; set; }
}

public class DatabaseOperationHandler
{
    private static readonly Lazy<DatabaseOperationHandler> _instance = new Lazy<DatabaseOperationHandler>(() => new DatabaseOperationHandler());
    private readonly ConcurrentQueue<DatabaseOperation> _tasks = new ConcurrentQueue<DatabaseOperation>();
    private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
    private DatabaseOperationMediator _mediator;

    private DatabaseOperationHandler()
    {
        // Start a dedicated task to process the queue
        Task.Factory.StartNew(ProcessQueueAsync, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public static DatabaseOperationHandler Instance => _instance.Value;

    public void SetMediator(DatabaseOperationMediator mediator)
    {
        _mediator = mediator;
    }

    public void EnqueueTask(Func<Task> task, TimeSpan delaySeconds)
    {
        _tasks.Enqueue(new DatabaseOperation { Task = task, DelaySeconds = delaySeconds });
        _signal.Release();
    }

    private async Task ProcessQueueAsync()
    {
        while (true)
        {
            await _signal.WaitAsync();

            if (_tasks.TryDequeue(out DatabaseOperation operation))
            {
                using var cts = new CancellationTokenSource();
                // Start the timeout warning
                _ = _mediator?.TimeoutWarning != null
                    ? ShowTimeoutWarningAsync(cts.Token, operation.DelaySeconds)
                    : Task.CompletedTask;

                try
                {
                    // Execute the task
                    await operation.Task();
                    await cts.CancelAsync(); // Task completed, cancel the timeout warning
                }
                catch (Exception ex)
                {
                    await cts.CancelAsync(); // Task failed, cancel the timeout warning
                    await HandleException(ex);
                }
                finally
                {
                    // Ensure the warning is closed if it was shown
                    _mediator?.TimeoutWarning.Close();
                }
            }
        }
    }

    private async Task ShowTimeoutWarningAsync(CancellationToken cancellationToken, TimeSpan delaySeconds)
    {
        // Delay showing the warning to give the operation time to complete
        try
        {
            await Task.Delay(delaySeconds, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Task was cancelled, so don't show the warning
            return;
        }

        // If the operation has not yet completed, show the warning
        if (!cancellationToken.IsCancellationRequested)
        {
            await _mediator.TimeoutWarning.ShowAsync(cancellationToken, delaySeconds);
        }
    }

    private async Task HandleException(Exception ex)
    {
        // Handle the exception, for example by showing a message dialog
        if (_mediator?.MessageDialog != null)
        {
            await _mediator.MessageDialog.ShowYesNoDialogAsync(ex.Message, "Error");
        }

        // Additional exception handling logic here
        // ...
    }
}