using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TrashCounter : CounterItem {
    
   
    // SERVER ONLY
    public override void PlaceItem(GameObject foodItem)
    {
        NetworkServer.Destroy(foodItem);
        RpcServeItem(foodItem);
    }
    
    [ClientRpc]
    public void RpcServeItem(GameObject foodItem)
    {
        Destroy(foodItem);
    }
    
}
