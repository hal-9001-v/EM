using System;
using Mirror;
using UnityEngine;
using TMPro;
using Random = System.Random;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class SetupPlayer : NetworkBehaviour
{   
    
    [SyncVar] private int _id;

    [SyncVar(hook = nameof(HandleDisplayNameUpdated))]
    private string _name;

    [SyncVar(hook = nameof(HandleDisplayColorUpdated))]
    private Color _carColor;

    private UIManager _uiManager;
    private MyNetworkManager _networkManager;
    private PlayerController _playerController;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private TextMeshProUGUI _nameText;
    private PlayerInfo _playerInfo;
    private PolePositionManager _polePositionManager;

    public struct ServerMessage : NetworkMessage
    {
        public int client_numberPlayers;
    }

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        _id = NetworkServer.connections.Count - 1;
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        _playerInfo.ID = _id;
        int aux = _id + 1;
        _playerInfo.Name = "Player " + aux;
        _playerInfo.CurrentColor = new Color(0.91f,0.33f,0.33f,1);
        _playerInfo.CurrentLap = 0;
        _polePositionManager.AddPlayer(_playerInfo);

    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        
        CmdSetDisplayName(_playerInfo.Name);
        CmdSetColor(_playerInfo.CurrentColor);
        InitializeInput();
        _playerController.InitializeInput(_input);
        
    }

    /// <summary>Stop event, only called on client and host.</summary>
    public override void OnStopClient()
    {

    }

    #endregion

    private BasicPlayer _input;

    private void InitializeInput(){

            _input = new BasicPlayer();

            _input.PC.Pause.performed += ctx =>
            {
                _uiManager.Pause();

            };
            _input.Enable();
    }

 
    private void Awake()
    {
        _playerInfo = GetComponent<PlayerInfo>();
        _playerController = GetComponent<PlayerController>();
        _networkManager = FindObjectOfType<MyNetworkManager>();
        _polePositionManager = FindObjectOfType<PolePositionManager>();
        _uiManager = FindObjectOfType<UIManager>();
    }

    private void OnApplicationQuit()
    {
        _polePositionManager.RemovePlayer(_playerInfo);
        Destroy(gameObject);
        NetworkManager.singleton.StopClient();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            _playerController.enabled = true;
            _playerController.OnSpeedChangeEvent += OnSpeedChangeEventHandler;
            _uiManager.myChangingPlayer = this;
            ConfigureCamera();
            NetworkClient.RegisterHandler<ServerMessage>(OnServerNotification);
        }
    }

    #region syncvar handling

    void HandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        Debug.Log("Color Cambiado");
        _meshRenderer.materials[1].color = newColor;
        _nameText.color = newColor;
        _playerInfo.CurrentColor = newColor;    
        }

    void HandleDisplayNameUpdated(string oldName, string newName)
    {
        Debug.Log("Nombre cambiado de " + oldName + " a " + newName);
        _nameText.text = newName;
        _playerInfo.name = newName;
    }

    void OnServerNotification(ServerMessage message)
    {
        Debug.Log("[CLIENT] Número de jugadores en el Lobby -> " + message.client_numberPlayers);
    }

    [Server]
    public void SetDisplayName(string newName)
    {
        int aux = _id + 1;
        if (newName.Length < 2 || newName.Length > 14)  _name = "Player " + aux;
        else _name = newName;
    }

    [Server]
    public void SetDisplayColor(Color newColor)
    {
        _carColor = newColor;
    }

    public string GetDisplayName()
    {
        return _name;
    }

    public Color GetDisplayColor()
    {
        return _carColor;
    }

    [Command]
    public void CmdSetDisplayName(string newName)
    {
        SetDisplayName(newName);
    }

    [Command]
    public void CmdSetColor(Color newColor)
    {
        SetDisplayColor(newColor);
    }

    #endregion

    void OnSpeedChangeEventHandler(float speed)
    {
        _uiManager.UpdateSpeed((int) speed * 5); // 5 for visualization purpose (km/h)
    }

    void ConfigureCamera()
    {
        if (Camera.main != null) Camera.main.gameObject.GetComponent<CameraController>().m_Focus = this.gameObject;
    }

    #region Test

    [ContextMenu("Cambiar nombre a dexaxi")]
    private void SetName()
    {
        CmdSetDisplayName("dexaxi");
    }


    [ContextMenu("Cambiar Color a Verde")]
    private void SetColor()
    {
        CmdSetColor(Color.green);
    }

    #endregion
}