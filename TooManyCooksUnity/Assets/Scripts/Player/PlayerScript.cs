using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerScript : NetworkBehaviour
{
    public TextMesh playerNameText;
    public GameObject floatingInfo;
    
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;
    
    [SyncVar(hook = nameof(OnImposterChange))]
    public bool isImposter;
    
    
    [SyncVar(hook = nameof(AssignHatColor))]
    public Color playerColor;

    public Color imposterColor;
    
    private bool assignedImposterColor = false;
    
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

    /*public override void OnStartLocalPlayer()
    {
        //string name = "Player" + Random.Range(100, 999);
        //Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Debug.Log($"player :{name}, {playerColor}");
        CmdSetupPlayer(name, playerColor);

    }*/

    [Command]
    public void CmdSetupPlayer(string _name, Color _color)
    {
        playerName = _name;
        playerColor = _color;
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
        
        if (!isLocalPlayer)
        {
            Destroy(GetComponentInChildren<ItemDetector>());
        }
    }
    
    void OnNameChanged(string _Old, string _New)
    {
        Debug.Log("Name changed!");
        playerName = _New;
        playerNameText.text = _New;
    }

    public void OnImposterChange(bool old, bool _new)
    {
        if (_new == true || isImposter) // change to isLocalPlayer after testing
        {
            Debug.Log($"I am imposter: {isImposter}");
            
        }
    }


    private void assignUpdatedColor()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
                
            if (r.gameObject.name == "hat")
            {
                Debug.Log($"Changing hat: {isImposter}");
                r.material.color = isImposter? imposterColor : playerColor;
                assignedImposterColor = isImposter;
            }
        }
    }
    
    public void AssignHatColor(Color oldColor, Color newColor)
    {
        
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (r.gameObject.name == "hat")
            {
                r.material.color = newColor;
                assignedImposterColor = false;
            }
        }
    }

    
    
    // Update is called once per frame
    void Update()
    {
        floatingInfo.transform.LookAt(Camera.main.transform);
        // prevent clients from updating other player objects.
        if (!isLocalPlayer)
        {
            
            return;
        }

        if (isImposter)
        {
            playerNameText.text = "Imposter!";
        } else if (!isImposter)
        {
            playerNameText.text = playerName;
        }
        
        
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
        if (!isLocalPlayer) return;
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
        if (!isLocalPlayer) return;
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
