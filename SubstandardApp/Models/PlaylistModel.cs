using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardLib.Metadata;

namespace SubstandardApp.Models;

public partial class PlaylistModel : ObservableObject
{
	public enum SortMode
	{
		TrackNumber,
		Title,
		Artist
	}
	
	[ObservableProperty] private string _title;
	[ObservableProperty] private int _duration;
	[ObservableProperty] private int _songCount;

	[ObservableProperty] private string _durationString;
	[ObservableProperty] private string _songCountString;
	[ObservableProperty] private bool _multiDisc;

	[ObservableProperty] private bool _trackListDiscNum;
	[ObservableProperty] private bool _trackListTrackNum;
	[ObservableProperty] private bool _trackListArtist = true;
	[ObservableProperty] private bool _trackListTitle = true;
	[ObservableProperty] private bool _trackListLength = true;
	[ObservableProperty] private bool _trackListStarred = true;

	[ObservableProperty] private List<SongModel> _songs;
	
	private string SecsToTimeStr(float seconds)
	{
		int secondsInt = (int)Math.Floor(seconds);

		int mins = secondsInt / 60;
		int secs = secondsInt % 60;

		return $"{mins}:{secs:D2}";
	}

	private List<SongModel> CreateSongModels(List<Song> songs)
	{
		var songModels = new List<SongModel>();
		int firstDisc = songs.Count > 0 ? songs[0].DiscNumber : 0;
		
		foreach (var song in songs)
		{
			if (song.DiscNumber != firstDisc)
				MultiDisc = true;
			
			Duration += song.DurationSeconds;
			var songModel = new SongModel(song);
			
			songModels.Add(songModel);
		}

		return songModels;
	}

	public PlaylistModel(Playlist playlist)
	{
		Title = playlist.Title;
		Duration = playlist.Duration;
		SongCount = playlist.SongCount;

		DurationString = SecsToTimeStr(Duration);
		SongCountString = $"{SongCount} songs";

		Songs = CreateSongModels(playlist.Songs);
	}

	public PlaylistModel(string title, List<Song> songs)
	{
		Title = title;
		SongCount = songs.Count;

		Songs = CreateSongModels(songs);

		DurationString = SecsToTimeStr(Duration);
		SongCountString = $"{SongCount} songs";
		
		// Songs.Sort((x, y) => x.TrackNumber.CompareTo(y.TrackNumber));
	}

	public void Sort(SortMode sortBy)
	{
		List<SongModel> sorted = new();

		switch (sortBy)
		{
			case SortMode.TrackNumber:
				sorted = Songs.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber).ToList();
				break;
			case SortMode.Title:
				sorted = Songs.OrderBy(s => s.Title).ToList();
				break;
			case SortMode.Artist:
				sorted = Songs.OrderBy(s => s.ArtistName).ThenBy(s => s.Title).ToList();
				break;
		}

		Songs = sorted;
	}
}