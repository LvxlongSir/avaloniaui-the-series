using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniaMiaDev.Services;
using AvaloniaMiaDev.ViewModels;
using AvaloniaMiaDev.ViewModels.SplitViewPane;
using AvaloniaMiaDev.Views;
using CommunityToolkit.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaMiaDev;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        LiveCharts.Configure(config =>
                config
                    .AddDarkTheme()
        );
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        DisableAvaloniaDataAnnotationValidation();
        // var locator = new ViewLocator();
        // DataTemplates.Add(locator);

        var services = new ServiceCollection();
        ConfigureViewModels(services);
        ConfigureViews(services);
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        // Typed-clients
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0#typed-clients
        services.AddHttpClient<ILoginService, LoginService>(httpClient =>
            httpClient.BaseAddress = new Uri("https://dummyjson.com/"));

        var provider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(provider);

        var vm = Ioc.Default.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView { DataContext = vm };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    [Singleton(typeof(MainViewModel))]
    [Transient(typeof(HomePageViewModel))]
    [Transient(typeof(ButtonPageViewModel))]
    [Transient(typeof(TextPageViewModel))]
    [Transient(typeof(ValueSelectionPageViewModel))]
    [Transient(typeof(ImagePageViewModel))]
    [Singleton(typeof(GridPageViewModel))]
    [Singleton(typeof(DragAndDropPageViewModel))]
    [Singleton(typeof(CustomSplashScreenViewModel))]
    [Singleton(typeof(LoginPageViewModel))]
    [Singleton(typeof(SecretViewModel))]
    [Transient(typeof(ChartsPageViewModel))]
    [SuppressMessage("CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.InvalidServiceRegistrationAnalyzer", "TKEXDI0004:Duplicate service type registration")]
    internal static partial void ConfigureViewModels(IServiceCollection services);

    [Singleton(typeof(MainWindow))]
    [Transient(typeof(HomePageView))]
    [Transient(typeof(ButtonPageView))]
    [Transient(typeof(TextPageView))]
    [Transient(typeof(ValueSelectionPageView))]
    [Transient(typeof(ImagePageView))]
    [Transient(typeof(GridPageView))]
    [Transient(typeof(DragAndDropPageView))]
    [Transient(typeof(CustomSplashScreenView))]
    [Transient(typeof(LoginPageView))]
    [Transient(typeof(SecretView))]
    [Transient(typeof(ChartsPageView))]
    internal static partial void ConfigureViews(IServiceCollection services);
}
