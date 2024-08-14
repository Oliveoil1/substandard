using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using FluentIcons.Common;
using ReactiveUI;
using SubstandardLib;
using SubstandardLib.Metadata;
using SubstandardLib.Subsonic;
using SubstandardMVVM.Models;

namespace SubstandardMVVM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	private Dictionary<string, PlaylistModel> _playlists = new();

	[ObservableProperty] private NowPlayingModel _nowPlaying = new NowPlayingModel();
	[ObservableProperty] private IAsyncImageLoader _imageLoader = new BaseWebImageLoader();
	[ObservableProperty] private Symbol _pausePlayIcon = Symbol.Play;
	[ObservableProperty] private string _pausePlayText = "Play";

	[ObservableProperty] private string _playbackSecondsString;
	[ObservableProperty] private string _playbackMaxSecondsString;

	[ObservableProperty] private bool _rightPanelOpen = false;
	[ObservableProperty] private Symbol _rightPanelSymbol = Symbol.ArrowLeft;

	[ObservableProperty] private ObservableCollection<PlaylistTreeNode> _playlistNodes;
	[ObservableProperty] private PlaylistModel _currentPlaylist = new("No Playlist", new List<Song>());
	
	[ObservableProperty] private QueueModel _queueModel;
	
	private readonly Client _subsonicClient;
	private readonly SettingsModel _settingsModel;

	public MainWindowViewModel()
	{
		_subsonicClient = new Client();
		_settingsModel = new SettingsModel();
		ServerInfoModel serverInfoModel = new();
		serverInfoModel.LoadCredentials(_settingsModel);
		
		_subsonicClient.Login(serverInfoModel.GetServerInfo());
		
		QueueModel = new QueueModel(_subsonicClient);

		SyncPlaylists();
		Update();
	}

	private async void Update()
	{
		while (true)
		{
			NowPlayingInfo nowPlaying = await _subsonicClient.GetNowPlayingInfo();
			NowPlaying.UpdateNowPlaying(nowPlaying);

			PlaybackSecondsString = SecsToTimeStr(NowPlaying.PlaybackSeconds);
			PlaybackMaxSecondsString = SecsToTimeStr(NowPlaying.PlaybackMaxSeconds);

			PausePlayText = NowPlaying.IsPaused ? "Play" : "Pause";
			
			if (NowPlaying.AtSongEnd)
			{
				QueueModel.NextSong();
			}
			
			await Task.Delay(100);
		}
	}

	private string SecsToTimeStr(float seconds)
	{
		int secondsInt = (int)Math.Floor(seconds);

		int mins = secondsInt / 60;
		int secs = secondsInt % 60;

		return $"{mins}:{secs:D2}";
	}

	public void PausePlayCommand()
	{
		_subsonicClient.TogglePause();
	}

	public void NextSongCommand()
	{
		QueueModel.NextSong();
	}
	
	public void PreviousSongCommand()
	{
		QueueModel.PreviousSong();
	}

	public void ToggleStarred()
	{
		_subsonicClient.SetStar(!NowPlaying.CurrentSong.Starred, NowPlaying.CurrentSong.Id);
	}

	public void LoadPlaylist(string playlistId)
	{
		CurrentPlaylist = _playlists[playlistId];
	}

	public async void SyncPlaylists()
	{
		SearchResult fullSearch = await _subsonicClient.Search("");

		//Library Tree
		_playlists["allSongs"] = new PlaylistModel("All songs", fullSearch.Songs);
		_playlists["allSongs"].Sort(PlaylistModel.SortMode.Artist);

		List<Song> favs = fullSearch.Songs.Where(song => song.Starred).ToList();
		_playlists["favSongs"] = new PlaylistModel("Favourites", favs);
		_playlists["favSongs"].Sort(PlaylistModel.SortMode.Artist);
		
		PlaylistTreeNode libraryNode = new PlaylistTreeNode("Library")
		{
			SubNodes = new ObservableCollection<PlaylistTreeNode>()
			{
				new PlaylistTreeNode("All Songs", "allSongs"),
				new PlaylistTreeNode("Favourited Songs", "favSongs")
			}
		};
		
		//Playlists Tree
		PlaylistTreeNode playlistsNode = new PlaylistTreeNode("Playlists");
		
		List<Playlist> playlists = await _subsonicClient.GetPlaylists();
		List<PlaylistTreeNode> playlistNodes = new();
		foreach (var playlist in playlists)
		{
			string id = $"playlist:{playlist.Id}";
			_playlists[id] = new PlaylistModel(playlist);
			playlistNodes.Add(new PlaylistTreeNode(playlist.Title, id));
		}

		playlistsNode.SubNodes = new(playlistNodes);
		
		//Artists Tree
		PlaylistTreeNode artistsNode = new("Artists");

		List<Artist> artists = fullSearch.Artists;
		List<PlaylistTreeNode> artistNodes = new();
		
		foreach (var artist in artists)
		{
			List<PlaylistTreeNode> albumNodes = new();
			string id = $"artist:{artist.Id}";
			_playlists[id] = new(artist.Name, fullSearch.Songs.Where(song => fullSearch.Albums.Find(album => album.Id ==  song.AlbumId).ArtistId == artist.Id).ToList());
			_playlists[id].Sort(PlaylistModel.SortMode.Title);
			PlaylistTreeNode artistNode = new(artist.Name);
			albumNodes.Add(new PlaylistTreeNode("All Songs", id));
			
			//get albums
			List<Album> albums = fullSearch.Albums.Where(album => album.ArtistId == artist.Id).ToList();
			foreach (var album in albums)
			{
				string albumPlaylistId = $"album:{album.Id}";
				List<Song> songs = fullSearch.Songs.Where(song => song.AlbumId == album.Id).ToList();
				var newPlaylist = new PlaylistModel(album.Title, songs);
				newPlaylist.TrackListTrackNum = true;
				newPlaylist.TrackListDiscNum = newPlaylist.MultiDisc;
				newPlaylist.Sort(PlaylistModel.SortMode.TrackNumber);
				_playlists[albumPlaylistId] = newPlaylist;
				PlaylistTreeNode albumNode = new(album.Title, albumPlaylistId);
				albumNodes.Add(albumNode);
			}
			
			artistNode.SubNodes = new(albumNodes);
			artistNodes.Add(artistNode);
		}

		artistNodes.Sort((x, y) => String.Compare(x.Title, y.Title, StringComparison.Ordinal));
		artistsNode.SubNodes = new(artistNodes);

		PlaylistNodes = new ObservableCollection<PlaylistTreeNode>()
		{
			libraryNode,
			playlistsNode,
			artistsNode
		};
	}

	public void ScanLibrary()
	{
		_subsonicClient.StartScan();
	}

	public void EnqueuePlaylist(int trackIndex)
	{
		QueueModel.LoadPlaylist(CurrentPlaylist.Songs);
		QueueModel.NextSong(trackIndex);
	}
	
	public void ShuffleEnqueuePlaylist(int trackIndex)
	{
		QueueModel.LoadPlaylist(CurrentPlaylist.Songs, true);
		QueueModel.NextSong();
	}

	public void MoveQueueSong(int originIndex, int destinationIndex)
	{
		QueueModel.MoveSong(originIndex, destinationIndex);
	}

	public void AddToQueue(List<SongModel> songs, bool playNext = false)
	{
		int index = -1;
		if (playNext)
			index = 0;
		
		QueueModel.AddToQueue(songs, index);
	}
}