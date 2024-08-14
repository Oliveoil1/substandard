using SubstandardLib.Metadata;

namespace SubstandardLib;

public struct SearchResult
{
	public List<Song> Songs = new List<Song>();
	public List<Album> Albums = new List<Album>();
	public List<Artist> Artists = new List<Artist>();
	
	public SearchResult() { }
}