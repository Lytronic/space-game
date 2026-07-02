using Godot;
using System;

public partial class SoundManager : Node2D
{

	string[] tracks; // Array for track filepaths
	string[] sounds; // Array for sound filepaths

	private AudioStreamPlayer _musicPlayer;
	private AudioStreamPlayer _soundPlayer;

	public override void _Ready()
	{
		_musicPlayer = GetChild<AudioStreamPlayer>(0);
		_soundPlayer = GetChild<AudioStreamPlayer>(1);

		// Add music here by adding it to the next index of the array "tracks" with the string: "res://sfx/music/YOUR_TRACK_HERE" and then updating the array size
		tracks = new string[8];
		tracks[0] = "res://sfx/music/End Fight.mp3";
		tracks[1] = "res://sfx/music/Fight Final.mp3";
		tracks[2] = "res://sfx/music/Fight.mp3";
		tracks[3] = "res://sfx/music/Intro.mp3";
		tracks[4] = "res://sfx/music/Outro.mp3";
		tracks[5] = "res://sfx/music/Start Fight.mp3";
		tracks[6] = "res://sfx/music/Workspace Final.mp3";
		tracks[7] = "res://sfx/music/Workspace.mp3";
		//PlayTrack(1); // For Test Purposes
		
		// Add sounds here by adding it to the next index of the array "sounds" with the string: "res://sfx/..." and then updating the array size
		sounds = new string[8];
		sounds[0] = "res://sfx/gui/menu/click.wav";
		sounds[1] = "res://sfx/gui/builder/assemble.mp3";
		sounds[2] = "res://sfx/gui/builder/bubble_pop.mp3";
		sounds[3] = "res://sfx/gui/builder/shift.mp3";
	}

	public override void _Process(double delta)
	{
	}
	public void PlaySound(int sound) { // function to play a sound

		_soundPlayer.Stream = GD.Load<AudioStream>(sounds[sound]);
			
		_soundPlayer.Play();
	}

	public void PlayTrack(int track) { // function to play the music

		_musicPlayer.Stream = GD.Load<AudioStream>(tracks[track]);
			
		_musicPlayer.Play();
	}
}
