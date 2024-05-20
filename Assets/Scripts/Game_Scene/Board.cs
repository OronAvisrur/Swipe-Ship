using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int rows = 6;
    private const int columns = 5;

    private GameObject blockClone;
    private AudioSource _audioSource;
    private GameData savedBoard;
    private UIManager _uiManager;

    private GameObject[,] board = new GameObject[columns, rows];
    List<Point> pathCoordinate = new List<Point>();
    Stack<Move> moves = new Stack<Move>();

    [Header("Game Components")]
    [SerializeField] private GameObject _blockPrefab;
    [SerializeField] private GameObject _blockContainer;
    [SerializeField] private GameObject _shipPrefab;
    [SerializeField] private Sprite[] _shipSprites;

    [Header("Audio Component")]
    [SerializeField] private AudioClip _swipeAudioClip;
    
    
    private enum Direction { Up, Down, Left, Right, Idle };
    Direction directionToMove = Direction.Idle;
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    private bool _canSwipe;

    private int shipYPosition = 0;
    private int moveCounter = 6;
    GameObject ship;
    Vector3 blockDestination;

    private bool isGameOver = false;

    public int MoveCounter { get { return moveCounter; } set { moveCounter = value; } }
    public bool CanSwipe { get { return _canSwipe; } set { _canSwipe = value; } }
    
    void Start()
    {
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager is null) Debug.LogError("UI Manager is NULL");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource is null) Debug.LogError("Audio Source Not Found!");

        _audioSource.clip = _swipeAudioClip;
        _canSwipe = true;

        initGameObjects();
        blockDestination = new Vector3(-1.5f, shipYPosition, 0);
    }

    void Update()
    {
        SwipeManager();
        MoveShip();
        if(moveCounter == 0 && !isGameOver)
        {
            StartCoroutine(GameOver());
            StartCoroutine(ExploadeBlocks());
            isGameOver = true;
        }
    }

    private void initGameObjects() 
    {
        initBlocks(); 
        savedBoard = SavingSystem.LoadGame();

        if(savedBoard is null)
        {
            shipYPosition = Random.Range(1,rows - 1);
            BuildPath(0, shipYPosition);
            StartCoroutine(MessbBoard());
        }
        else LoadSavedGame(savedBoard);

        InstantiateShip();        
    }
    
    private void InstantiateShip()
    {
        ship = Instantiate(_shipPrefab, new Vector3(-1.5f, shipYPosition, 0.0f), Quaternion.identity);
        ship.transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
        ship.transform.GetComponent<SpriteRenderer> ().sprite = _shipSprites[PlayerPrefs.GetInt("InUseShip", 0)];
    }

    private void initBlocks()
    {
        Block block;
        Vector3 posToSpawn;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                posToSpawn = new Vector3(x, y, 0);

                board[x, y] = Instantiate(_blockPrefab, posToSpawn, Quaternion.identity);
                board[x, y].transform.parent = _blockContainer.transform;

                block = board[x, y].GetComponent<Block>();

                block.X = x;
                block.Y = y;
            }
        }

        //Instantiate block clone. The Clone is used each swipe to make a portal like feel
        posToSpawn = new Vector3(-10, -10, 0);

        blockClone = Instantiate(_blockPrefab, posToSpawn, Quaternion.identity);
        blockClone.transform.parent = _blockContainer.transform;
    }

    private void BuildPath(int x, int y)
    {
        if(x != 5)
        {
            pathCoordinate.Add(new Point(x, y));
            int rndBlockYPosition = 0;

            board[x,y].GetComponent<Block>().PathBlock = true;
            switch (y)
            {
                case 1:
                    rndBlockYPosition = Random.Range(1,3);
                    break;
                case 2:
                    rndBlockYPosition = Random.Range(1,4);
                    break;
                case 3:
                    rndBlockYPosition = Random.Range(2,5);
                    break;
                case 4:
                    rndBlockYPosition = Random.Range(3,5);
                    break;
                    
            }
            BuildPath(x + 1, rndBlockYPosition);
        }
    }
    
    private void LoadSavedGame(GameData loadedGame)
    {
        shipYPosition = loadedGame.ShipYPosition;
        foreach (Point point in loadedGame.CurrentPathBlocksLocations)
        {
            board[point.X, point.Y].GetComponent<Block>().PathBlock = true;
        }

        pathCoordinate = loadedGame.OriginalPathBlocksLocations;

        moveCounter = loadedGame.Moves;
    }

    public void SaveGame()
    {
        List<Point> currentPathBlocksLocations = new List<Point>();
        for(int x = 0; x < columns; x++)
        {
            for(int y = 0; y < rows; y++)
            {
                if(board[x, y].GetComponent<Block>().PathBlock) currentPathBlocksLocations.Add(new Point(x, y));
            }
        }
        SavingSystem.SaveGame(new GameData(currentPathBlocksLocations, pathCoordinate, shipYPosition, moveCounter));
    }

    public IEnumerator MessbBoard()
    {
        int randomSet;
        int rndDirection;
        int rowOrCol = Random.Range(0, 2);

        _canSwipe = false;

        StartCoroutine(_uiManager.CountDown());
        yield return new WaitForSeconds(5.0f);

        for(int i = 0; i < 6; i++)
        {
            if(rowOrCol == 0)
            {
                randomSet = Random.Range(1, rows - 1);
                rndDirection = Random.Range((int)Direction.Left, (int)Direction.Right + 1);
                switch (rndDirection)
                {
                    case (int)Direction.Right:
                        directionToMove = Direction.Right;
                        break;
                    case (int)Direction.Left:
                        directionToMove = Direction.Left;
                        break;
                }

                MoveBlocksOnScreen(0, randomSet);
            }
            else
            {
                randomSet = Random.Range(0, columns);
                rndDirection = Random.Range((int)Direction.Up, (int)Direction.Down + 1);
                switch (rndDirection)
                {
                    case (int)Direction.Up:
                        directionToMove = Direction.Up;
                        break;
                    case (int)Direction.Down:
                        directionToMove = Direction.Down;
                        break;
                }

                MoveBlocksOnScreen(randomSet, 0);
            }

            rowOrCol = Random.Range(0, 2);
            yield return new WaitForSeconds(0.4f);
        }
        
        _canSwipe = true;
    }
    
    public void SwipeManager()
    {
        if (_canSwipe == true)
        {
            int y = 0;
            int x = 0;
            directionToMove = Direction.Idle;
            List<Block> blocksToMove = new List<Block>();

            if (Input.GetMouseButtonDown(0))
            {
                //Get 2D position of the first click
                firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                //Get 2D position of the second click
                secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //Create Vector from the first and second clicks positions
                currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                currentSwipe.Normalize();

                y = CalculateRowLocation(firstPressPos);
                x = CalculateColLocation(firstPressPos);

                if (y != -1 && x != -1)
                {
                    moveCounter--;
                    _uiManager.UpdateRemainingMoves();
                    
                    if (RightSwipe(currentSwipe)) directionToMove = Direction.Right;
                    else if (LeftSwipe(currentSwipe)) directionToMove = Direction.Left;
                    else if (UpSwipe(currentSwipe)) directionToMove = Direction.Up;
                    else if (DownSwipe(currentSwipe)) directionToMove = Direction.Down;

                    MoveBlocksOnScreen(x, y);
                    moves.Push(new Move(x, y, (int)directionToMove));
                }
            }
        }
    }

    private void MoveBlocksOnScreen(int x, int y)
    {
        List<Block> blocksToMove = new List<Block>();

        if(PlayerPrefs.GetInt("SoundState", 1) == 1 ? true : false) _audioSource.Play();
        
        if (directionToMove != Direction.Idle)
        {
            blocksToMove = SelectBlocksForSwipe(x, y, directionToMove);

            StartCoroutine(MoveBlocksOnBoard(x, y, directionToMove));

            foreach(var block in blocksToMove)
            {
                StartCoroutine(block.Swipe((int)directionToMove, rows, columns));   
            }    
            StartCoroutine(SwipeCloneCoroutine());
        }
    }

    private List<Block> SelectBlocksForSwipe(int x, int y, Direction direction)
    {
        int i = 0;
        List<Block> blocksToMove = new List<Block>();
        if(direction == Direction.Right || direction == Direction.Left)
        {
            for(i = 0; i < board.GetLength(0); i++)
            {
                blocksToMove.Add(board[i, y].GetComponent<Block>());
                
            }
            SetCloneProperties((direction == Direction.Right ? blocksToMove[i- 1] : blocksToMove[0]), (direction == Direction.Right ? columns - 1 : 0), y);
        }
        else if(direction == Direction.Up || direction == Direction.Down)
        {
            for(i = 0; i < board.GetLength(1); i++)
            {
                blocksToMove.Add(board[x, i].GetComponent<Block>());
                
            }
            SetCloneProperties((direction == Direction.Up ? blocksToMove[i- 1] : blocksToMove[0]), x, (direction == Direction.Up ? rows - 1 : 0));
        }

        return blocksToMove;
    }

    private int CalculateRowLocation(Vector2 pos)
    {
        int res = -1;

        if (pos.y >= 480 && pos.y <= 560) res = 0;
        else if (pos.y >= 635 && pos.y < 705) res = 1;
        else if (pos.y >= 785 && pos.y < 850) res = 2;
        else if (pos.y >= 910 && pos.y < 970) res = 3;
        else if (pos.y >= 1060 && pos.y < 1115) res = 4;
        else if (pos.y >= 1180 && pos.y < 1270) res = 5;

        return res;
    }

    private int CalculateColLocation(Vector2 pos)
    {
        int res = -1;

        if (pos.x >= 222 && pos.x <= 286) res = 0;
        else if (pos.x >= 380 && pos.x < 430) res = 1;
        else if (pos.x >= 525 && pos.x < 592) res = 2;
        else if (pos.x >= 665 && pos.x < 740) res = 3;
        else if (pos.x >= 815 && pos.x < 885) res = 4;

        return res;
    }

    private bool LeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && (swipe.y > -0.5f && swipe.y < 0.5f);
    }

    private bool RightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && (swipe.y > -0.5f && swipe.y < 0.5f);
    }

    private bool UpSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && (swipe.x > -300 && swipe.x < 300);
    }

    private bool DownSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && (swipe.x > -300 && swipe.x < 300);
    }

    public IEnumerator SwipeCloneCoroutine()
    {
        blockClone.SetActive(true);
        yield return blockClone.GetComponent<Block>().Swipe((int)directionToMove, rows, columns);
        blockClone.SetActive(false);
    }

    private void SetCloneProperties(Block origin, int x, int y)
    {
        Vector3 posToSpawn = new Vector3(x, y, 0);
        blockClone.transform.position = new Vector3(x, y, 0);
        if(origin.PathBlock) blockClone.GetComponent<Block>().PathBlock = true;
    }
 
    private IEnumerator MoveBlocksOnBoard(int x, int y, Direction direction)
    {
        _canSwipe = false;

        switch (direction)
        {
            case Direction.Right:
                MoveBlocksRight(y);
                break;
            case Direction.Left:
                MoveBlocksLeft(y);
                break;
            case Direction.Up:
                MoveBlocksUp(x);
                break;
            case Direction.Down:
                MoveBlocksDown(x);
                break;
        }

        yield return new WaitForSeconds(0.4f);

        _canSwipe = true;
    }

    private void MoveBlocksLeft(int y)
    {
        board[0, y].transform.position = new Vector3(columns, y , 0);
        GameObject tmpBlock = board[0, y];
        for(int i = 0; i < columns - 1; i++)
        {
            board[i , y] = board[i + 1, y];
        }

        board[columns - 1, y] = tmpBlock;
    }
    
    private void MoveBlocksRight(int y)
    {   
        board[columns - 1, y].transform.position = new Vector3(-1 ,y , 0);
        GameObject tmpBlock = board[columns - 1, y];
        for(int i = columns - 1; i > 0; i--)
        {
            board[i, y] = board[i - 1, y];
        }

        board[0, y] = tmpBlock;
    }

    private void MoveBlocksUp(int x)
    {
        board[x, rows - 1].transform.position = new Vector3(x, -1, 0);
        GameObject tmpBlock = board[x, rows - 1];
        for(int i = rows - 1; i > 0; i--)
        {
            board[x, i] = board[x, i - 1];
        }

        board[x, 0] = tmpBlock;
    }
    
    private void MoveBlocksDown(int x)
    {
        board[x, 0].transform.position = new Vector3(x, rows, 0);
        GameObject tmpBlock = board[x, 0];
        for(int i = 0; i < rows - 1; i++)
        {
            board[x, i] = board[x, i + 1];
        }

        board[x, rows - 1] = tmpBlock;
    }

    public void RedoMove()
    {
        if(moves.Count != 0)
        {
            moveCounter++;
            _uiManager.UpdateRemainingMoves();

            Move lastMove = moves.Pop();
            switch (lastMove.Direction)
            {
                case (int)Direction.Right:
                    directionToMove = Direction.Left;
                    break;
                case (int)Direction.Left:
                    directionToMove = Direction.Right;
                    break;
                case (int)Direction.Up:
                    directionToMove = Direction.Down;
                    break;
                case (int)Direction.Down:
                    directionToMove = Direction.Up;
                    break;
            }
            MoveBlocksOnScreen(lastMove.X, lastMove.Y);
        }
        
    }

    private IEnumerator GameOver()
    {
        int count = 0;
        foreach (Point point in pathCoordinate)
        {
            if(board[point.X, point.Y].GetComponent<Block>().PathBlock == true) count++;
        }
        StartCoroutine(_uiManager.GameOverSequence(count == columns ? true : false));
        
        if(count == columns)
        {
            int wins = PlayerPrefs.GetInt("Wins", 0);
            wins++;
            PlayerPrefs.SetInt("Wins", wins);

            foreach (Point point in pathCoordinate)
            {
                blockDestination = board[point.X, point.Y].transform.position;                
                yield return new WaitForSeconds(1.0f);
            }
        }
        else
        {
            int losses = PlayerPrefs.GetInt("Losses", 0);
            losses++;
            PlayerPrefs.SetInt("Losses", losses);
        }
        
    }

    private IEnumerator ExploadeBlocks()
    {
        foreach (var block in board)
        {
            if(block.GetComponent<Block>().PathBlock == false) block.GetComponent<Block>().ExploadeBlock();
            yield return new WaitForSeconds(0.15f);
        }
    }
    
    private void MoveShip()
    {
        ship.transform.position = Vector3.MoveTowards(ship.transform.position, blockDestination, 2.0f * Time.deltaTime);
    }
}
