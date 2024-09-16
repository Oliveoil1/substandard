using System.Text.Json.Nodes;
using SubstandardLib.Metadata;
using SubstandardLib.Subsonic;

namespace SubstandardLib;

public class Client
{
	// public SearchResult Search(string query)
	// {
	// 	var result = new SearchResult();
	// }
	private bool _hasSubmittedScrobble = false;

	private AudioPlayer _audioPlayer;
	private string _playingId;

	private NowPlayingInfo _mostRecentNowPlaying = new();

	public Client()
	{
		_audioPlayer = new AudioPlayer();
		_playingId = "null";
	}

	public void Login(ServerInfo serverInfo)
	{
		Utils.ServerInfo = serverInfo;
	}

	public void PlaySong(string id)
	{
		_playingId = id;
		string streamUrl = Utils.HttpGetUrl(
			"stream", 
			new []
			{
				$"id={id}"
			});
		
		_audioPlayer.PlayUrl(streamUrl);

		_hasSubmittedScrobble = false;
	}

	public void TogglePause()
	{
		_audioPlayer.TogglePause();
	}

	public void SetVolume(int volume)
	{
		_audioPlayer.SetVolume(volume);
	}

	public void SeekTo(float seconds)
	{
		_audioPlayer.SeekTo(seconds);
	}

	public async void FixPlayback()
	{
		var nowPlaying = await GetNowPlayingInfo();
		float stuckAtTime = nowPlaying.PlaybackSeconds;
		PlaySong(_playingId);
		SeekTo(stuckAtTime);
	}

	public async Task<NowPlayingInfo> GetNowPlayingInfo()
	{
		NowPlayingInfo info = new NowPlayingInfo();
		
		if (_mostRecentNowPlaying.PlayingSong.Id != _playingId)
		{
			Song playingSong = await GetSong(_playingId);

			info.PlayingSong = playingSong;
			info.PlayingAlbum = await GetAlbum(playingSong.AlbumId);
			info.PlayingArtist = await GetArtist(playingSong.ArtistId);

			info.PlayingArtUrl = Utils.HttpGetUrl(
				"getCoverArt",
				new[]
				{
					$"id={playingSong.CoverArtId}",
					$"size={512}"
				}
			);
			info.StartedPlaying = DateTime.Now;
			info.AtSongEnd = false;

			Scrobble(info);
		}
		else
		{
			info = _mostRecentNowPlaying;
			info.PlayingSong.Starred = _mostRecentNowPlaying.PlayingSong.Starred;
		}
		
		info.IsPaused = _audioPlayer.GetPaused();
		info.PlaybackSeconds = _audioPlayer.GetPlaybackSeconds();
		info.PlaybackMaxSeconds = _audioPlayer.GetPlaybackMaxSeconds();

		_mostRecentNowPlaying = info;
		
		if(info.PlaybackSeconds / info.PlaybackMaxSeconds > 0.5)
			Scrobble(info, true);
		
		if (_audioPlayer.GetStopped())
		{
			info.AtSongEnd = true;
		}

		return info;
	}

	public async Task<Song> GetSong(string id)
	{
		var response = await Utils.GetServerResponse(
			"getSong",
			new []
			{
				$"id={id}"
			}
		);

		JsonNode? jsonResponse = Utils.TryParseSubsonic(response);
		if (jsonResponse != null)
		{
			JsonNode? songJson = jsonResponse!["subsonic-response"]!["song"];
			Song song = new Song(songJson);
			return song;
		}
		
		return new Song();
	}
	
	public async Task<Album> GetAlbum(string id)
	{
		var response = await Utils.GetServerResponse(
			"getAlbum",
			new []
			{
				$"id={id}"
			}
		);

		JsonNode? jsonResponse = Utils.TryParseSubsonic(response);
		if (jsonResponse != null)
		{
			JsonNode? albumJson = jsonResponse!["subsonic-response"]!["album"];
			Album album = new Album(albumJson);
			return album;
		}
		
		return new Album();
	}
	
	public async Task<Artist> GetArtist(string id)
	{
		var response = await Utils.GetServerResponse(
			"getArtist",
			new []
			{
				$"id={id}"
			}
		);

		JsonNode? jsonResponse = Utils.TryParseSubsonic(response);
		if (jsonResponse != null)
		{
			JsonNode? artistJson = jsonResponse!["subsonic-response"]!["artist"];
			Artist artist = new Artist(artistJson);
			return artist;
		}
		
		return new Artist();
	}

	public async Task<Playlist> GetPlaylist(string id)
	{
		var response = await Utils.GetServerResponse(
			"getPlaylist",
			new[]
			{
				$"id={id}"
			}
		);

		JsonNode? jsonResponse = Utils.TryParseSubsonic(response);
		if (jsonResponse != null)
		{
			JsonNode? playlistJson = jsonResponse!["subsonic-response"]!["playlist"];
			Playlist playlist = new Playlist(playlistJson);
			return playlist;
		}

		return new Playlist();
	}

	public async Task<List<Playlist>> GetPlaylists()
	{
		List<Playlist> playlists = new();
		
		var response = await Utils.GetServerResponse(
			"getPlaylists"
		);
		
		JsonNode? jsonResponse = Utils.TryParseSubsonic(response);
		if (jsonResponse != null)
		{
			JsonArray? playlistsJson = jsonResponse!["subsonic-response"]!["playlists"]!["playlist"]?.AsArray();

			if (playlistsJson != null)
			{
				foreach (var playlistJson in playlistsJson)
				{
					playlists.Add(await GetPlaylist(playlistJson!["id"]!.GetValue<string>()));
				}
			}
			
			// Playlist playlist = new Playlist(playlistJson);
		}
		
		return playlists;
	}
	
	public async Task<SearchResult> Search(string query)
	{
		SearchResult result = new SearchResult();
		
		var response = await Utils.GetServerResponse(
			"search3",
			new[]
			{
				$"query={query}",
				"songCount=99999",
				"albumCount=99999",
				"artistCount=99999"
			});
		
		JsonNode? jsonResponse = Utils.TryParseSubsonic(response);
		if (jsonResponse != null)
		{
			JsonNode searchResults = jsonResponse!["subsonic-response"]!["searchResult3"]!;
			if (searchResults["song"] != null)
			{
				JsonArray songs = searchResults["song"]!.AsArray();
				foreach (JsonNode jsonSong in songs)
				{
					Song song = new Song(jsonSong);
					result.Songs.Add(song);
				}
			}

			if (searchResults["album"] != null)
			{
				JsonArray albums = searchResults["album"]!.AsArray();
				foreach (JsonNode jsonAlbum in albums)
				{
					Album album = new Album(jsonAlbum);
					result.Albums.Add(album);
				}
			}
			if (searchResults["artist"] != null)
			{
				JsonArray artists = searchResults["artist"]!.AsArray();
				foreach (JsonNode jsonArtist in artists)
				{
					Artist artist = new Artist(jsonArtist);
					result.Artists.Add(artist);
				}
			}
		}

		return result;
	}

	private async void Scrobble(NowPlayingInfo info, bool submission = false)
	{
		if (submission)
		{
			if (_hasSubmittedScrobble)
				return;

			_hasSubmittedScrobble = true;
		}

		string response = await Utils.GetServerResponse(
			"scrobble",
			new[]
			{
				$"id={info.PlayingSong.Id}",
				$"time={new DateTimeOffset(info.StartedPlaying).ToUnixTimeMilliseconds()}",
				$"submission={submission}"
			}
		);
	}

	public async void SetStar(bool starred, string id = "", string albumId = "", string artistId = "")
	{
		string request = starred ? "star" : "unstar";

		string response = await Utils.GetServerResponse(
			request,
			new[]
			{
				$"id={id}",
				$"albumId={albumId}",
				$"artistId={artistId}"
			}
		);

		if (id == _mostRecentNowPlaying.PlayingSong.Id)
		{
			_mostRecentNowPlaying.PlayingSong.Starred = starred;
		}
	}

	public async void StartScan()
	{
		string response = await Utils.GetServerResponse(
			"startScan"
		);
	}
}