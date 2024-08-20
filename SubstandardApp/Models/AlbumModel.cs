using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardLib.Metadata;

namespace SubstandardApp.Models;

public partial class AlbumModel : ObservableObject
{
	[ObservableProperty] private string _id;
	[ObservableProperty] private string _artistId;
	[ObservableProperty] private string _coverArtId;
	[ObservableProperty] private string _coverArtUrl;
	
	[ObservableProperty] private string _title;
	[ObservableProperty] private string _artistName;
	[ObservableProperty] private string _genre;

	[ObservableProperty] private int _durationSeconds;
	[ObservableProperty] private int _songCount;
	[ObservableProperty] private int _year;

	[ObservableProperty] private List<SongModel> _songs;

	public AlbumModel(Album album)
	{
		Id = album.Id;
		ArtistId = album.ArtistId;
		CoverArtId = album.CoverArtId;
		CoverArtUrl = album.CoverArtUrl;

		Title = album.Title;
		ArtistName = album.ArtistName;
		Genre = album.Genre;

		DurationSeconds = album.DurationSeconds;
		SongCount = album.SongCount;
		Year = album.SongCount;
		
		Songs = new List<SongModel>();
		foreach (var song in album.Songs)
		{
			Songs.Add(new SongModel(song));
		}
	}
}