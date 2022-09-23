using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openDistance;
    public float timeToOpen = 0.5f;
    bool isOpen = false;
    public Lootables[] doorObjectives;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool allObjectivesComplete = true;
        for (int i = 0; i < doorObjectives.Length; i++)
        {
            if (doorObjectives[i])
            {
                if (doorObjectives[i].stolen == false)
                {
                    allObjectivesComplete = false;
                    break;
                }
            }
        }

        if(allObjectivesComplete)
        {
            if (!isOpen)
            {
                Destroy(gameObject);
                isOpen = true;
            }

        }
    }

    // Do not use
    IEnumerator OpenDoor()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = transform.position + Vector3.right * openDistance;
        float timeElapsed = 0f;
        while (timeElapsed < timeToOpen)
        {
            Vector3.Lerp(oldPos, newPos, timeElapsed);
            yield return null;
            timeElapsed += Time.unscaledDeltaTime;
        }
    }
}
