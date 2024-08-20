using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardApp.Models;

namespace SubstandardApp.ViewModels;

public partial class SettingsControlViewModel : ViewModelBase
{
	[ObservableProperty] private SettingsModel _settingsModel = new(new());

	public SettingsControlViewModel()
	{
		//default empty constructor
	}
	
	public SettingsControlViewModel(SettingsModel settingsModel)
	{
		SettingsModel = settingsModel;
	}

	public void ApplySettings()
	{
		SettingsModel.SaveSettings();
	}
}