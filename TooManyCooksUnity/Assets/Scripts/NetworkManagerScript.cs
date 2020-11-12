using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagerScript : NetworkManager
{
    public Transform[] spawnPoints;
    GameObject ball;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // add player at correct spawn position
        Transform start = spawnPoints[numPlayers];
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        // Start game if there are 5 players
        if (numPlayers == 5)
        {
           // Do something cool here.
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // do something if disconnected here. 
        // like go to main menu screen or something.

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
