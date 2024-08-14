using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace SubstandardLib.Subsonic;

public static class Utils
{
	public static ServerInfo ServerInfo;
	
	private const string ApiVersion = "1.16.1";
	private const string ClientId = "Substandard";
	private const string ApiFormat = "json";
	// private static int _seed = new Random().Next();

	public static void SetServerInfo(ServerInfo serverInfo)
	{
		ServerInfo = serverInfo;
	}
	
	public static (string, string) GetAuthSaltToken(string password)
	{
		string salt = new string(
			Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 6)
				.Select(s => s[new Random().Next(s.Length)])
				.ToArray()
			);
		string token;

		//create md5 hash of password + salt for authentication
		byte[] inputBytes = Encoding.UTF8.GetBytes(password + salt);
		using (MD5 md5 = MD5.Create())
		{
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			StringBuilder sb = new StringBuilder();
			foreach (byte b in hashBytes)
			{
				sb.Append(b.ToString("x2"));
			}

			token = sb.ToString();
		}

		return (salt, token);
	}

	public static string HttpGetUrl(string request, string[]? additionalParameters = null)
	{
		var (salt, token) = GetAuthSaltToken(ServerInfo.Password);

		string[] baseParameters =
		{
			$"c={ClientId}",
			$"v={ApiVersion}",
			$"f={ApiFormat}",
			$"u={ServerInfo.Username}",
			$"s={salt}",
			$"t={token}"
			// new KeyValuePair<string, string>("v", ApiVersion),
			// new KeyValuePair<string, string>("f", ApiFormat),
			// new KeyValuePair<string, string>("u", serverInfo.Username),
			// new KeyValuePair<string, string>("s", salt),
			// new KeyValuePair<string, string>("t", token)
		};

		string[] parameters = baseParameters;
		
		if (additionalParameters != null)
			parameters = baseParameters.Concat(additionalParameters).ToArray();
		//
		// FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
		//
		// HttpResponseMessage response = await client.PostAsync(request, content);
		// return await response.Content.ReadAsStringAsync();

		string url = $"{ServerInfo.Url}/rest/{request}?{string.Join("&", parameters)}";

		// Console.WriteLine($"HTTP URL: {url}");
		
		return url;
	}
	
	public static async Task<string> GetServerResponse(string request, string[]? additionalParameters = null)
	{
		HttpClient client = new HttpClient();
		string url = HttpGetUrl(request, additionalParameters);
		try
		{
			HttpResponseMessage response = await client.GetAsync(url);
			string responseString = await response.Content.ReadAsStringAsync();
			return responseString;
		}
		catch (Exception e)
		{
			return e.Message;
		}
	}
	
	public static JsonNode? TryParseSubsonic(string json)
	{
		JsonNode? jsonResponse;
		try
		{
			jsonResponse = JsonNode.Parse(json);
		}
		catch (Exception e)
		{
			jsonResponse = null;
			Console.WriteLine(e);
		}

		if (jsonResponse == null)
		{
			return null;
		}
		if (jsonResponse["subsonic-response"]!["error"] != null)
		{
			return null;
		}
		return jsonResponse;
	}
}