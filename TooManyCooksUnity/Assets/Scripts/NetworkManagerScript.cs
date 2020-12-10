using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    private GameObject[] players = new GameObject[4];
    public Color[] colors;
    public int minPlayers;
    public int imposterIndex = -1;

    
    public override void OnStartServer()
    {
        base.OnStartServer();

        //NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
    }
    
    
    /*public override void OnClientConnect(NetworkConnection conn)
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
    }*/
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (numPlayers == 5)
        {
            conn.Disconnect();
            return;
        }
        
        Debug.Log("Player connected!");
        // add player at correct spawn position
        Transform start = spawnPoints[numPlayers];
        GameObject playerObj = Instantiate(playerPrefab, start.position, start.rotation);
        players[numPlayers] = playerObj;
        NetworkServer.AddPlayerForConnection(conn, playerObj);
        Debug.Log("Player connected!");

        if (numPlayers == 0)
        {
            foreach (OvenCounter o in FindObjectsOfType<OvenCounter>())
            {
                o.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
            }
        }
        //Choose an imposter when enough players connect. 
        
        if (numPlayers >= minPlayers)
        {
            imposterIndex = Random.Range(0, numPlayers);
            for (int i = 0; i < numPlayers; i++)
            {
                PlayerScript player = players[i].GetComponent<PlayerScript>();
                player.playerName = $"P {i+1}";
                if (i == imposterIndex)
                {
                    player.isImposter = true;
                }
                else
                {
                    player.isImposter = false;
                }
                player.playerColor = colors[i];
            }
            Debug.Log($"{numPlayers} players spawned!, imposter: P {imposterIndex + 1}");
        }
    }
    
    
    /*public void OnCreateCharacter(NetworkConnection conn, CreateCharacterMessage playerData)
    {
        // add player at correct spawn position
        Transform start = spawnPoints[numPlayers];
        GameObject gameObject = Instantiate(playerPrefab, start.position, start.rotation);
        PlayerScript player = gameObject.GetComponent<PlayerScript>();
        Debug.Log(playerData.name);
        player.name = playerData.name;
        player.playerColor = playerData.color;
        NetworkServer.AddPlayerForConnection(conn, gameObject);

    
    }*/




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
        if (numPlayers < 2)
        {
            NetworkServer.DisconnectAllConnections();
        }
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
