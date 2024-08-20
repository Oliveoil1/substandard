using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SubstandardApp.ViewModels;

public partial class TabItemViewModel : ViewModelBase
{
	[ObservableProperty] private string _header;
	[ObservableProperty] private UserControl _content;
}