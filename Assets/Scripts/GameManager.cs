using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameUI currentCanvas;

    bool isWinConditionMet = false;
    bool gameHasEnded = false;

    static GameManager _instance = null;
    public static GameManager instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    int _score = 0;
    int _lives = 1;

    public int maxLives = 3;
    public GameObject playerPrefab;

    [HideInInspector] public GameObject playerInstance;
    [HideInInspector] public LevelManager currentLevel;


    // example of delegate "Number Printer"
    delegate void PrintNumber(int num);
    PrintNumber printNumber;

    void NumberPrinter(int numToPrint) 
    {
       /*use to print attempt count?
       * Debug.Log("My Favourite Number is" + numToPrint);*/
        
    }

    public int score
    {
        get { return _score; }
        set
        {
            _score = value;
            //currentCanvas.SetScoreText(_score.ToString("0000"));
            //Debug.Log("Score changed to " + _score);
        }
    }

    public int lives
    {
        get { return _lives; }
        set
        {
            if (!currentCanvas)
                currentCanvas = FindObjectOfType<GameUI>();

            _lives = value;
            if (_lives > maxLives)
                _lives = maxLives;

            if (_lives < 0)
            {
                //gameover stuff can go here
                instance.EndGame(false);
                return;
            }

            //if execution reaches here - we need to respawn
            if (currentLevel)
            {
                Destroy(playerInstance);
                SpawnPlayer(currentLevel.spawnPoint);
            }

            if (currentCanvas)
            {
                //currentCanvas.SetLivesText(_lives.ToString("00"));
            }
            



        }
    }

    public bool IsWinConditionMet { get => isWinConditionMet; set => isWinConditionMet = value; }
    public bool GameHasEnded { get => gameHasEnded; set => gameHasEnded = value; }

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    void Start()
    {


        if (!currentCanvas)
        {
            currentCanvas = FindObjectOfType<GameUI>();
        }
        SceneManager.sceneLoaded += OnReload;

        printNumber += NumberPrinter;
        printNumber(6);

    }

    // Update is called once per frame
    void Update()
    {
       
        /*// Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           // if (SceneManager.GetActiveScene().name == titleScene.name)
           //{
              //  QuitGame();
            //}
        }*/
    }

    public void OnReload(Scene scene,LoadSceneMode sceneMode)
    {
        if (!currentCanvas)
        {
            currentCanvas = FindObjectOfType<GameUI>();
        }
        if (!playerInstance)
        {
            playerInstance = FindObjectOfType<Player>().gameObject;
        }
        ResumeGame();

    }

    public void QuitGame()
    {
            Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
        
    }

    public void ReturnToTitle()
    {
       // SceneManager.LoadScene(titleScene.name);
    }
    public void EndGame(bool isComplete)
    {
        isWinConditionMet = isComplete;
    }
    public void SpawnPlayer(Transform spawnLocation)
    {
        if (spawnLocation)
            playerInstance = Instantiate(playerPrefab, spawnLocation.position, spawnLocation.rotation);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void MoveToNextScene()
    {
       // SceneManager.LoadScene(nextScene.name);
    }
}