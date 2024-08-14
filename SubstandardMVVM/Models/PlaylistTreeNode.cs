using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SubstandardMVVM.Models;

public partial class PlaylistTreeNode : ObservableObject
{
	[ObservableProperty] private ObservableCollection<PlaylistTreeNode>? _subNodes;
	[ObservableProperty] private string _title;
	public string? AttachedPlaylist;

	public PlaylistTreeNode(string title, string? playlistId = null)
	{
		Title = title;

		if (playlistId != null)
		{
			AttachedPlaylist = playlistId;
		}
	}
}