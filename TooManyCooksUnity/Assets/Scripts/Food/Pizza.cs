using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pizza : FoodContainer {

    public GameObject pizzaBase;
    

    public GameObject sauceObj;
    public GameObject cheeseObj;


    [SyncVar]
    public bool sauce = false;

    [SyncVar]
    public bool cheese = false;

    public override void Start()
    {
        base.Start();

        cookable = false;
        if (sauce || cheese)
            InitPizza();
    }

    void InitPizza()
    {
        if (sauce)
        {
            sauceObj.GetComponent<MeshRenderer>().enabled = true;
            inContainer.Add("Sliced Tomato");
        }

       
        if (cheese)
        { 
            cheeseObj.SetActive(true);
            inContainer.Add("Sliced Cheese");
        }

        UpdatePizza();

    }

    public override bool CanInsertItem(GameObject item)
    {

        switch (item.GetComponent<FoodItem>().itemName)
        {
            case "Sliced Tomato":
                if (sauce)
                    return false;
                else return true;
          
            case "Sliced Cheese":
                if (cheese)
                    return false;
                else return true;
            default:
                return false;
        }

    }

    // Runs as a command on the server
    public override void InsertFood(GameObject foodItem)
    {

        //Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);
        FoodItem f = foodItem.GetComponent<FoodItem>();
        var fname = f.customName;
        GameObject smokeEffect = null;
        if (f.poisoned)
        {   
            smokeEffect = Instantiate(GameManager.S.SmokePrefab);
            poisoned = true;
            smokeEffect.transform.position = transform.position;
           
        }
        switch (fname)
        {
            case "Sliced Tomato":
                sauce = true;
                break;
            
            case "Sliced Cheese":
                cheese = true;
                break;
            default:
                break;
        }

        inContainer.Add(fname);

        RpcUpdateFood(fname);
        
        if (smokeEffect != null)
        {
            NetworkServer.Spawn(smokeEffect);
        }

        NetworkServer.Destroy(foodItem);
    
        if (cheese && sauce)
        {
            cookable = true;
        }
        else
        {
            cookable = false;
        }
        
    }

    [ClientRpc]
    public void RpcUpdateFood(string fname)
    {

        switch (fname)
        {
            case "Sliced Tomato":
                sauceObj.GetComponent<MeshRenderer>().enabled = true;
                break;
           
            case "Sliced Cheese":
                cheeseObj.SetActive(true);
                break;
            default:
                break;
        }

        UpdatePizza();

    }

    void UpdatePizza()
    {

        if (cheese && sauce)
        {
            cookable = true;
        }
        else
        {
            cookable = false;
        }
        
        /*var lastPos = pizzaBase.transform.localPosition;

      
        if (cheese)
        {
            cheeseObj.GetComponent<MeshRenderer>().enabled = true;
            cheeseObj.transform.transform.localPosition = lastPos;
            lastPos = cheeseObj.transform.localPosition;
        }
        else lastPos -= new Vector3(0, 0.02f, 0);

        if (sauce)
        {
            sauceObj.GetComponent<MeshRenderer>().enabled = true;
            sauceObj.transform.transform.localPosition = lastPos;
            lastPos = sauceObj.transform.localPosition;
        }
        else lastPos -= new Vector3(0, 0.01f, 0);*/

    }
    
}
