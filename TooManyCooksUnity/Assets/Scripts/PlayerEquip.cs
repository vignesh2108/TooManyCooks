using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EquippedItem : byte
{
    nothing, 
    tomato,
    onion
}

public class PlayerEquip : NetworkBehaviour
{
    public GameObject sceneObjectPrefab;

    public GameObject hand;

    public GameObject tomatoPrefab;
    public GameObject onionPrefab;
    

    [SyncVar(hook = nameof(OnChangeEquipment))]
    public EquippedItem equippedItem;

    public bool canPickup;
    private GameObject pickupablePGameObject;

    void OnChangeEquipment(EquippedItem oldEquippedItem, EquippedItem newEquippedItem)
    {
        StartCoroutine(ChangeEquipment(newEquippedItem));
    }
    
    
    // Since Destroy is delayed to the end of the current frame, we use a coroutine
    // to clear out any child objects before instantiating the new one
    IEnumerator ChangeEquipment(EquippedItem newEquippedItem)
    {
        while (hand.transform.childCount > 0)
        {
            Destroy(hand.transform.GetChild(0).gameObject);
            yield return null;
        }

        switch (newEquippedItem)
        {
            case EquippedItem.onion:
                Instantiate(onionPrefab, hand.transform);
                break;
            case EquippedItem.tomato:
                Instantiate(tomatoPrefab, hand.transform);
                break;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        UseButton.S.GetComponent<Button>().onClick.AddListener(HandleButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleButtonClick();
        }
        
        // for testing purposes.
        if (Input.GetKeyDown(KeyCode.T) && equippedItem == EquippedItem.nothing)
            CmdChangeEquippedItem(EquippedItem.tomato);
    }
    
    void HandleButtonClick()
    {
        if (!isLocalPlayer) return;

        if (equippedItem != EquippedItem.nothing)
        {
            Debug.Log("dropping item");
            CmdDropItem();
        }
        else if (canPickup)
        {
            CmdPickupItem(pickupablePGameObject);
        }
    }
    
    
    [Command]
    void CmdChangeEquippedItem(EquippedItem selectedItem)
    {
        equippedItem = selectedItem;
    }
    
    [Command]
    void CmdDropItem()
    {
        // Instantiate the scene object on the server
        Vector3 pos = hand.transform.position;
        Quaternion rot = hand.transform.rotation;
        GameObject newSceneObject = Instantiate(sceneObjectPrefab, pos, rot);

        // set the RigidBody as non-kinematic on the server only (isKinematic = true in prefab)
        newSceneObject.GetComponent<Rigidbody>().isKinematic = false;

        SceneObject sceneObject = newSceneObject.GetComponent<SceneObject>();

        // set the child object on the server
        sceneObject.SetEquippedItem(equippedItem);

        // set the SyncVar on the scene object for clients
        sceneObject.equippedItem = equippedItem;

        // set the player's SyncVar to nothing so clients will destroy the equipped child item
        equippedItem = EquippedItem.nothing;

        // Spawn the scene object on the network for all to see
        NetworkServer.Spawn(newSceneObject);
    }
    
    
    // CmdPickupItem is public because it's called from a script on the SceneObject
    [Command]
    public void CmdPickupItem(GameObject sceneObject)
    {
        // set the player's SyncVar so clients can show the equipped item
        equippedItem = sceneObject.GetComponent<SceneObject>().equippedItem;

        // Destroy the scene object
        NetworkServer.Destroy(sceneObject);
    }
    
    
    private void OnCollisionEnter(Collision other) // to see when the player enters the collider
    {
        if (!isLocalPlayer) return;
        if(other.gameObject.CompareTag("Pickable")) //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            canPickup = true;  //set the pick up bool to true
            pickupablePGameObject = other.gameObject;
            Debug.Log("Can Pickup!");
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (!isLocalPlayer) return;
        if (other.gameObject.CompareTag("Pickable"))
        {
            canPickup = false; //when you leave the collider set the canpickup bool to false
            Debug.Log("Can't Pickup!");
            pickupablePGameObject = null;
        }


    }
}
