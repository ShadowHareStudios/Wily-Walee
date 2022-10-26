using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HidingSpots : MonoBehaviour
{
    float hideTime = 0f;
    bool playerIsColliding;
    Player player;
    PlayerController playerC;
    
    Vector3 exitDirection;

    private void Awake()
    {
        
    }
    private void Update()
    {
        if (playerIsColliding)
        {
            
            exitDirection = this.transform.forward.normalized;
            /* Debug.Log("Player is Colliding");*/
            if (player.GetComponent<Player>().isHiding)
            {
                hideTime += Time.deltaTime;
                player.transform.position = Vector3.Lerp(player.transform.position, transform.position, hideTime);
                if (!playerC.requestInteract)
                {
                    /* Debug.Log("exit to" + exitDirection);*/

                    /*Debug.Log("Space is Up Stay");*/
                    player.GetComponent<Player>().Hide(false);
                    player.GetComponent<Player>().LeaveHidingSpot(exitDirection);
                }
            }
            else
            {
                hideTime = 0f;
            }
            if(playerC.requestInteract)
            {
                /*Debug.Log("Space is Down Stay");*/
                if (player.GetComponent<Player>().isHiding == false)
                {
                    player.GetComponent<Player>().Hide(true);
                    
                }
            }
        }
    }
            private void OnTriggerStay(Collider other)
        {
        if (other.CompareTag("Player"))
        {
            playerIsColliding = true;
            player = other.GetComponent<Player>();
            playerC = other.GetComponent<PlayerController>();
        }
        }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsColliding = false;
        }
    }

    private void OnDrawGizmos()
    {
        exitDirection = this.transform.forward.normalized;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, exitDirection * 2);
    }

    // function for when player is close enough to hide, highlight the hiding spot, call in Update when player is colliding

}
