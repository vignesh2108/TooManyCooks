using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagerScript : NetworkManager
{
    public Transform[] spawnPoints;
    public GameObject sceneObjectPrefab;
    public Color[] colors;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // add player at correct spawn position
        Transform start = spawnPoints[numPlayers];
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        assignHatColor(player);
        NetworkServer.AddPlayerForConnection(conn, player);

        // spawn a tomato when the second player connects. 
        if (numPlayers == 2)
        {
           // Do something cool here.
           //SpawnTomato();
        }
    }

    void assignHatColor(GameObject player)
    {
        
        foreach (Renderer r in player.GetComponentsInChildren<Renderer>())
        {
            if (r.gameObject.name == "hat")
            {
                r.material.color = colors[numPlayers];
            }
        }

        
    }

    void SpawnTomato()
    {
        GameObject newSceneObject = Instantiate(sceneObjectPrefab, transform.position, transform.rotation);
        
        // set the RigidBody as non-kinematic on the server only (isKinematic = true in prefab)
        newSceneObject.GetComponent<Rigidbody>().isKinematic = false;
        
        SceneObject sceneObject = newSceneObject.GetComponent<SceneObject>();
        
        // set the child object on the server
        sceneObject.SetEquippedItem(EquippedItem.tomato);
        
        // set the SyncVar on the scene object for clients
        sceneObject.equippedItem = EquippedItem.tomato;
        
        
        // Spawn the scene object on the network for all to see
        NetworkServer.Spawn(newSceneObject);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // do something if disconnected here. 
        // like go to main menu screen or something.

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
