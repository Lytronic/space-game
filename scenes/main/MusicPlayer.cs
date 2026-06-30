using Godot;
using System;

public partial class MusicPlayer : AudioStreamPlayer
{
	string[] tracks; // Array for track filepaths
	
	public override void _Ready()
	{
		// Add music here by adding it to the next index of the array "tracks" with the string: "res://sfx/music/YOUR_TRACK_HERE" and then updating the array size
		tracks = new string[2];
		tracks[0] = "res://sfx/music/Fight.mp3";
		tracks[1] = "res://sfx/music/Workspace.mp3";
		PlayTrack(1);
		
	}

	public override void _Process(double delta)
	{
	}
	
	public void PlayTrack(int track) { // function to play the music

		Stream = GD.Load<AudioStream>(tracks[track]);
			
		Play();
	}
}
