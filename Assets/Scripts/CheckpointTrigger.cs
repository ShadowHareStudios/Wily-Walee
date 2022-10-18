using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
   public GameManager gameManager;
   public LevelManager levelManager;
    public Player player;
    private bool playerIsColliding;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsColliding = true;
            player = other.GetComponent<Player>();
            levelManager.LevelHasBeenCompleted();
        }
    }


}
