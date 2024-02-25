using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public int PlayerCount { get; private set; } = 1;

    private Canvas _canvas;
    [SerializeField] private GameObject playerSelectScreen;
    [SerializeField] private GameObject levelSelectScreen;
    
    // Start is called before the first frame update
    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        DontDestroyOnLoad(this);   
    }

    // Update is called once per frame
    void Update()
    {
        
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
        SceneManager.LoadScene(index);
    }
    
    
}
