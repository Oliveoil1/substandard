using System.ComponentModel;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardLib.Metadata;
using SubstandardLib.Subsonic;

namespace SubstandardMVVM.Models;

public partial class SongModel : ObservableObject
{
	[ObservableProperty] private IAsyncImageLoader _imageLoader = new BaseWebImageLoader();
	
	[ObservableProperty] private string _id;
	[ObservableProperty] private string _albumId;
	[ObservableProperty] private string _artistId;
	[ObservableProperty] private string _coverArtId;
	[ObservableProperty] private string _coverArtUrl;
	
	[ObservableProperty] private string _title;
	[ObservableProperty] private string _albumName;
	[ObservableProperty] private string _artistName;
	
	[ObservableProperty] private int _durationSeconds;
	[ObservableProperty] private int _bitrate;
	[ObservableProperty] private int _discNumber;
	[ObservableProperty] private int _trackNumber;
	
	[ObservableProperty] private bool _starred;
	
	
	public SongModel(Song song)
	{
		Id = song.Id;
		AlbumId = song.AlbumId;
		ArtistId = song.ArtistId;
		CoverArtId = song.CoverArtId;
		CoverArtUrl = song.CoverArtUrl;

		Title = song.Title;
		AlbumName = song.AlbumName;
		ArtistName = song.ArtistName;

		DurationSeconds = song.DurationSeconds;
		Bitrate = song.Bitrate;
		DiscNumber = song.DiscNumber;
		TrackNumber = song.TrackNumber;

		Starred = song.Starred;
	}
}