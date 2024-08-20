using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SubstandardApp.Models;
using SubstandardApp.ViewModels;

namespace SubstandardApp.Views;

public partial class SettingsControl : UserControl
{
	public SettingsControl(SettingsModel settingsModel)
	{
		DataContext = new SettingsControlViewModel(settingsModel);
		InitializeComponent();
	}
}