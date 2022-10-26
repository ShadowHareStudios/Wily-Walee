using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchWalkPoint : MonoBehaviour
{

    //Search Variables
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float walkDistance;
    public LayerMask whatIsGround;
    public float alertedTimer = 10f;
    public float startAlertedTimer;
    public Vector3 playerLastSeenPosition;
    public Vector3 CreatePointAt(Vector3 walkPoint)
    {

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        /* Debug.Log("This is randomZ: " + randomZ + " This is randomX: " + randomX);*/

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }

        Vector3 dirToWalkPoint = ((transform.position + Vector3.down * 0.5f) - walkPoint).normalized;

        RaycastHit hit;
        if (Physics.Raycast((transform.position + Vector3.down * 0.5f), dirToWalkPoint, out hit))
        {
            walkPoint = hit.point;
        }
        Debug.DrawLine((transform.position + Vector3.down * 0.5f), walkPoint, Color.red, 5f);
        return walkPoint;
    }
}
