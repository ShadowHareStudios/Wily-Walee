using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpots : MonoBehaviour
{
    float hideTime = 0f;
    bool playerIsColliding;

    Player player;

    private void Update()
    {
        if (playerIsColliding)
        {

            Debug.Log("Player is Colliding");
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
                Debug.Log("Space is Down Stay");
                if (player.GetComponent<Player>().isHiding == false)
                {
                    player.GetComponent<Player>().Hide(true);

                }

            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Debug.Log("Space is Up Stay");
                player.GetComponent<Player>().Hide(false);
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

    // function for when player is close enough to hide, highlight the hiding spot, call in Update when player is colliding

}
