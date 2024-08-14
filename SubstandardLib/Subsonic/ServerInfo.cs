namespace SubstandardLib.Subsonic;

public struct ServerInfo(string url, string username, string password)
{
	public readonly string Url = url;
	public readonly string Username = username;
	public readonly string Password = password;
}