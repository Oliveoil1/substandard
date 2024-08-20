using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SubstandardApp.Models;
using SubstandardApp.ViewModels;

namespace SubstandardApp.Views;

public partial class PlaylistControl : UserControl
{
	public PlaylistControl(PlaylistModel playlistModel, QueueModel queueModel)
	{
		DataContext = new PlaylistControlViewModel(playlistModel, queueModel);
		InitializeComponent();
	}
	
	private void PlayNext_OnClick(object? sender, RoutedEventArgs e)
	{
		if (DataContext is PlaylistControlViewModel viewModel && sender is MenuItem { DataContext: SongModel song })
		{
			viewModel.AddToQueue(new List<SongModel>()
			{
				song
			}, true);
		}
	}

	private void PlayLast_OnClick(object? sender, RoutedEventArgs e)
	{
		if (DataContext is PlaylistControlViewModel viewModel && sender is MenuItem { DataContext: SongModel song })
		{
			viewModel.AddToQueue(new List<SongModel>()
			{
				song
			});
		}
	}
	
	private void SongList_DoubleTapped(object? sender, TappedEventArgs e)
	{
		if (DataContext is PlaylistControlViewModel viewModel && SongList.SelectedItem is SongModel)
		{
			viewModel.EnqueuePlaylist(SongList.SelectedIndex);
		}
	}
}