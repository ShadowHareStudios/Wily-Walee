using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    

    public int startingLives;
    public Transform spawnPoint;
    //public SceneAsset nextScene;
    /*public TimeManager tm;*/

    [Header("Objectives")]
    public Lootables[] levelObjectives;
    int itemsCollected = 0;
    public GameUI gameUI;

    public Guard guardsOnDuty;

    public AudioSource levelMusic;
    // PlayerSounds ps;

    // Start is called before the first frame update
    void Start()
    {
        // ps = GetComponent<PlayerSounds>();
        
        GameManager.instance.lives = startingLives;
        GameManager.instance.SpawnPlayer(spawnPoint);
        GameManager.instance.currentLevel = this;

        
        //if (nextScene) GameManager.instance.nextScene = nextScene;

    }

    private void LateUpdate()
    {
        bool allObjectivesComplete = true;
        for(int i = 0; i < levelObjectives.Length; i ++)
        {
            if (levelObjectives[i])
            {
                if (levelObjectives[i].stolen == false)
                {
                    allObjectivesComplete = false;
                    break;
                }
            }
        }
        GameManager.instance.IsWinConditionMet = allObjectivesComplete;
       // Debug.Log("has Won = " + GameManager.instance.IsWinConditionMet);
    }

    public void PauseLevelMusic()
    {
        if (levelMusic)
        {
            levelMusic.Pause();
        }
    }

    public void LevelHasBeenCompleted()
    {
        // Play sounds, animations, music and all other things that happen when the player wins
        GameUI canvas = GameManager.instance.currentCanvas;
        canvas.ShowGameWinUI();
        /*canvas.startButton.onClick.AddListener(() => GameManager.instance.ReloadScene());*/
        canvas.gameObject.SetActive(true);
        /*tm.StopTime();*/
       
    }

    public void LevelHasBeenLost()
    {
        GameUI canvas = GameManager.instance.currentCanvas;
        canvas.ShowGameLoseUI();
        
        canvas.gameObject.SetActive(true);
/*        tm.StopTime();*/
        
    }

    public void ItemCollected()
    {
        itemsCollected += 1;
    }
}