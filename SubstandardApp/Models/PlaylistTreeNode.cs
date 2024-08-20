using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SubstandardApp.Models;

public partial class PlaylistTreeNode : ObservableObject
{
	[ObservableProperty] private ObservableCollection<PlaylistTreeNode>? _subNodes;
	[ObservableProperty] private string _title;
	public string? AttachedPlaylist;
	public string? ArtistId;

	public PlaylistTreeNode(string title, string? playlistId = null, string? artistId = null)
	{
		Title = title;

		if (playlistId != null)
		{
			AttachedPlaylist = playlistId;
		}
		if(artistId != null)
		{
			ArtistId = artistId;
		}
	}
}