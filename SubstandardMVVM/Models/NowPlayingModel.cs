using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardLib;
using SubstandardLib.Metadata;

namespace SubstandardMVVM.Models;

public partial class NowPlayingModel : ObservableObject
{
	[ObservableProperty] private SongModel _currentSong = new SongModel(new Song());
	[ObservableProperty] private AlbumModel _currentAlbum = new AlbumModel(new Album());
	[ObservableProperty] private ArtistModel _currentArtist = new ArtistModel(new Artist());

	[ObservableProperty] private string _playingArtUrl;

	[ObservableProperty] private bool _isPaused = true;
	[ObservableProperty] private bool _atSongEnd = false;

	[ObservableProperty] private float _playbackSeconds = 0;
	[ObservableProperty] private float _playbackMaxSeconds = 0;

	[ObservableProperty] private DateTime _startedPlaying = DateTime.Now;
	
	

	public void UpdateNowPlaying(NowPlayingInfo nowPlayingInfo)
	{
		CurrentSong = new SongModel(nowPlayingInfo.PlayingSong);
		CurrentAlbum = new AlbumModel(nowPlayingInfo.PlayingAlbum);
		CurrentArtist = new ArtistModel(nowPlayingInfo.PlayingArtist);

		PlayingArtUrl = nowPlayingInfo.PlayingArtUrl;

		IsPaused = nowPlayingInfo.IsPaused;
		AtSongEnd = nowPlayingInfo.AtSongEnd;

		PlaybackSeconds = nowPlayingInfo.PlaybackSeconds;
		PlaybackMaxSeconds = nowPlayingInfo.PlaybackMaxSeconds;

		StartedPlaying = nowPlayingInfo.StartedPlaying;
	}
}