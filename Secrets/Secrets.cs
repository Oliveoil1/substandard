using Microsoft.Extensions.Configuration;

namespace Secrets;

public static class Secrets
{
	public static string LastFmKey = "";
	public static string DiscordKey = "";

	public static void InitKeys(IConfigurationRoot config)
	{
		LastFmKey = config["LastFMKey"] ?? "";
		DiscordKey = config["DiscordKey"] ?? "";
	}
	
	// public static void Secrets()
	// {
	// 	private IConfiguration config = new ConfigurationBuilder()
	// 		.AddUserSecrets<Secrets>()
	// 		.Build();
	// }
}