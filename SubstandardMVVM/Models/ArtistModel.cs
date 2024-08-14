using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardLib.Metadata;

namespace SubstandardMVVM.Models;

public partial class ArtistModel : ObservableObject
{
	[ObservableProperty] private string _id;
	[ObservableProperty] private string _name;

	[ObservableProperty] private int _albumCount;
	
	[ObservableProperty] private string _coverArtId;
	[ObservableProperty] private string _artistImageUrl;

	public ArtistModel(Artist artist)
	{
		Id = artist.Id;
		Name = artist.Name;

		AlbumCount = artist.AlbumCount;

		CoverArtId = artist.CoverArtId;
		ArtistImageUrl = artist.ArtistImageUrl;
	}
}