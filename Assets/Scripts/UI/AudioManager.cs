using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    AudioClip _loadedAudioClip;

    void Awake() {
        //ChangeVolume(PlayerPrefs.GetFloat("BackgroundMusicVolume", 1));

        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        LoadSFXClips();
        LoadMusicClips();
    }

    void LoadSFXClips() {
       sfxClips["gameOver"] = Resources.Load<AudioClip>("SFX/GameOver");
       sfxClips["victory"] = Resources.Load<AudioClip>("SFX/Victory");
    }

    void LoadMusicClips() {
        musicClips["menu"] = Resources.Load<AudioClip>("Music/menu");
    }

    public void PlaySFX(string clipName) {
        if (sfxClips.ContainsKey(clipName)) {
            sfxSource.clip = sfxClips[clipName];
            sfxSource.Play();
        } else Debug.LogWarning("El AudioClip " + clipName + " no se encontr� en el diccionario de sfxClips.");
    }

    public void PlayMusic(string clipName) {
        if (musicClips.ContainsKey(clipName)) {
            musicSource.clip = musicClips[clipName];
            musicSource.Play();
        } else Debug.LogWarning("El AudioClip " + clipName + " no se encontr� en el diccionario de musicClips.");
    }

    public void PlaySong() {
        if (PlayerPrefs.GetString("song.path") == null) Debug.LogError("Could not find song path");
        else {
            string songPath = Path.Combine("Music", "Songs", PlayerPrefs.GetString("song.path"));
            _loadedAudioClip = Resources.Load<AudioClip>(songPath);
            musicSource.clip = _loadedAudioClip;
            musicSource.Play();
        }
    }

    public void UnloadSong() {
        if (_loadedAudioClip != null) {
            Resources.UnloadAsset(_loadedAudioClip);
            _loadedAudioClip = null;
        } else Debug.LogWarning("No song loaded to unload.");
    }

    public void ChangeVolume(float value) {
        sfxSource.volume = value;
        musicSource.volume = value;
    }
}