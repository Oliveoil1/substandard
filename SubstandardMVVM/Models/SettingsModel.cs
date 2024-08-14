using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SubstandardMVVM.Models;

public partial class SettingsModel : ObservableObject
{
	[ObservableProperty] private string _serverUrl = string.Empty;
	[ObservableProperty] private string _serverUsername = string.Empty;
	
	private readonly string _dataFolderPath;
	private readonly string _settingsFilePath;
		
	public SettingsModel()
	{
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
	}
}

public class SettingsFile
{
	public string ServerUrl { get; set; }
	public string ServerUsername { get; set; }
}