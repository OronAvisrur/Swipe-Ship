using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text _ratioText;
    [SerializeField] private GameObject _soundButton;
    [SerializeField] private Sprite _soundOn;
    [SerializeField] private Sprite _soundOff;

    [Header("Game Manager Component")]
    [SerializeField]  private GameManager _gameManager;

    private SoundSystem soundSystem;

    void Start()
    {
        if (_gameManager is null) Debug.LogError("Game Manager is NULL");

        soundSystem = GameObject.Find("Background_Music").GetComponent<SoundSystem>();
        if (soundSystem is null) Debug.LogError("Sound System is NULL");

        if(!SoundSystem.IsPlaying && PlayerPrefs.GetInt("SoundState", 1) == 1 ? true : false) soundSystem.Play(true);

        float ratio = (float)PlayerPrefs.GetInt("Wins", 0) / (float)PlayerPrefs.GetInt("Losses", 0);
        _ratioText.text = string.Format("{0:N2}", ratio);

        _soundButton.GetComponent<Image>().sprite = PlayerPrefs.GetInt("SoundState", 1) == 1 ? _soundOn : _soundOff;
    }

    public void SoundButtonClick()
    {
        soundSystem.Play(PlayerPrefs.GetInt("SoundState", 1) == 1 ? false : true);
        SetSoundButtonSprite(_gameManager.ChangeSoundState());
    }

    public void SetSoundButtonSprite(bool state)
    {
        if(state) _soundButton.GetComponent<Image>().sprite = _soundOn;
        else _soundButton.GetComponent<Image>().sprite = _soundOff;
    }
}
