using System.Text.Json.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardLib.Subsonic;

namespace SubstandardLib.Metadata;

public struct Song
{
	public string Id;
	public string AlbumId;
	public string ArtistId;
	public string CoverArtId;
	
	public string Title;
	public string AlbumName;
	public string ArtistName;
	
	public int DurationSeconds;
	public int Bitrate;
	public int DiscNumber;
	public int TrackNumber;

	public float TrackGain;
	public float AlbumGain;

	public bool Starred;
	
	public string CoverArtUrl;
	
	public Song()
	{
		Id = "null";
		AlbumId = "null";
		ArtistId = "null";
		CoverArtId = string.Empty;
		Title = "Null Name";
		AlbumName = "Null Album";
		ArtistName = "Null Artist";
		DurationSeconds = 0;
		Bitrate = 0;
		DiscNumber = 0;
		TrackNumber = 0;
		TrackGain = 0;
		AlbumGain = 0;
		Starred = false;
		CoverArtUrl = string.Empty;
	}
	public Song(JsonNode jsonSong)
	{
		Id = jsonSong["id"]?.GetValue<string>() ?? "null";
		AlbumId = jsonSong["albumId"]?.GetValue<string>() ?? "null";
		ArtistId = jsonSong["artistId"]?.GetValue<string>() ?? "null";
		CoverArtId = jsonSong["coverArt"]?.GetValue<string>() ?? String.Empty;
		Title = jsonSong["title"]?.GetValue<string>() ?? "Null Name";
		AlbumName = jsonSong["album"]?.GetValue<string>() ?? "Null Album";
		ArtistName = jsonSong["artist"]?.GetValue<string>() ?? "Null Artist";
		DurationSeconds = jsonSong["duration"]?.GetValue<int>() ?? 0;
		Bitrate = jsonSong["bitRate"]?.GetValue<int>() ?? 0;
		DiscNumber = jsonSong["discNumber"]?.GetValue<int>() ?? 0;
		TrackNumber = jsonSong["track"]?.GetValue<int>() ?? 0;
		TrackGain = jsonSong["replayGain"]?["trackGain"]?.GetValue<float>() ?? 0;
		AlbumGain = jsonSong["replayGain"]?["albumGain"]?.GetValue<float>() ?? 0;
		Starred = jsonSong["starred"]?.GetValue<string>() != null;
		
		CoverArtUrl = Utils.HttpGetUrl(
			"getCoverArt",
			new[]
			{
				$"id={CoverArtId}",
				$"size={64}"
			}
		);
	}
}