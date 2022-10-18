using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    public float smoothMoveTime = 0.01f;
    public float turnSpeed = 8;

    public bool isHiding = false;
    Vector3 exitDir;
    public bool exitHiding = false;

    float angle;
    public float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    new Rigidbody rigidbody;
    bool disabled;

    PlayerController playerC;

    private void Start()
    {
        playerC = GetComponent<PlayerController>();
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardHasCaughtPlayer += Disable;
    }
    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled && playerC.requestMove)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }

        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        /*transform.eulerAngles = Vector3.up * angle;

        transform.Translate (transform.forward * moveSpeed * Time.deltaTime * smoothInputMagnitude, Space.World);*/

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
        if(inputMagnitude == 0)
        {
            playerC.requestMove = false;
        }
    }

  public void Hide(bool hidden)
    {
        if (hidden)
        {
            
            gameObject.layer = LayerMask.NameToLayer("HiddenPlayer");
            isHiding = true;
            Disable(true);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            isHiding = false;   
            Disable(false);
        }
    }

    public void LeaveHidingSpot(Vector3 exitDir)
    {
        
        exitHiding = true;
        transform.Translate(exitDir * moveSpeed * Time.deltaTime, Space.World);
        
        if (exitHiding == true)
        {

            exitHiding = false;
            
        }
    }


    private void Disable()
    {
        disabled = true;
        
    }
    private void Disable(bool disabled)
    {
        this.disabled = disabled; 
    }
    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime * smoothInputMagnitude);

        
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasCaughtPlayer -= Disable;
    }
}
