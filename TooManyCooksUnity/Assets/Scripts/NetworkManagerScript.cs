using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public struct CreateCharacterMessage : NetworkMessage
{
    
    public string name;
    public int numPlayers;
    public Color color;
}

public class NetworkManagerScript : NetworkManager
{
    public Transform[] spawnPoints;
    public GameObject sceneObjectPrefab;

    public Color[] colors;

    
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
    }
    
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // you can send the message here, or wherever else you want
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            
            name = "Player",
            
            color = colors[0],
        };

        conn.Send(characterMessage);
        // spawn a tomato when the second player connects. 
        if (numPlayers == 2)
        {
           Debug.Log("We can start!");
        }
    }
    
    
    
    public void OnCreateCharacter(NetworkConnection conn, CreateCharacterMessage playerData)
    {
        // add player at correct spawn position
        Transform start = spawnPoints[numPlayers];
        GameObject gameObject = Instantiate(playerPrefab, start.position, start.rotation);
        PlayerScript player = gameObject.GetComponent<PlayerScript>();
        Debug.Log(playerData.name);
        player.name = playerData.name;
        player.playerColor = playerData.color;
        NetworkServer.AddPlayerForConnection(conn, gameObject);

    
    }




    /*
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
    }*/

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // do something if disconnected here. 
        // like go to main menu screen or something.

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
