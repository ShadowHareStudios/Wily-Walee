using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Lootables : MonoBehaviour
{
    public LevelManager levelManager;
    public GameManager gameManager;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;

    float viewAngle;
    float playerVisibleTimer;
    float timeToSteal = 4f;
    float isStealing;
    public bool stolen;
    

    Vector3 originalItemLocation;
    Color originalSpotlightColour;



    Transform player;

    public static event System.Action PlayerHasStolen;



    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalItemLocation = transform.position;
        originalSpotlightColour = spotlight.color;
        stolen = false;

       
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
            spotlight.color = Color.Lerp(spotlight.color, originalSpotlightColour, timeToSteal);

        }
        isStealing = Mathf.Clamp(isStealing, 0, timeToSteal);
        

        if ( isStealing >= timeToSteal)
        {
            GameObject.Destroy(gameObject);
            stolen = true;
            
            //add to player heist value total or howver else we are tracking player score
            /*PlayerHasStolen();*/
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
