using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardApp.Models;

namespace SubstandardApp.ViewModels;

public partial class SettingsWindowViewModel(SettingsModel settingsModel, ServerInfoModel serverInfoModel) : ViewModelBase
{
	[ObservableProperty] private SettingsModel _settingsModel = settingsModel;
	[ObservableProperty] private ServerInfoModel _serverInfoModel = serverInfoModel;
}