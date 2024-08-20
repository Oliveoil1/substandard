using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardApp.Models;
using SubstandardLib;

namespace SubstandardApp.ViewModels;

public partial class PlaylistControlViewModel : ViewModelBase
{
	[ObservableProperty] private PlaylistModel _playlistModel;
	[ObservableProperty] private QueueModel _queueModel;
	
	public PlaylistControlViewModel(PlaylistModel playlistModel, QueueModel queueModel)
	{
		PlaylistModel = playlistModel;
		QueueModel = queueModel;
	}
	public PlaylistControlViewModel()
	{
		PlaylistModel = new PlaylistModel("Null", new());
		QueueModel = new QueueModel(new Client());
	}
	
	public void EnqueuePlaylist(int trackIndex)
	{
		QueueModel.LoadPlaylist(PlaylistModel.Songs);
		QueueModel.NextSong(trackIndex);
	}
	
	public void ShuffleEnqueuePlaylist(int trackIndex)
	{
		QueueModel.LoadPlaylist(PlaylistModel.Songs, true);
		QueueModel.NextSong();
	}
	
	public void AddToQueue(List<SongModel> songs, bool playNext = false)
	{
		int index = -1;
		if (playNext)
			index = 0;
		
		QueueModel.AddToQueue(songs, index);
	}
}