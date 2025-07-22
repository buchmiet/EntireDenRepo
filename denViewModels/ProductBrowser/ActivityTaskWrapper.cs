using denSharedLibrary;
using System.Collections.ObjectModel;

namespace denViewModels;

public class ActivityTaskWrapper
{
    private readonly ObservableCollection<ActivityViewModel> _activityViewModelCollection;
    IDispatcherService _dispatcherService;

    public ActivityTaskWrapper(ObservableCollection<ActivityViewModel> activityViewModelCollection, IDispatcherService dispatcherService)
    {
        _activityViewModelCollection = activityViewModelCollection;
        _dispatcherService = dispatcherService;
    }

    public async Task ExecuteTaskAsync(Task task, string taskName, CancellationToken cancellationToken = default)
    {
        var viewModel = new ActivityViewModel
        {
            CurrentImage = @"pack://application:,,,/Data/perfectcircle32.gif",
            TaskName = taskName,
        };

        _dispatcherService.Invoke(() => _activityViewModelCollection.Add(viewModel));
        try
        {
            await task.ConfigureAwait(false);

            _dispatcherService.Invoke(() => viewModel.CurrentImage = @"pack://application:,,,/Data/tick.gif");
            viewModel.TickTack = true;
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            viewModel.CurrentImage = @"Data\cancelled.gif";
        }
        finally
        {
            if (cancellationToken.IsCancellationRequested)
            {

                _dispatcherService.Invoke(() => _activityViewModelCollection.Remove(viewModel));
            }
            else if (viewModel.CurrentImage == @"pack://application:,,,/Data/tick.gif")
            {
                _dispatcherService.Invoke(() => _activityViewModelCollection.Remove(viewModel));
            }
        }
    }






    public async Task<T> ExecuteTaskWithResultAsync<T>(Task<T> task, string taskName, CancellationToken cancellationToken = default)

    {
        var viewModel = new ActivityViewModel
        {
            CurrentImage = @"pack://application:,,,/Data/perfectcircle32.gif",
            TaskName = taskName,
        };

        _dispatcherService.Invoke(() => _activityViewModelCollection.Add(viewModel));


        T result = default;

        try
        {
            result = await task.ConfigureAwait(false);
            _dispatcherService.Invoke(() => viewModel.CurrentImage = @"pack://application:,,,/Data/tick.gif");
            viewModel.TickTack = true;
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            //     viewModel.CurrentImage = @"Data\cancelled.gif";
        }
        finally
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _dispatcherService.Invoke(() => _activityViewModelCollection.Remove(viewModel));
            }
            else if (viewModel.CurrentImage == @"pack://application:,,,/Data/tick.gif")
            {
                _dispatcherService.Invoke(() => _activityViewModelCollection.Remove(viewModel));
            }
        }

        return result;
    }

}