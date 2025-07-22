using Microsoft.Extensions.DependencyInjection;
using System;

namespace denMVVM;

public class ViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TViewModel Create<TViewModel>(params object[] additionalArgs) where TViewModel : class
    {
        // Odbieranie instancji ViewModelu z kontenera DI
        var viewModel = _serviceProvider.GetService<TViewModel>();

        if (viewModel == null)
            throw new InvalidOperationException($"Type {typeof(TViewModel).Name} has not been registered.");

        // Tutaj możesz przekazać dodatkowe argumenty do ViewModelu, jeśli jest taka potrzeba.
        // Na przykład możesz poszukać odpowiedniej metody w ViewModelu i ją wywołać.

        return viewModel;
    }
}