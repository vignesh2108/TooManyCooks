using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class OvenCounter : CounterItem
{

    [SyncVar]
    float cookProgress = 0;

    public Image progressBar;
    public GameObject progressCanvas;

    public float cookSpeed = 1.2f;

    bool checkCook = false;
    float changeVelocity;
    
    public Transform doorTransform;
    private bool doorIsOpen = false;
    private bool isAnimating;
    public float doorAnimTime = 1f;
    
    
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
    
    // OVERRIDE PLACEMENT POSITION HERE
    public override void Start()
    {

        // Here we override the placement position
        base.Start();
        //placePos = transform.position + transform.up * 0.1f - transform.forward * 0.1f;
    }
    
    private void Update()
    {
        if (checkCook)
        {
            CmdCookItem();
            SmoothVal();
        }
            

        checkCook = CheckCookable(itemOnCounter);

        
    }


    // Function for checking if the item on the counter has a cookable item within!
    bool CheckCookable(GameObject foodItem)
    {
        if (foodItem == null)
        {
            progressCanvas.SetActive(false);
            return false;
        }
            

        if (foodItem.GetComponent<FoodItem>() != null)
        {
            
            if (foodItem.GetComponent<FoodItem>().cookable)
            {
                Debug.Log("Is Cookable!");
                return true;
            }
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

        cookProgress += cookSpeed * Time.deltaTime;

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

        // progressBar.fillAmount = value / 10f;
    }


    [ClientRpc]
    void RpcCompleteCooking(GameObject oldFood, GameObject newFood)
    {
        checkCook = CheckCookable(newFood);
        progressBar.fillAmount = 0;
        progressCanvas.SetActive(false);
        Destroy(oldFood);
    }

    public override void LocalPlaceItem(GameObject foodItem)
    {
        base.LocalPlaceItem(foodItem);
        checkCook = CheckCookable(foodItem);
    }

    void SmoothVal()
    {
        float toVal = cookProgress / 10f;
        float curVal = Mathf.SmoothDamp(progressBar.fillAmount, toVal, ref changeVelocity, 0.2f);
        progressBar.fillAmount = curVal;

        if (cookProgress == 0 || itemOnCounter == null)
            progressCanvas.SetActive(false);
    }

}