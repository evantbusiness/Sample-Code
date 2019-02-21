using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicChannel;
    [SerializeField] private AudioSource soundChannel;

    [SerializeField] private List<AudioClip> musicBank;
    [SerializeField] private List<AudioClip> soundBank;

    private Dictionary<string, AudioClip> musicLibrary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();

    public static SoundManager Instance = null;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		else if (Instance != this)
		{
			Destroy(gameObject);
		}

        foreach (AudioClip clip in musicBank)
            musicLibrary.Add(clip.name, clip);

        foreach (AudioClip clip in soundBank)
            soundLibrary.Add(clip.name, clip);

        DontDestroyOnLoad(gameObject);
	}

    public void PlayMusic(string songName)
    {
        //Prevent music restarts (Will not change song if current song is the same)
        if(musicChannel.clip != musicLibrary[songName])
        {
            musicChannel.clip = musicLibrary[songName];
            musicChannel.Play();
        }
    }

    public void PlaySound(string clipName)
	{
		soundChannel.clip = soundLibrary[clipName];
		soundChannel.Play();
	}
}