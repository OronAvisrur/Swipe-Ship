using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int x;
    private int y;
    private bool _pathBlock = false;
    private Vector3 startPosition, endPosition;
    private float swipeDuration = 0.2f;
    private enum Direction { Up, Down, Left, Right };
    private Animator _anim;
    
    [Header("Audio Component")]
    [SerializeField] private AudioClip _explosionAudioClip;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource is null) Debug.LogError("Audio Source Not Found!");

        _anim = GetComponentInChildren<Animator>();
        _audioSource.clip = _explosionAudioClip;
    }

    public int X { get { return x; } set { x = value; } }
    public int Y { get { return y; } set { y = value; } }

    public bool PathBlock
    {
        get { return _pathBlock; }
        set 
        {
            _pathBlock = value;
            switch (value)
            {
                case true:
                    transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.green;
                    break;
                case false:
                    transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                    break;
            } 
        }
    }

    public IEnumerator Swipe(int direction, int rows, int columns)
    {
        //How much time passed since we started the Swipe  
        float functionInternalTime = 0.0f;
        startPosition = transform.position;

        if (direction == (int)Direction.Right)
        {
            x = x == columns - 1 ? 0 : ++x; 
            endPosition = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        }
        else if (direction == (int)Direction.Left) 
        {
            x = x == 0 ? columns - 1 : --x;
            endPosition = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        }
        else if (direction == (int)Direction.Up) 
        {
            y = y == rows - 1 ? 0 : ++y;
            endPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        }
        else if (direction == (int)Direction.Down)
        {
            y = y == 0 ? rows - 1 : --y;
            endPosition = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }

        while (functionInternalTime < swipeDuration)
        {
            functionInternalTime += Time.deltaTime;
            transform.position = Vector2.Lerp(startPosition, endPosition, functionInternalTime / swipeDuration);
            yield return null;
        }
    }

    public void ExploadeBlock()
    {
        if(PlayerPrefs.GetInt("SoundState", 1) == 1 ? true : false) _audioSource.Play();
        _anim.SetTrigger("Explosion");
    }
}
