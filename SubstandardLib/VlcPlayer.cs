using LibVLCSharp.Shared;

namespace SubstandardLib;

public class VlcPlayer
{
	private readonly LibVLC _vlc;
	private readonly MediaPlayer _musicPlayer;

	public VlcPlayer()
	{
		_vlc = new LibVLC();
		_musicPlayer = new MediaPlayer(_vlc);
	}

	public void PlayUrl(string url)
	{
		Media media = new Media(_vlc, new Uri(url));
		_musicPlayer.Play(media);
	}

	public void TogglePause()
	{
		_musicPlayer.Pause();
	}

	public void SetVolume(int volume)
	{
		_musicPlayer.Volume = volume;
	}

	public void SeekTo(float seconds)
	{
		_musicPlayer.Time = (long)(seconds * 1000);
	}

	public bool GetPaused()
	{
		return !_musicPlayer.IsPlaying;
	}

	public float GetPlaybackMaxSeconds()
	{
		return _musicPlayer.Length / 1000f;
	}

	public float GetPlaybackSeconds()
	{
		return _musicPlayer.Position * _musicPlayer.Length / 1000f;
	}
	
}