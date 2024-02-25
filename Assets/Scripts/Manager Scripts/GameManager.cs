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

    private Transform _spawnPos;
    [SerializeField] private GameObject _playerObject;
    
    private List<GolfController> _activePlayers = new List<GolfController>();

    private int _turnIndex = 0;
    private List<int> _levelSequence = new List<int>();

    [SerializeField]private Canvas _canvas;
    [SerializeField] private RectTransform _scoreUIHolder;
    [SerializeField] private TMP_TextInfo _playerOneScoreTextObject;
    [SerializeField] private GameObject _playerTwoTextObject;
    [SerializeField] private TMP_TextInfo _playerTwoScoreTextObject;

    private static GameManager _singleton;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (_singleton == null)
        {
            _singleton = this;
        }
        else if(_singleton != this)
        {
            _singleton.Awake();
            Destroy(this.gameObject);
            Debug.Log("Destroyed extra game manager");
        }

        DontDestroyOnLoad(this);

        if (FindObjectOfType<MainMenuManager>() || !_isInitialized) return;
        
        // TODO: Maybe make a script that keeps reference to these things for a scene and then find only that script.
        _spawnPos = FindObjectOfType<SpawnTransform>().transform;
        _camera = FindObjectOfType<CameraFollow>();
        
        Debug.Log("It do thing?");
        
        SpawnNewPlayer();
        _camera.ChangeCurrentPlayerController(_activePlayers[0], 0);
    }

    public void Init(int playerCount, int[] levelSequence)
    {
        Debug.Log($"Initializing game manager: player count {_playerCount}, {playerCount}");
        _playerCount = playerCount;
        _levelSequence.AddRange(levelSequence);
        Debug.Log($"Initialized game manager: player count {_playerCount}, {playerCount}");
        _isInitialized = true;
    }
    
    private void ChangeTurn()
    {
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
        _activePlayers[_turnIndex].StartPlayerTurn();
        
        _camera.ChangeCurrentPlayerController(_activePlayers[_turnIndex]);
    }

    private void SpawnNewPlayer()
    {
        Debug.Log($"Spawning player, Player count: {_playerCount}");
        var newPlayer = Instantiate(_playerObject, _spawnPos).GetComponent<GolfController>();   
        newPlayer.PlayerIndex = _activePlayers.Count;
        _activePlayers.Add(newPlayer);

        if (_playerCount != 1)
        {
            newPlayer.OnTurnDone += ChangeTurn;
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
        Debug.Log($"Player {winnerIndex} won!");
        _canvas.enabled = true;
        StartCoroutine(HandleWinUI());
    }

    private IEnumerator HandleWinUI()
    {
        if (_playerCount == 1)
        {
            _playerTwoTextObject.SetActive(false);
            _playerTwoScoreTextObject.textComponent.gameObject.SetActive(false);
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
    }
}
