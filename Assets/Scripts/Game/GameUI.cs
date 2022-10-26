using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject tutorialMessageUI;
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    // Start is called before the first frame update
    void Start()
    {
        
        

    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
    public void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }
    public void ShowGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }

    public void ShowTutorialMessageUI(GameObject tutorialMessageUI, GameObject tutorialLocation)
    {
        OnTipRequest(tutorialMessageUI);
    }
    public void OnTipRequest(GameObject tutorialMessageUI)
    {
        tutorialMessageUI.SetActive(true);
        

    }
    public void HideTutorialMessageUI(GameObject tutorialMessageUI, GameObject tutorialLocation)
    {
        tutorialMessageUI.SetActive(false);
    }

    public void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;

        
    }
}
