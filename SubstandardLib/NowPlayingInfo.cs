using SubstandardLib.Metadata;

namespace SubstandardLib;

public struct NowPlayingInfo()
{
	public Song PlayingSong = new();
	public Album PlayingAlbum = new();
	public Artist PlayingArtist = new();
	
	public string PlayingArtUrl = string.Empty;

	public bool IsPaused = true;
	public bool AtSongEnd = false;

	public float PlaybackSeconds = 0;
	public float PlaybackMaxSeconds = 0;
	
	public DateTime StartedPlaying = DateTime.Now;
}