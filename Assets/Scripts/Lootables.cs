using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lootables : MonoBehaviour
{
    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;

    float viewAngle;
    float playerVisibleTimer;
    float timeToSteal = 4f;
    float isStealing;
    

    Vector3 originalItemLocation;
    Color originalSpotlightColour;



    Transform player;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalItemLocation = transform.position;
        originalSpotlightColour = spotlight.color;

       
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            if (Input.GetKey(KeyCode.Space)) 
            { 
                isStealing += Time.deltaTime;
                spotlight.color = Color.Lerp(originalSpotlightColour, Color.green,  isStealing / timeToSteal);
                
            }

        }
        else
        {
            isStealing -= Time.deltaTime;

        }
        isStealing = Mathf.Clamp(isStealing, 0, timeToSteal);
        

        if ( isStealing >= timeToSteal)
        {
            GameObject.Destroy(gameObject);
            //add to player heist value total or howver else we are tracking player score
        }
    }


    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenItemAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenItemAndPlayer < viewAngle / 2f)
            {
                if (Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }



}