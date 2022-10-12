using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public GameUI gameUI;
    public GameObject tutorialMessage;
    public GameObject tutorialLocation;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameUI.ShowTutorialMessageUI(tutorialMessage, tutorialLocation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            {
            gameUI.HideTutorialMessageUI();
        }
    }
}
