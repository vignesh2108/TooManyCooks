using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class SceneObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangeEquipment))]
    public EquippedItem equippedItem;

    public Rigidbody rb;
    public GameObject tomatoPrefab;
    public GameObject doughPrefab;
    
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
            case EquippedItem.dough:
                Instantiate(doughPrefab, transform);
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

    public void ToggleSelection(bool isSelected)
    {
       
        Debug.Log($"sel: {isSelected}");
        GetComponent<Highlighter>().BrightenObject(isSelected);

    }

    private void OnCollisionEnter(Collision other)
    {
        rb.isKinematic = true;
    }

  
}
