using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SubstandardLib.Subsonic;
using GitCredentialManager;

namespace SubstandardApp.Models;

public partial class ServerInfoModel : ObservableObject
{
	[ObservableProperty] private string _url = "";
	[ObservableProperty] private string _username = "";
	[ObservableProperty] private string _password = "";

	private readonly ICredentialStore _credentialStore = CredentialManager.Create("substandard");

	public void LoadCredentials(SettingsModel settingsModel)
	{
		settingsModel.LoadSettings();
		Url = settingsModel.ServerUrl;
		Username = settingsModel.ServerUsername;

		ICredential cred = _credentialStore.Get(Url, Username);
		
		if(cred != null)
			Password = cred.Password;
	}

	public void SaveCredentials()
	{
		_credentialStore.AddOrUpdate(Url, Username, Password);
	}

	public ServerInfo GetServerInfo()
	{
		return new ServerInfo(Url, Username, Password);
	}
}