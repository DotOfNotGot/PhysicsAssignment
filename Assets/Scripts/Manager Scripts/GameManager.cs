using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private CameraFollow _camera;

    private bool _isInitialized = false;
    
    [SerializeField, Range(1,2)]
    private int _playerCount = 1;

    private SpawnTransform _spawnPos;
    [SerializeField] private GameObject _playerObject;
    
    private List<GolfController> _activePlayers = new List<GolfController>();

    private int _turnIndex = 0;
    private int _sceneIndex = 0;
    private List<int> _levelSequence = new List<int>();

    private EnemyManager _enemyManager;
    
    [SerializeField]private Canvas _canvas;
    [SerializeField] private RectTransform _scoreUIHolder;
    [SerializeField] private TMP_TextInfo _playerOneScoreTextObject;
    [SerializeField] private GameObject _playerTwoTextObject;
    [SerializeField] private TMP_TextInfo _playerTwoScoreTextObject;

    private bool _playerHasWon = false;
    
    private static GameManager _singleton;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (CheckForSingleton() != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);

        bool mainMenuManagerExists = FindObjectOfType<MainMenuManager>();
        
        if (mainMenuManagerExists)
        {
            _isInitialized = false;
        }
        else if (!_isInitialized)
        {
            Init(1, new []{0, 1});
        }
        
        if (!_isInitialized) return;
        
        // TODO: Maybe make a script that keeps reference to these things for a scene and then find only that script.
        _spawnPos = FindObjectOfType<SpawnTransform>();
        _camera = FindObjectOfType<CameraFollow>();
        _enemyManager = FindObjectOfType<EnemyManager>();
        
        SpawnNewPlayer();
        _camera.ChangeCurrentPlayerController(_activePlayers[0], 0);
    }

    public GameManager CheckForSingleton()
    {
        if (_singleton == null)
        {
            _singleton = this;
        }
        else if(_singleton != this)
        {
            _singleton.Awake();
        }
        
        return _singleton;
    }
    
    public void Init(int playerCount, int[] levelSequence)
    {
        _playerCount = playerCount;
        _levelSequence.AddRange(levelSequence);
        _isInitialized = true;
    }
    
    private void ChangeTurn()
    {
        if (_playerHasWon) return;
        _turnIndex++;

        if (_activePlayers.Count == 1)
        {
            SpawnNewPlayer();
        }
        
        if (_turnIndex == _activePlayers.Count)
        {
            _turnIndex = 0;
        }
        _activePlayers[1 - _turnIndex].EndPlayerTurn();
        StartCoroutine(TurnChangeTimer(_turnIndex));
        _camera.ChangeCurrentPlayerController(_activePlayers[_turnIndex]);
    }

    private void HandleSingleplayerTurn()
    {
        _activePlayers[0].EndPlayerTurn();
        _camera.SetShouldLerp(true);
        StartCoroutine(TurnChangeTimer(0));
    }

    private IEnumerator TurnChangeTimer(int index)
    {
        yield return new WaitForSeconds(3.0f);
        _activePlayers[index].StartPlayerTurn();
    }
    
    private void SpawnNewPlayer()
    {
        Vector3 actualSpawnPos = _spawnPos.transform.position;
        if (_activePlayers.Count != 0)
        {
            actualSpawnPos = Vector3.Distance(_activePlayers[0].transform.position, _spawnPos.transform.position) > 2.0f
                ? _spawnPos.transform.position
                : _spawnPos.BackUpSpawnPos;
        }
        
        
        var newPlayer = Instantiate(_playerObject, actualSpawnPos, _spawnPos.transform.rotation).GetComponent<GolfController>();   
        newPlayer.PlayerIndex = _activePlayers.Count;
        _activePlayers.Add(newPlayer);

        if (_playerCount != 1)
        {
            newPlayer.OnTurnDone += ChangeTurn;
        }
        else
        {
            newPlayer.OnTurnDone += HandleSingleplayerTurn;
        }

        newPlayer.OnWin += HandleWin;
    }

    [ContextMenu("Handle Win")]
    public void HandleWinDebug()
    {
        HandleWin(1);
    }
    
    
    private void HandleWin(int winnerIndex)
    {
        _playerHasWon = true;
        _canvas.enabled = true;
        StartCoroutine(HandleWinUI());
    }

    private IEnumerator HandleWinUI()
    {
        List<TMP_TextInfo> playerScoreTexts = new List<TMP_TextInfo>();
        playerScoreTexts.Add(_playerOneScoreTextObject);
        if (_playerCount == 1)
        {
            _playerTwoTextObject.SetActive(false);
            _playerTwoScoreTextObject.textComponent.gameObject.SetActive(false);
        }
        else
        {
            _playerTwoTextObject.SetActive(true);
            _playerTwoScoreTextObject.textComponent.gameObject.SetActive(true);
            playerScoreTexts.Add(_playerTwoScoreTextObject);
        }
        
        var playerScores = _enemyManager.GetPlayerScores();

        for (int i = 0; i < playerScoreTexts.Count; i++)
        {
            playerScoreTexts[i].textComponent.text = playerScores[_activePlayers[i]].ToString();
        }
        
        float t = 0.0f;

        Vector2 startPos = new Vector3(_scoreUIHolder.anchoredPosition.x, -1000);
        Vector2 endPos = new Vector3(_scoreUIHolder.anchoredPosition.x, 0);
        
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            _scoreUIHolder.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            
            yield return new WaitForEndOfFrame(); 
        }

        _scoreUIHolder.anchoredPosition = endPos;

        _sceneIndex++;
        if (_sceneIndex >= _levelSequence.Count)
        {
            _sceneIndex = 0;
            _isInitialized = false;
        }
        
        yield return new WaitForSeconds(5.0f);

        _scoreUIHolder.anchoredPosition = startPos;
        _canvas.enabled = false;

        for (int i = 0; i < _activePlayers.Count; i++)
        {
            _activePlayers[0].OnTurnDone -= ChangeTurn;
            _activePlayers[0].OnTurnDone -= HandleSingleplayerTurn;
            _activePlayers[0].OnWin -= HandleWin;
        }

        _activePlayers.Clear();
        _turnIndex = 0;
        _playerHasWon = false;
        
        SceneManager.LoadScene(_levelSequence[_sceneIndex]);
    }
}
