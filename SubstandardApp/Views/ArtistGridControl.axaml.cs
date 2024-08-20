using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SubstandardApp.Models;
using SubstandardApp.ViewModels;

namespace SubstandardApp.Views;

public partial class ArtistGridControl : UserControl
{
	public ArtistGridControl()
	{
		InitializeComponent();
	}
	
	public ArtistGridControl(MainWindowViewModel mainWindow, List<AlbumModel> albumsToShow)
	{
		DataContext = new ArtistGridControlViewModel(mainWindow, albumsToShow);
		InitializeComponent();
	}
}