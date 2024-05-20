using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour 
{
    private static SoundSystem _instance;
    private static bool _isPlaying = false;

    [SerializeField] private AudioClip _backgroundAudioClip;
    private AudioSource _audioSource;
 
    public static SoundSystem Instance { get { return _instance; } }
    public static bool IsPlaying{ get { return _isPlaying; } set { _isPlaying = value; } }
 
    void Awake() 
    {
        if(_instance is null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource is null) Debug.LogError("Audio Source Not Found!");

        _audioSource.clip = _backgroundAudioClip;
        _audioSource.loop = true;
    }
 
    public void Play(bool state)
    {
        if(state) 
        {
            _audioSource.Play();
            _isPlaying = true;
        }
        else
        {
            _audioSource.Stop();
            _isPlaying = false;
        }
    }
}
