using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{

    private UIManager UI;
    private SetupPlayer player;

 #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("[CLIENT] Jugador conectado.");


    }

    #endregion
  
  #region Server

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        UI = FindObjectOfType<UIManager>();
        player = conn.identity.GetComponent<SetupPlayer>();

        

        Debug.Log("[SERVER] Se ha conectado: " + player.GetDisplayName());
    }



      


    #endregion

}
