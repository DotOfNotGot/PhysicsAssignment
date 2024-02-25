using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField, Range(1,2)]
    private int _playerCount = 1;

    [SerializeField] private Transform _spawnPos;
    [SerializeField] private GameObject _playerObject;
    
    private List<GolfController> _activePlayers = new List<GolfController>();

    private int _turnIndex = 0;

    private CameraFollow _camera;

    [SerializeField] private Canvas _canvas;
    
    // Start is called before the first frame update
    void Awake()
    {
        _canvas = GetComponentInChildren<Canvas>();
        var mainMenuMan = FindObjectOfType<MainMenuManager>();

        if (mainMenuMan)
        {
            _playerCount = mainMenuMan.PlayerCount;
        }
        
        _camera = FindObjectOfType<CameraFollow>();
        SpawnNewPlayer();
        _camera.ChangeCurrentPlayerController(_activePlayers[0]);
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
        var newPlayer = Instantiate(_playerObject, _spawnPos).GetComponent<GolfController>();   
        newPlayer.PlayerIndex = _activePlayers.Count;
        _activePlayers.Add(newPlayer);

        if (_playerCount != 1)
        {
            newPlayer.OnTurnDone += ChangeTurn;
        }

        newPlayer.OnWin += HandleWin;
    }

    private void HandleWin(int winnerIndex)
    {
        Time.timeScale = 0;
        Debug.Log($"Player {winnerIndex} won!");
        _canvas.enabled = true;
    }
    
}
