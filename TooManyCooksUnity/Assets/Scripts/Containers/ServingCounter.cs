using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServingCounter : CounterItem {
    
    public override void Start()
    {
        base.Start();
        isServingStation = true;
    }
    
    // SERVER ONLY
    public override void PlaceItem(GameObject foodItem)
    {
        string gameDialogState = "default";
        
        FoodItem f = foodItem.GetComponent<FoodItem>();
        if (f.customName == "CookedPizza" && !f.burnt)
        {
            if (f.poisoned)
            {
                Debug.Log("This is a poisoned pizza!");
                GameManager.S.guestsPoisoned += 1;
                gameDialogState = "poisonedServed";
            }
            else
            {
                Debug.Log("This is a good pizza!");
                GameManager.S.dishesServed += 1;
            }
        } else if (f.burnt)
        {
            gameDialogState = "burnt";
        }
        else
        {
            gameDialogState = "invalid";
        }
        
        switch (GameManager.S.checkGameWinCondition())
        {
            case "chef":
                gameDialogState = "chefWin";
                break;
            case "imposter":
                gameDialogState = "imposterWin";
                break;
            case "none":
                break;
        }
        
        NetworkServer.Destroy(foodItem);
        RpcServeItem(foodItem, gameDialogState);
    }
    
    [ClientRpc]
    public void RpcServeItem(GameObject foodItem, string gameDialogState)
    {
        Destroy(foodItem);
        if (GameManager.S.dialogManager.isOpen)
        {
            GameManager.S.dialogManager.EndDialog();
        }
        GameManager.S.ShowDialog(gameDialogState);
    }
    
}
