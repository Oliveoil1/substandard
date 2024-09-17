using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using SubstandardLib;
using SubstandardLib.Metadata;

namespace SubstandardApp.Models;

public partial class QueueModel : ObservableObject
{
	private readonly Client _client;
	private readonly SettingsModel _settingsModel;

	private bool _isAlbum = false;
	
	[ObservableProperty] private bool _shuffleEnabled;
	[ObservableProperty] private List<SongModel> _queue = new();
	[ObservableProperty] private List<SongModel> _queueHistory = new();
	[ObservableProperty] private SongModel? _currentSong;

	public QueueModel(Client client, SettingsModel settingsModel)
	{
		_client = client;
		_settingsModel = settingsModel;
	}

	public void LoadPlaylist(List<SongModel> songs, bool shuffle = false, bool isAlbum = false)
	{
		Queue.Clear();
		QueueHistory.Clear();
		
		Queue = songs;

		if (shuffle)
		{
			Random rng = new();
			Queue = songs.OrderBy(_ => rng.Next()).ToList();
		}

		_isAlbum = isAlbum;
	}

	public void NextSong(int songsToSkip = 0)
	{
		var qModified = new List<SongModel>(Queue);
		var qhModified = new List<SongModel>(QueueHistory);

		int skipped = 0;
		while (skipped <= songsToSkip)
		{
			if (CurrentSong != null && qModified.Count > 0)
			{
				qhModified.Insert(0, CurrentSong);
			}

			if (qModified.Count > 0)
			{
				CurrentSong = qModified[0];
				qModified.RemoveAt(0);
			}

			skipped++;
		}

		AudioPlayer.ReplayGainMode gainMode = _settingsModel.GainMode switch
		{
			AudioPlayer.ReplayGainMode.Auto => _isAlbum ? AudioPlayer.ReplayGainMode.Album : AudioPlayer.ReplayGainMode.Track,
			_ => _settingsModel.GainMode
		};
		
		_client.SetReplayGain(gainMode, CurrentSong?.TrackGain ?? 0, CurrentSong?.AlbumGain ?? 0);
		_client.PlaySong(CurrentSong?.Id ?? "null");

		Queue = qModified;
		QueueHistory = qhModified;
	}

	public void PreviousSong(int songsToSkip = 1)
	{
		var qModified = new List<SongModel>(Queue);
		var qhModified = new List<SongModel>(QueueHistory);
		
		int skipped = 0;
		while (skipped < songsToSkip)
		{
			if (CurrentSong != null && QueueHistory.Count > 0)
			{
				qModified.Insert(0, CurrentSong);
				CurrentSong = qhModified[0];
				qhModified.RemoveAt(0);
			}

			skipped++;
		}

		AudioPlayer.ReplayGainMode gainMode = _settingsModel.GainMode switch
		{
			AudioPlayer.ReplayGainMode.Auto => _isAlbum ? AudioPlayer.ReplayGainMode.Album : AudioPlayer.ReplayGainMode.Track,
			_ => _settingsModel.GainMode
		};
		
		_client.SetReplayGain(gainMode, CurrentSong?.TrackGain ?? 0, CurrentSong?.AlbumGain ?? 0);
		_client.PlaySong(CurrentSong?.Id ?? "null");
		
		Queue = qModified;
		QueueHistory = qhModified;
	}

	public void MoveSong(int originIndex, int destinationIndex)
	{
		var qModified = new List<SongModel>(Queue);
		
		SongModel toMove = qModified[originIndex];
		qModified.RemoveAt(originIndex);
		qModified.Insert(destinationIndex, toMove);

		Queue = qModified;
	}

	public void AddToQueue(List<SongModel> toAdd, int index = -1)
	{
		var qModified = new List<SongModel>(Queue);
		qModified.AddOrInsertRange(toAdd, index);
		Queue = qModified;
		Console.WriteLine(toAdd[0].Title);
	}
}