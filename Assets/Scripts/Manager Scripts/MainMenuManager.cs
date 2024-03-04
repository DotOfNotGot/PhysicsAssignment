using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public int PlayerCount { get; private set; } = 1;
    
    [SerializeField] private GameManager _gameManager;

    private Canvas _canvas;
    [SerializeField] private GameObject playerSelectScreen;
    [SerializeField] private GameObject levelSelectScreen;


    private int[][] _levelSequences = new int[][]{new int[]{1, 0}};

    private int[] _chosenLevelSequence;
    
    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _canvas = GetComponent<Canvas>();
        _gameManager = _gameManager.CheckForSingleton();
    }

    public void SetPlayerCount(int amount)
    {
        PlayerCount = Mathf.Clamp(amount, 1, 2);
    }

    public void SwitchToLevelSelect()
    {
        playerSelectScreen.SetActive(false);
        levelSelectScreen.SetActive(true);
    }

    public void SwitchToPlayerSelect()
    {
        levelSelectScreen.SetActive(false);
        playerSelectScreen.SetActive(true);
    }

    public void LoadLevelPack(int index)
    {
        _canvas.enabled = false;
        _chosenLevelSequence = _levelSequences[0];
        InitializeGameManager();
        StartCoroutine(ScreenFadeThenLoad(index));
    }

    private IEnumerator ScreenFadeThenLoad(int index)
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            
            t += Time.deltaTime;
        }
        
        SceneManager.LoadScene(index);
    }

    private void InitializeGameManager()
    {
        Debug.Log("Main menu init game manager");
        _gameManager.Init(PlayerCount, _chosenLevelSequence);
    }
    
}
