using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using DiscordRPC;
using DynamicData;
using FluentIcons.Common;
using ReactiveUI;
using SubstandardApp.Models;
using SubstandardApp.Views;
using SubstandardLib;
using SubstandardLib.Metadata;
using SubstandardLib.Subsonic;

namespace SubstandardApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	private readonly Dictionary<string, PlaylistModel> _playlists = new();

	[ObservableProperty] private NowPlayingModel _nowPlaying = new NowPlayingModel();
	[ObservableProperty] private IAsyncImageLoader _imageLoader = new BaseWebImageLoader();
	[ObservableProperty] private Symbol _pausePlayIcon = Symbol.Play;
	[ObservableProperty] private string _pausePlayText = "Play";

	[ObservableProperty] private string _playbackSecondsString;
	[ObservableProperty] private string _playbackMaxSecondsString;
	[ObservableProperty] private bool _seekingManually = true;

	[ObservableProperty] private bool _rightPanelOpen = false;
	[ObservableProperty] private Symbol _rightPanelSymbol = Symbol.ArrowLeft;

	[ObservableProperty] private ObservableCollection<PlaylistTreeNode> _playlistNodes;
	[ObservableProperty] private PlaylistModel _currentPlaylist = new("No Playlist", new List<Song>());
	
	[ObservableProperty] private QueueModel _queueModel;
	[ObservableProperty] private string _queueBoxHeader = "Queue (0)";
	[ObservableProperty] private string _historyBoxHeader = "History (0)";
	
	[ObservableProperty] private ObservableCollection<TabItemViewModel> _tabs = new();
	[ObservableProperty] private int _tabSelectedIndex;

	[ObservableProperty] private float _volume = 1.0f;
	
	private readonly Client _subsonicClient;
	private readonly SettingsModel _settingsModel;

	private readonly DiscordRpcClient _discordRpcClient;

	public MainWindowViewModel()
	{
		_subsonicClient = new Client();
		_settingsModel = new SettingsModel(_subsonicClient);
		_settingsModel.LoadSettings();
		
		QueueModel = new QueueModel(_subsonicClient, _settingsModel);

		_discordRpcClient = new DiscordRpcClient(Secrets.Secrets.DiscordKey);
		_discordRpcClient.Initialize();
		App? app = Application.Current as App;
		if (app != null)
			app.ShutdownRequested += (sender, args) =>
			{
				_discordRpcClient.ClearPresence();
				_discordRpcClient.Dispose();
			};
		
		SyncPlaylists();
		Update();
	}

	private async void Update()
	{
		const int discordWaitTicks = 50;
		int discordTicks = 0;
		while (true)
		{
			SeekingManually = false;
			
			NowPlayingInfo nowPlaying = await _subsonicClient.GetNowPlayingInfo();
			NowPlaying.UpdateNowPlaying(nowPlaying);

			PlaybackSecondsString = SecsToTimeStr(NowPlaying.PlaybackSeconds);
			PlaybackMaxSecondsString = SecsToTimeStr(NowPlaying.PlaybackMaxSeconds);

			PausePlayText = NowPlaying.IsPaused ? "Play" : "Pause";
			
			if (NowPlaying.AtSongEnd && QueueModel.Queue.Count > 0)
			{
				QueueModel.NextSong();
				discordTicks = 0;
			}

			SeekingManually = true;
			
			QueueBoxHeader = $"Queue ({QueueModel.Queue.Count})";
			HistoryBoxHeader = $"History ({QueueModel.QueueHistory.Count})";
			
			_subsonicClient.SetVolume(Volume);

			if (NowPlaying is { AtSongEnd: false, IsPaused: false })
			{
				if (discordTicks <= 0)
				{
					_discordRpcClient.SetPresence(new RichPresence()
					{
						Details = nowPlaying.PlayingSong.Title,
						State = $"By {nowPlaying.PlayingSong.ArtistName}",
						Timestamps = new Timestamps()
						{
							Start = nowPlaying.StartedPlaying.ToUniversalTime(),
							End = nowPlaying.StartedPlaying.ToUniversalTime().AddSeconds(nowPlaying.PlaybackMaxSeconds)
						},
						Assets = new Assets()
						{
							LargeImageText = nowPlaying.PlayingSong.AlbumName,
							LargeImageKey = nowPlaying.LastFMCoverUrl
						}
					});

					discordTicks = discordWaitTicks;
				}
			}
			else
			{
				_discordRpcClient.ClearPresence();
				discordTicks = discordWaitTicks;
			}

			discordTicks -= 1;
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
		if (playlistId is "artist:null" or "playlist:null" or "album:null")
			return;
		
		PlaylistModel playlist = _playlists[playlistId];
		Tabs.Add(new()
		{
			Header = $"Playlist: {playlist.Title}",
			Content = new PlaylistControl(playlist, QueueModel)
		});
		TabSelectedIndex = Tabs.Count - 1;
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
			albumNodes.Add(new PlaylistTreeNode("All Albums", null, artist.Id));
			
			//get albums
			List<Album> albums = fullSearch.Albums.Where(album => album.ArtistId == artist.Id).ToList();
			foreach (var album in albums)
			{
				string albumPlaylistId = $"album:{album.Id}";
				List<Song> songs = fullSearch.Songs.Where(song => song.AlbumId == album.Id).ToList();
				var newPlaylist = new PlaylistModel(album.Title, songs, true);
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

	public void MoveQueueSong(int originIndex, int destinationIndex)
	{
		QueueModel.MoveSong(originIndex, destinationIndex);
	}

	public void CloseTab(TabItemViewModel toClose)
	{
		Tabs.Remove(toClose);
	}

	public void OpenSettings()
	{
		Tabs.Add(new()
		{
			Header = "Settings",
			Content = new SettingsControl(_settingsModel)
		});
	}

	public void FixPlayback()
	{
		_subsonicClient.FixPlayback();
	}

	public void GoToArtist()
	{
		ShowArtistGrid(NowPlaying.CurrentAlbum.ArtistId);
	}
	
	public void GoToAlbum()
	{
		LoadPlaylist($"album:{NowPlaying.CurrentSong.AlbumId}");
	}

	public void SeekTo(float time)
	{
		_subsonicClient.SeekTo(time);
	}

	public async void ShowArtistGrid(string artistId)
	{
		Artist artist = await _subsonicClient.GetArtist(artistId);
		SearchResult searchResult = await _subsonicClient.Search(artist.Name);
		List<Album> albums = searchResult.Albums.Where(album => album.ArtistId == artist.Id).ToList();;
		List<AlbumModel> albumModels = new();
		foreach (var album in albums)
		{
			albumModels.Add(new AlbumModel(album));
		}
		
		Tabs.Add(new()
		{
			Header = $"Artist: {artist.Name}",
			Content = new ArtistGridControl(this, albumModels)
		});
		TabSelectedIndex = Tabs.Count - 1;
	}
}