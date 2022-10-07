using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableObjective : MonoBehaviour
{
    Animator anim;

    bool isStolen = false;

   

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (!isStolen)
            {
                GameManager.instance.currentLevel.ItemCollected();
            }
              
        }
    }
}
