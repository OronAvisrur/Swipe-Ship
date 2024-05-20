using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _wonScreenPanel;
    [SerializeField] private GameObject _lostScreenPanel;
    [SerializeField] private GameObject _pasuePanel;

    [Header("Audio Components")]
    [SerializeField] private AudioClip _gameOverAudioClip;
    [SerializeField] private GameObject _soundButton;
    [SerializeField] private Sprite _soundOn;
    [SerializeField] private Sprite _soundOff;

    [Header("Other Components")]
    [SerializeField]  private GameManager _gameManager;
    [SerializeField] private TMP_Text _movesConuterText;

   
    private AudioSource _audioSource;
    private Board _board;
    private SoundSystem soundSystem;

    void Start()
    {
        _wonScreenPanel.gameObject.SetActive(false);
        _lostScreenPanel.gameObject.SetActive(false);
        _pasuePanel.gameObject.SetActive(false);

        if (_gameManager is null) Debug.LogError("Game Manager is NULL");

        _board = GameObject.Find("Board").GetComponent<Board>();
        if (_board is null) Debug.LogError("UI Manager is NULL");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource is null) Debug.LogError("Audio Source Not Found!");

        soundSystem = GameObject.Find("Background_Music").GetComponent<SoundSystem>();
        if (soundSystem is null) Debug.LogError("Sound System is NULL");

        _audioSource.clip = _gameOverAudioClip;

        _soundButton.GetComponent<Image>().sprite = PlayerPrefs.GetInt("SoundState", 1) == 1 ? _soundOn : _soundOff;
        
        UpdateRemainingMoves();
    }

    public IEnumerator GameOverSequence(bool winning)
    {
        if(PlayerPrefs.GetInt("SoundState", 1) == 1 ? true : false) _audioSource.Play();
        
        if(winning)
        {
            _wonScreenPanel.gameObject.SetActive(true);

            int coins = PlayerPrefs.GetInt("Coins", 0);
            coins += 100;
            PlayerPrefs.SetInt("Coins", coins);
        }
        else _lostScreenPanel.gameObject.SetActive(true);
       
        yield return new WaitForSeconds(5f);

        SavingSystem.DeleteGame();
        _gameManager.LoadGame();

        if(winning) _wonScreenPanel.gameObject.SetActive(false);
        else _lostScreenPanel.gameObject.SetActive(false);
        
        _board.CanSwipe = false;
    }

    public void PauseGame() 
    {
        _pasuePanel.gameObject.SetActive(true);
        _board.CanSwipe = false;
        _board.SaveGame();
    }

    public void UnPauseGame()
    {
        _pasuePanel.gameObject.SetActive(false);
        _board.CanSwipe = true;
    }

    public void UpdateRemainingMoves()
    {
        _movesConuterText.text = "Moves: " + _board.MoveCounter + " / 6";
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

    public IEnumerator CountDown()
    {
        for(int i = 5; i > 0; i--)
        {
            _movesConuterText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        UpdateRemainingMoves();
    }
}
