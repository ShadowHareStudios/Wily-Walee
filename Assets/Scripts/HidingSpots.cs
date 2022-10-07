using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpots : MonoBehaviour
{
    float hideTime = 0f;
    bool playerIsColliding;

    Player player;
    Vector3 exitDirection;

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
            }
            else
            {
                hideTime = 0f;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                /*Debug.Log("Space is Down Stay");*/
                if (player.GetComponent<Player>().isHiding == false)
                {
                    player.GetComponent<Player>().Hide(true);

                }

            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
               /* Debug.Log("exit to" + exitDirection);*/

                /*Debug.Log("Space is Up Stay");*/
                player.GetComponent<Player>().Hide(false);
                player.GetComponent<Player>().LeaveHidingSpot(exitDirection);
            }
        }
    }
            private void OnTriggerStay(Collider other)
        {
        if (other.CompareTag("Player"))
        {
            playerIsColliding = true;
            player = other.GetComponent<Player>();
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
