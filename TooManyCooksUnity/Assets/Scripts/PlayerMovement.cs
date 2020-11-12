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
    
    
    public GameObject myHands; //reference to your hands/the position where you want your object to go
    bool canpickup; //a bool to see if you can or cant pick up the item
    GameObject ObjectIwantToPickUp; // the gameobject onwhich you collided with
    bool hasItem; 
    
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
        canpickup = false;    //setting both to false
        hasItem = false;
        
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

        if(canpickup == true) // if you enter thecollider of the objecct
        {
            if (Input.GetButtonDown("Fire1")) // can be e or any key
            {
                ObjectIwantToPickUp.GetComponent<Rigidbody>().isKinematic = true;   //makes the rigidbody not be acted upon by forces
                ObjectIwantToPickUp.transform.position = myHands.transform.position; // sets the position of the object to your hand position
                ObjectIwantToPickUp.transform.parent = myHands.transform; //makes the object become a child of the parent so that it moves with the hands
                hasItem = true;
            }
        }
        if (Input.GetButtonDown("Fire2") && hasItem == true) // if you have an item and get the key to remove the object, again can be any key
        {
            ObjectIwantToPickUp.GetComponent<Rigidbody>().isKinematic = false; // make the rigidbody work again
            ObjectIwantToPickUp.transform.parent = null; // make the object no be a child of the hands
            hasItem = false;
        }
    }

  
    private void OnTriggerEnter(Collider other) // to see when the player enters the collider
    {
        if(other.gameObject.tag == "Pickable") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            canpickup = true;  //set the pick up bool to true
            ObjectIwantToPickUp = other.gameObject; //set the gameobject you collided with to one you can reference
            Debug.Log("Can Pickup!");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        canpickup = false; //when you leave the collider set the canpickup bool to false
     
    }
    void OnCollisionEnter(Collision collision) 
    {
        rb.velocity = Vector3.zero;
    }
}
