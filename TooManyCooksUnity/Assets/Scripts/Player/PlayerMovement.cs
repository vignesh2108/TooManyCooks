using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    private Joystick joystick;
    private CameraScript cameraObject;
    public GameObject hands;
    public Rigidbody rb;
    
    public ActionHandler action;
    [Header("References")]
    [SerializeField] public Animator playerAnimator; 
    
    
    // Environment
    private bool isMobile;
    
    // Movement
    private Vector3 movementForce;
    public float moveSpeed;

    private void Awake()
    {
        action = GetComponentInParent<ActionHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, 18f, transform.position.z);
        rb = GetComponent<Rigidbody>();
        if (hasAuthority)
        {
            joystick = FloatingJoystick.S;
            cameraObject = CameraScript.S;
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
        
        ClientAnimatePlayerWalkRPC(movementForce != Vector3.zero);
        
        var position = transform.position;
        cameraObject.transform.position = new Vector3(position.x, cameraObject.transform.position.y, position.z - 50);
    }

    // [Command]
    // void CallAnimatePlayerWalk(bool isWalking)
    // {
    //     ClientAnimatePlayerWalkRPC(isWalking);
    // }

    
    void ClientAnimatePlayerWalkRPC(bool isWalking)
    {
        playerAnimator.SetBool("isWalking", isWalking);
    }

    void OnCollisionEnter(Collision collision) 
    {
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
 
        if (other.gameObject.GetComponentInParent<FoodItem>() != null)
        {
            var f = other.gameObject.GetComponentInParent<FoodItem>();
            Debug.Log("Food!:");
            
            
            action.itemFocusOverride = f.gameObject;
            action.itemFocusOverride.GetComponent<Highlighter>().BrightenObject(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FoodItem>() != null)
        {
            if (action.itemFocusOverride != null)
            {
                action.itemFocusOverride.GetComponent<Highlighter>().BrightenObject(false);
                action.itemFocusOverride = null;
            }
        }
    }
}
