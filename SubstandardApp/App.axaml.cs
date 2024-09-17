using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SubstandardApp.ViewModels;
using SubstandardApp.Views;

namespace SubstandardApp;

public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = new MainWindow
			{
				DataContext = new MainWindowViewModel(),
			};
			
			desktop.ShutdownRequested += DesktopOnShutdownRequested;
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
	{
		OnShutdownRequested(e);
	}

	protected virtual void OnShutdownRequested(ShutdownRequestedEventArgs e)
	{
		ShutdownRequested?.Invoke(this, e);
	}
	
	public event EventHandler<ShutdownRequestedEventArgs>? ShutdownRequested;
}