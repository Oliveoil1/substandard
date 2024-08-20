using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using DynamicData;
using SubstandardApp.Models;
using SubstandardApp.ViewModels;

namespace SubstandardApp.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	private void PlaylistTreeView_OnDoubleTapped(object? sender, TappedEventArgs e)
	{
		if (DataContext is MainWindowViewModel viewModel && PlaylistTreeView.SelectedItem is PlaylistTreeNode node)
		{
			if (node.AttachedPlaylist != null)
				viewModel.LoadPlaylist(node.AttachedPlaylist);
			else if (node.ArtistId != null)
				viewModel.ShowArtistGrid(node.ArtistId);
		}
	}

	private void QueueBox_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		Point mousePos = e.GetPosition(QueueBox);
		var hit = QueueBox.InputHitTest(mousePos);

		while (hit != null && !(hit is StackPanel))
		{
			hit = (hit as Visual)?.GetVisualParent() as IInputElement;
		}

		if (hit is StackPanel { DataContext: SongModel song} && song != QueueBox.SelectedItem)
		{
			int destinationIndex = QueueBox.Items.IndexOf(song);
			int originIndex = QueueBox.SelectedIndex;
			
			if (DataContext is MainWindowViewModel viewModel)
			{
				viewModel.MoveQueueSong(originIndex, destinationIndex);
			}
		}
	}

	private void Seekbar_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
	{
		if (DataContext is MainWindowViewModel viewModel && viewModel.SeekingManually)
		{
			viewModel.SeekTo((float)e.NewValue);
		}
	}
}