using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class OvenCounter : CounterItem
{

    [SyncVar]
    public float cookProgress = 0;
    double oldCookTimer = 0; 

    public Image progressBar;
    public GameObject progressCanvas;
    
    public GameObject exclamationPoint;

    public float cookSpeed = 1.2f;

    bool checkCook = false;
    float changeVelocity;

 
    
    
    public Transform doorTransform;
    
    private bool doorIsOpen = false;
    private bool isAnimating;
    public float doorAnimTime = 1f;

    private NetworkIdentity netID = null;
    
    IEnumerator PlayDoorAnim(bool open, bool alsoReverse = false)
    {
        doorIsOpen = open;
        isAnimating = true;
        float totalTime = doorAnimTime;
        float curTime = totalTime;
        float totalAngle = 66;
        float multiplier = 1f;
        float finalAngle = 66;
        if (!open)
        {
            finalAngle = 0;
            multiplier = -1f;
        }

        while (curTime > 0)
        {
            var amount = Time.deltaTime;
            var eulerTemp = doorTransform.rotation.eulerAngles;

            doorTransform.Rotate(new Vector3( (multiplier * totalAngle) * amount / totalTime,0f, 0f),Space.Self);
            curTime -= Time.deltaTime;
            yield return null;
        }
        doorTransform.localRotation= Quaternion.Euler(new Vector3(finalAngle,0f, 0f));
        doorIsOpen = false;

        yield return new WaitForSeconds(.2f);
        if (alsoReverse)
        {
            yield return StartCoroutine(PlayDoorAnim(!open, false));
            isAnimating = false;

        }
        else
            isAnimating = false;
    }
    
    // OVERRIDE PARENT PARAMS
    public override void Start()
    {
        base.Start();
        isOven = true;
        netID = GetComponent<NetworkIdentity>();
    }
    
    private void Update()
    {
        if (checkCook && netID.hasAuthority)
        {
            CmdCookItem();
           
        }
        SmoothVal();
        checkCook = CheckCookable(itemOnCounter);
    }


    // Function for checking if the item on the counter has a cookable item within!
    bool CheckCookable(GameObject foodItem)
    {
        if (foodItem == null)
        {
            exclamationPoint.SetActive(false);
            progressCanvas.SetActive(false);
            return false;
        }
            
        FoodItem item = foodItem.GetComponent<FoodItem>();
        if (item != null)
        {
            
            if (item.cookable)
            {
                //Debug.Log("Is Cookable!");
                FoodItem nextItem = item.cooksTo.GetComponent<FoodItem>();
                if (nextItem.burnt)
                {
                    exclamationPoint.SetActive(true);
                }
                else
                {
                    exclamationPoint.SetActive(false);
                }
                return true;
            }
            exclamationPoint.SetActive(false);
        }
        return false;

    }

    // SERVER ONLY
    public override void PlaceItem(GameObject foodItem)
    {
        //Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);
        cookProgress = 0;
        base.PlaceItem(foodItem);
    }

    [Command]
    void CmdCookItem()
    {
        if (itemOnCounter == null)
        {
            cookProgress = 0;
            return;
        }

        double now = NetworkTime.time;
        if ((now - oldCookTimer) < 1)
        {
            // basically run this only every second on the server. 
            return;
        }

        oldCookTimer = now;
        cookProgress += cookSpeed;

        cookProgress = Mathf.Clamp(cookProgress, 0, 10f);

        if (cookProgress >= 10f)
        {
            // Code here to complete cooking
            cookProgress = 0;

            //itemOnCounter.GetComponent<FoodItem>().cookable = false;
            var newItem = Instantiate(itemOnCounter.GetComponent<FoodItem>().cooksTo);
            var oldItem = itemOnCounter;
            //newItem.GetComponent<FoodItem>().cookable = false;
            newItem.transform.position = oldItem.transform.position;

            NetworkServer.Destroy(itemOnCounter);
            NetworkServer.Spawn(newItem);

            //PUT THE NEW ITEM ON THE PAN HERE PLS
            base.PlaceItem(newItem);

            // RPC Callback to destroy the serverside Item
            RpcCompleteCooking(oldItem, newItem);
            return;
        }
        SmoothVal();

        RpcUpdateCooking(cookProgress);
    }

    [ClientRpc]
    void RpcUpdateCooking(float value)
    {
        Debug.Log("Cooking: " + value);
        if (value > 0)
        {
            progressCanvas.SetActive(true);
        }
        else progressCanvas.SetActive(false);
        
        SmoothVal();
        // progressBar.fillAmount = value / 10f;
    }


    [ClientRpc]
    void RpcCompleteCooking(GameObject oldFood, GameObject newFood)
    {
        checkCook = CheckCookable(newFood);
        cookProgress = 0;
        progressBar.fillAmount = 0;
        progressCanvas.SetActive(false);
        Destroy(oldFood);
    }
    
    
    public override void LocalPlaceItem(GameObject foodItem)
    {
        StartCoroutine(PlayDoorAnim(true, true));
        base.LocalPlaceItem(foodItem);
        checkCook = CheckCookable(foodItem);
    }
    
    [ClientRpc]
    public override void RpcClearCounter()
    {
        base.RpcClearCounter();
        StartCoroutine(PlayDoorAnim(true, true));
      
    }

    void SmoothVal()
    {
        //float toVal = cookProgress / 10f;
       // float curVal = Mathf.SmoothDamp(progressBar.fillAmount, toVal, ref changeVelocity, 0.2f);
        progressBar.fillAmount = cookProgress / 10f;

        if (cookProgress == 0 || itemOnCounter == null)
            progressCanvas.SetActive(false);
        
    }

}