using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SubstandardLib;
using SubstandardLib.Subsonic;

namespace SubstandardApp.Models;

public partial class SettingsModel : ObservableObject
{
	[ObservableProperty] private string _serverUrl = string.Empty;
	[ObservableProperty] private string _serverUsername = string.Empty;
	[ObservableProperty] private string _serverPassword = string.Empty;
	
	private readonly string _dataFolderPath;
	private readonly string _settingsFilePath;
	private readonly ServerInfoModel _serverInfoModel = new();
	private readonly Client _subsonicClient;
		
	public SettingsModel(Client subsonicClient)
	{
		_subsonicClient = subsonicClient;
		_dataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\substandard";
		_settingsFilePath = _dataFolderPath + @"\settings.json";
		
		CheckDataFolder();
	}

	private void CheckDataFolder()
	{
		if (!Directory.Exists(_dataFolderPath))
			Directory.CreateDirectory(_dataFolderPath);

		if (!File.Exists(_settingsFilePath))
			SaveSettings();
	}

	public void LoadSettings()
	{
		string jsonString = File.ReadAllText(_settingsFilePath);
		SettingsFile settings = JsonSerializer.Deserialize<SettingsFile>(jsonString) ?? new SettingsFile();

		ServerUrl = settings.ServerUrl;
		ServerUsername = settings.ServerUsername;
		
		_serverInfoModel.LoadCredentials(ServerUrl, ServerUsername);
		ServerPassword = _serverInfoModel.Password;
		
		_subsonicClient.Login(GetServerInfo());
	}

	public void SaveSettings()
	{
		SettingsFile settings = new()
		{
			ServerUrl = ServerUrl,
			ServerUsername = ServerUsername
		};

		string jsonString = JsonSerializer.Serialize(settings);
		File.WriteAllText(_settingsFilePath, jsonString);

		_serverInfoModel.Url = ServerUrl;
		_serverInfoModel.Username = ServerUsername;
		_serverInfoModel.Password = ServerPassword;
		_serverInfoModel.SaveCredentials();
		
		LoadSettings();
	}

	public ServerInfo GetServerInfo()
	{
		return _serverInfoModel.GetServerInfo();
	}
}

public class SettingsFile
{
	public string ServerUrl { get; set; }
	public string ServerUsername { get; set; }
}