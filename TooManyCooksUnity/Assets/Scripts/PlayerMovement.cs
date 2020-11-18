using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    public Joystick joystick;  
    public Rigidbody rb;
    
    
    // Environment
    private bool isMobile;
    
    // Movement
    private Vector3 movementForce;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (hasAuthority)
        {
            joystick = FloatingJoystick.S;
        }
     
     
    }
    
    
    // Update is called once per frame
    void Update()
    {
       
        // prevent clients from updating other player objects.
        if (!isLocalPlayer) { return;}
        
        
        if (GameManager.S.isMobile)
        {
            movementForce = new Vector3(joystick.Horizontal * moveSpeed, 0, joystick.Vertical * moveSpeed);
        }
        else
        {
            movementForce = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, 0, Input.GetAxis("Vertical") * moveSpeed);
        }

        rb.velocity = movementForce;

        if (movementForce != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementForce.normalized), 0.2f);
        }
    }
    
    void OnCollisionEnter(Collision collision) 
    {
        rb.velocity = Vector3.zero;
    }
}
