using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

public class SceneObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangeEquipment))]
    public EquippedItem equippedItem;

    public Rigidbody rb;
    public GameObject tomatoPrefab;
    public GameObject onionPrefab;
    
    private bool canPickup;
    
    void OnChangeEquipment(EquippedItem oldEquippedItem, EquippedItem newEquippedItem)
    {
        StartCoroutine(ChangeEquipment(newEquippedItem));
    }

    // Since Destroy is delayed to the end of the current frame, we use a coroutine
    // to clear out any child objects before instantiating the new one
    IEnumerator ChangeEquipment(EquippedItem newEquippedItem)
    {
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            yield return null;
        }

        // Use the new value, not the SyncVar property value
        SetEquippedItem(newEquippedItem);
    }
    
    // SetEquippedItem is called on the client from OnChangeEquipment (above),
    // and on the server from CmdDropItem in the PlayerEquip script.
    public void SetEquippedItem(EquippedItem newEquippedItem)
    {
        switch (newEquippedItem)
        {
            case EquippedItem.tomato:
                Instantiate(tomatoPrefab, transform);
                break;
            case EquippedItem.onion:
                Instantiate(onionPrefab, transform);
                break;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }

    private void ToggleSelection(bool isSelected)
    {
       
        Debug.Log($"sel: {isSelected}");
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = isSelected;
        }

    }
    
    
    private void OnCollisionEnter(Collision other) // to see when the player enters the collider
    {
        if(other.gameObject.CompareTag("Player")) //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            this.ToggleSelection(true);
        }
        else
        {
            rb.isKinematic = true;
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            this.ToggleSelection(false);
        }
        
        
    }
}
