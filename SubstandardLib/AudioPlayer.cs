using NAudio;
using NAudio.Utils;
using NAudio.Wave;

namespace SubstandardLib;

public class AudioPlayer
{
	public enum ReplayGainMode
	{
		None,
		Track,
		Album,
		Auto
	}
	private readonly WaveOutEvent _outputDevice = new WaveOutEvent();
	private AudioFileReader? _audioFile;

	public float TrackGain = 0.0f;
	public float AlbumGain = 0.0f;
	public ReplayGainMode GainMode = ReplayGainMode.None;
	
	public AudioPlayer()
	{
		// // _vlc = new LibVLC();
		// _musicPlayer = new WasapiOut();
	}

	public void PlayUrl(string url)
	{
		_outputDevice.Stop();
		try
		{
			_audioFile = new AudioFileReader(url);

			float gainDb = GainMode switch
			{
				ReplayGainMode.Track => TrackGain,
				ReplayGainMode.Album => AlbumGain,
				_ => 0.0f
			};

			_audioFile.Volume = MathF.Pow(10, gainDb / 20);
			
			_outputDevice.Init(_audioFile);
			_outputDevice.Play();
		}
		catch
		{
			Console.WriteLine("Unable to play!");
		}
	}

	public void TogglePause()
	{
		switch (_outputDevice.PlaybackState)
		{
			case PlaybackState.Playing:
				_outputDevice.Pause();
				break;
			case PlaybackState.Paused:
				_outputDevice.Play();
				break;
			case PlaybackState.Stopped:
				break;
		}
	}

	public void SetVolume(float volume)
	{
		_outputDevice.Volume = volume;
	}

	public void SeekTo(float seconds)
	{
		// _musicPlayer.Time = (long)(seconds * 1000);
		if (_audioFile != null)
			_audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
	}

	public bool GetPaused()
	{
		return _outputDevice is { PlaybackState: PlaybackState.Paused };
	}
	
	public bool GetStopped()
	{
		return _outputDevice is { PlaybackState: PlaybackState.Stopped };
	}

	public float GetPlaybackMaxSeconds()
	{
		// return _musicPlayer. / 1000f;
		float seconds = -1;
		if (_audioFile != null)
			seconds = (float)_audioFile.TotalTime.TotalSeconds;
		return seconds;
	}

	public float GetPlaybackSeconds()
	{
		float seconds = 0;
		// return _musicPlayer.Position * _musicPlayer.Length / 1000f;
		if (_audioFile != null)
			seconds = (float)_audioFile.CurrentTime.TotalSeconds;
		return seconds;
	}
	
}