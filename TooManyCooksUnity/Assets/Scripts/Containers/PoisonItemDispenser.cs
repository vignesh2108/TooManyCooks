using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PoisonItemDispenser : ItemDispenser
{
   public GameObject[] itemsToDispense;

   public int prevDispenseIndex = 0;
   
   
   public override void UseCounter(GameObject player, bool cont, float deltaTime)
   {
      //Debug.Log("CMD ON CLIENT ERROR: SOURCE IS :" + name);

      // Make sure the counter is blank before dispensing something
      if (itemOnCounter == null && itemsToDispense.Length != 0 && !waitTrigger)
      {
         int nextIndex = Random.Range(0, itemsToDispense.Length);
         if (nextIndex == prevDispenseIndex)
         {
            nextIndex = (nextIndex + 1) % itemsToDispense.Length;
         }
         
         itemToDispense = itemsToDispense[nextIndex];
         prevDispenseIndex = nextIndex;
         
         var obj = Instantiate(itemToDispense);
         obj.transform.position = transform.position;
         if (player.GetComponent<PlayerScript>().isImposter)
         {
            obj.GetComponent<FoodItem>().poisoned = true;
         }

         // Spawn the item to dispense
         NetworkServer.Spawn(obj);

         //Set its position
         obj.transform.position = transform.position + new Vector3(0, 1f, 0);

         // Set its name to the proper format. (Food Item Name + NET ID VALUE)
         obj.name = obj.GetComponent<FoodItem>().itemName + obj.GetComponent<NetworkIdentity>().netId;

         // Set the itemoncounter name to be properly set
         itemOnCounterName = obj.name;

         // Perform the local placing function
         PlaceItem(obj);

         RpcDispenserCallback(player, obj);

         StartCoroutine(WaitTrigger());
      }
   }
}
