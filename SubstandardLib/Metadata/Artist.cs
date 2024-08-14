using System.Text.Json.Nodes;

namespace SubstandardLib.Metadata;

public struct Artist
{
	public string Id;
	public string Name;

	public int AlbumCount;

	public string CoverArtId;
	public string ArtistImageUrl;

	public Artist()
	{
		Id = "null";
		Name = "Null Artist";
		AlbumCount = 0;
		CoverArtId = "null";
		ArtistImageUrl = "null";
	}

	public Artist(JsonNode jsonArtist)
	{
		Id = jsonArtist["id"]?.GetValue<string>() ?? "null";
		Name = jsonArtist["name"]?.GetValue<string>() ?? "Null Artist";
		AlbumCount = jsonArtist["albumCount"]?.GetValue<int>() ?? 0;
		CoverArtId = jsonArtist["coverArt"]?.GetValue<string>() ?? "null";
		ArtistImageUrl = jsonArtist["artistImageUrl"]?.GetValue<string>() ?? "null";
	}
}