using System.Text.Json.Nodes;

namespace SubstandardLib.Metadata;

public struct Playlist
{
	public string Id;
	public string Title;
	public string Owner;
	
	public int SongCount;
	public int Duration;

	public List<Song> Songs;

	public Playlist()
	{
		Id = "null";
		Title = "Null Playlist";
		Owner = "null";

		SongCount = 0;
		Duration = 0;
		
		Songs = new List<Song>();
	}

	public Playlist(JsonNode playlistJson)
	{
		Id = playlistJson["id"]?.GetValue<string>() ?? "null";
		Title = playlistJson["name"]?.GetValue<string>() ?? "Null Playlist";
		Owner = playlistJson["owner"]?.GetValue<string>() ?? "null";

		SongCount = playlistJson["songCount"]?.GetValue<int>() ?? 0;
		Duration = playlistJson["duration"]?.GetValue<int>() ?? 0;
		
		Songs = new List<Song>();
		foreach (JsonNode? songJson in playlistJson["entry"]?.AsArray() ?? new JsonArray())
		{
			Song song = new Song(songJson);
			Songs.Add(song);
		}
	}
}