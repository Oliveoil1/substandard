using System.Collections.Generic;
using System.Reactive;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardApp.Models;

namespace SubstandardApp.ViewModels;

public partial class ArtistGridControlViewModel : ViewModelBase
{
	private MainWindowViewModel _mainWindow;
	[ObservableProperty] private List<AlbumModel> _albumsToShow = new();
	[ObservableProperty] private int _gridSize = 250;
	[ObservableProperty] private IAsyncImageLoader _imageLoader = new BaseWebImageLoader();

	public ArtistGridControlViewModel(MainWindowViewModel mainWindow, List<AlbumModel> albumsToShow)
	{
		_mainWindow = mainWindow;
		AlbumsToShow = albumsToShow;
	}

	public void AlbumClicked(AlbumModel albumModel)
	{
		_mainWindow.LoadPlaylist($"album:{albumModel.Id}");
	}
}