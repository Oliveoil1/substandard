using System.Text.Json.Nodes;
using SubstandardLib.Subsonic;

namespace SubstandardLib.Metadata;

public struct Album
{
	public string Id;
	public string ArtistId;
	public string CoverArtId;
	public string CoverArtUrl;
	
	public string Title;
	public string ArtistName;
	public string Genre;
	
	public int DurationSeconds;
	public int SongCount;
	public int Year;

	public List<Song> Songs;

	public Album()
	{
		Id = "null";
		ArtistId = "null";
		CoverArtId = "null";
		Title = "Null Album";
		ArtistName = "Null Artist";
		Genre = "Music";
		DurationSeconds = 0;
		SongCount = 0;
		Year = 0000;
		Songs = new List<Song>();
	}

	public Album(JsonNode albumJson)
	{
		Id = albumJson["id"]?.GetValue<string>() ?? "null";
		ArtistId = albumJson["artistId"]?.GetValue<string>() ?? "null";
		CoverArtId = albumJson["coverArt"]?.GetValue<string>() ?? "null";
		Title = albumJson["name"]?.GetValue<string>() ?? "Null Album";
		ArtistName = albumJson["artist"]?.GetValue<string>() ?? "Null Artist";
		Genre = albumJson["genre"]?.GetValue<string>() ?? "Music";
		DurationSeconds = albumJson["duration"]?.GetValue<int>() ?? 0;
		SongCount = albumJson["songCount"]?.GetValue<int>() ?? 0;
		Year = albumJson["year"]?.GetValue<int>() ?? 0000;

		Songs = new List<Song>();
		if (albumJson["song"] != null)
		{
			foreach (JsonNode? songJson in albumJson["song"]!.AsArray())
			{
				Song song = new Song(songJson);
				Songs.Add(song);
			}
		}
		
		CoverArtUrl = Utils.HttpGetUrl(
			"getCoverArt",
			new[]
			{
				$"id={CoverArtId}",
				$"size={512}"
			}
		);
	}
}