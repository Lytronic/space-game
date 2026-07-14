using Godot;
using System;
using System.IO;
using Microgravity.util;

public partial class SoundManager : Node2D
{

/// <summary> EXPLANATION FOR ADDING SOUNDS TO STUFF:
///	NON-2D: Create a private variable called _soundManager in whatever script you want to make the sound. In the script's _Ready write "_soundManager = GetNode("/root/SoundManager");"
/// and then call the sound you want to use by finding out the index of it in the array below and write this line of code: "_soundManager.Call("PlaySound", 0, 1);" with 0 being the index of the soundManager and 1 being the sound being played </summary>

	string[] tracks; // Array for track filepaths
	string[] sounds; // Array for sound filepaths

	public float MusicVolume;
	private AudioStreamPlayer _musicPlayer;
	public AudioStreamPlayer[] SoundPlayerArray;

	public bool FlightSound;

	private bool _playingLoop;

	public float masterVolume;

	private float _musicVolume;

	public override void _Ready()
	{
		_musicPlayer = GetChild<AudioStreamPlayer>(0);
		SoundPlayerArray = new AudioStreamPlayer[6];
		for (int i = 0; i < SoundPlayerArray.Length; i++)
		{
			SoundPlayerArray[i] = GetChild<AudioStreamPlayer>(i + 1);
		}
		//GD.Print(SoundPlayerArray);
		// Add music here by adding it to the next index of the array "tracks" with the string: "res://sfx/music/YOUR_TRACK_HERE" and then updating the array size
		tracks = new string[6];
		tracks[0] = "res://sfx/music/2 End Fight.mp3";
		tracks[1] = "res://sfx/music/2 Fight Final.mp3";
		tracks[2] = "res://sfx/music/2 Intro.mp3";
		tracks[3] = "res://sfx/music/2 Outro.mp3";
		tracks[4] = "res://sfx/music/2 Start Fight.mp3";
		tracks[5] = "res://sfx/music/2 Workspace Final.mp3";
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

		SettingsModel.Instance.SettingChanged += ChangedCallback;
		for(int i = 0; i < SoundPlayerArray.Length; i++)
		{
			SoundPlayerArray[i].VolumeLinear = ((SettingsEntry.Float)SettingsModel.Instance.Settings["audio.master_volume"]).Value / 100;
		}
		_musicVolume = ((SettingsEntry.Float)SettingsModel.Instance.Settings["audio.music_volume"]).Value / 100;
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
	// function to play the intro
	public async void PlayIntro()
	{
		_musicVolume = ((SettingsEntry.Float)SettingsModel.Instance.Settings["audio.music_volume"]).Value / 100;
		_playingLoop = true;
		PlayTrack(5);
		while (_playingLoop == true)
		{
			await ToSignal(_musicPlayer, AudioStreamPlayer.SignalName.Finished);
			PlayTrack(5);
		}
		//FadeOutMusic(1.0f);
	}

	// function to play the battle music

	public void FadeOut(int SPindex, float FadeDuration)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(SoundPlayerArray[SPindex], "volume_db", -80.0f, FadeDuration);
		tween.TweenCallback(Callable.From(() => SoundPlayerArray[SPindex].Stop()));
	}
	/*public void FadeOutMusic(float FadeDuration)
	{
		GD.Print("what");
		Tween tween = CreateTween();
		tween.TweenProperty(_musicPlayer, "volume_db", -80.0f, FadeDuration);
		tween.TweenCallback(Callable.From(() => _musicPlayer.Stop()));
	}*/

	public void DisableLoop()
	{
		_playingLoop = false;
	}

	public void StopMusic()
	{
		_musicPlayer.Stop();
	}

	public async void Fight()
	{
		//await ToSignal(_musicPlayer, AudioStreamPlayer.SignalName.Finished);
		StopMusic();
		PlayTrack(1);
		_playingLoop = true;
		while (_playingLoop == true)
		{
			await ToSignal(_musicPlayer, AudioStreamPlayer.SignalName.Finished);
			PlayTrack(1);
		}
	}
	public async void Menu()
	{
		//await ToSignal(_musicPlayer, AudioStreamPlayer.SignalName.Finished);
		StopMusic();
		PlayTrack(5);
		_playingLoop = true;
		while (_playingLoop == true)
		{
			await ToSignal(_musicPlayer, AudioStreamPlayer.SignalName.Finished);
			PlayTrack(5);
		}
	}

	public async void StartFlightNoise()
	{
		SoundPlayerArray[3].VolumeLinear = 0;
		//GD.Print("SoundPlayerArray[3].VolumeLinear");
		FlightSound = true;
		PlaySound(3, 6);
		while (FlightSound == true)
		{
			await ToSignal(SoundPlayerArray[3], AudioStreamPlayer.SignalName.Finished);
			PlaySound(3, 6);
		}
		SoundPlayerArray[3].Stop();
	}

	public void ChangeFlightNoise(float volume)
	{
		//GD.Print("Master Volume" + masterVolume);
		SoundPlayerArray[3].VolumeLinear = masterVolume / 2500 * volume;
		//GD.Print("Adjusted Volume / 100 " + SoundPlayerArray[3].VolumeLinear);
	}
	
	public void EndFlightNoise()
	{
		FlightSound = false;
		SoundPlayerArray[3].Stop();
	}

	// function to change the music volume
	public void ChangeMusicVolume(float volume)
	{
		_musicPlayer.VolumeLinear = volume / 100;
		//GD.Print("Changed Music volume to " + _musicPlayer.VolumeLinear);
	}

	// function to change a specific sound players volume
	public void ChangeSoundVolume(int idx, float volume)
	{
		SoundPlayerArray[idx].VolumeLinear = volume;
	}
	public void ChangeMasterVolume(float volume)
	{
		for(int i = 0; i < SoundPlayerArray.Length; i ++)
		{
			if(i != 3)
			{
			SoundPlayerArray[i].VolumeLinear = volume / 100;
			//GD.Print("Changed Master volume to " + SoundPlayerArray[i].VolumeLinear);
			}
		}
	}

	public float GetMastervolume()
	{
		return masterVolume;
	}

	private void ChangedCallback(string key)
	{
		//GD.Print("ChangedCallback()" + key);
		switch (key)
		{
			case "audio.music_volume":
				_musicVolume = ((SettingsEntry.Float)SettingsModel.Instance.Settings["audio.music_volume"]).Value;
				//GD.Print(_musicVolume);
				ChangeMusicVolume(_musicVolume);
				break;
			case "audio.master_volume":
				masterVolume = ((SettingsEntry.Float)SettingsModel.Instance.Settings["audio.master_volume"]).Value;
				ChangeMasterVolume(masterVolume);
				//GD.Print(masterVolume);
				break;
		}
	}
	
}
