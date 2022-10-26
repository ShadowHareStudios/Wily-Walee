using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    PlayerInput input;

    public bool requestMove;
    public bool requestInteract;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        context.action.ReadValue<Vector2>();
        requestMove = true;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            
            requestInteract = true;
        }
        if (context.action.WasReleasedThisFrame())
        {
            
            requestInteract = false;
        }
    }
}
