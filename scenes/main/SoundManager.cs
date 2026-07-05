using Godot;
using System;

public partial class SoundManager : Node2D
{

/// <summary> EXPLANATION FOR ADDING SOUNDS TO STUFF:
///	NON-2D: Create a private variable called _soundManager in whatever script you want to make the sound. In the script's _Ready write "_soundManager = GetNode("/root/SoundManager");"
/// and then call the sound you want to use by finding out the index of it in the array below and write this line of code: "_soundManager.Call("PlaySound", 0, 1);" with 0 being the index of the soundManager and 1 being the sound being played </summary>

	string[] tracks; // Array for track filepaths
	string[] sounds; // Array for sound filepaths

	private AudioStreamPlayer _musicPlayer;
	public AudioStreamPlayer[] SoundPlayerArray;

	public override void _Ready()
	{
		_musicPlayer = GetChild<AudioStreamPlayer>(0);
		SoundPlayerArray = new AudioStreamPlayer[6];
		for (int i = 1; i < SoundPlayerArray.Length; i++)
		{
			SoundPlayerArray[i - 1] = GetChild<AudioStreamPlayer>(i);
		}
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
		sounds = new string[19];
		sounds[0] = "res://sfx/gui/menu/click.wav";
		sounds[1] = "res://sfx/gui/builder/assemble.mp3";
		sounds[2] = "res://sfx/gui/builder/bubble_pop.mp3";
		sounds[3] = "res://sfx/gui/builder/shift.mp3";
		sounds[4] = "res://sfx/game/enemy/arc_distant.mp3";
		sounds[5] = "res://sfx/game/enemy/explosion_distant.mp3";
		sounds[6] = "res://sfx/game/ship/engine_loop.mp3";
		sounds[7] = "res://sfx/game/ship/explosion.wav";
		sounds[8] = "res://sfx/game/ship/ion_loop.mp3";
		sounds[9] = "res://sfx/game/ship/nuclear_loop.mp3";
		sounds[10] = "res://sfx/game/ship/shield_hit.mp3";
		sounds[11] = "res://sfx/game/weapons/arc.mp3";
		sounds[12] = "res://sfx/game/weapons/cannon.mp3";
		sounds[13] = "res://sfx/game/weapons/emp.mp3";
		sounds[14] = "res://sfx/game/weapons/laser.mp3";
		sounds[15] = "res://sfx/game/weapons/missile.mp3";
		sounds[16] = "res://sfx/game/weapons/plasma.mp3";
		sounds[17] = "res://sfx/game/weapons/rail_coil.mp3";
		sounds[18] = "res://sfx/game/weapons/torpedo.mp3";
	}

	public override void _Process(double delta)
	{
	}
	public void PlaySound(int SPindex, int sound) { // function to play a sound

		SoundPlayerArray[SPindex].Stream = GD.Load<AudioStream>(sounds[sound]);
			
		SoundPlayerArray[SPindex].Play();
	}

	public void PlayTrack(int track) { // function to play the music

		_musicPlayer.Stream = GD.Load<AudioStream>(tracks[track]);
			
		_musicPlayer.Play();
	}

	public void FadeOut(int SPindex, float FadeDuration)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(SoundPlayerArray[SPindex], "volume_db", -80.0f, FadeDuration);
		tween.TweenCallback(Callable.From(() => SoundPlayerArray[SPindex].Stop()));
	}
}
