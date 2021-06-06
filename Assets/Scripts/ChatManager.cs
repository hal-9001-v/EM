using System;
using Mirror;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private UIManager uiManager;
    private BasicPlayer input;

    private static event Action<string> Message;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        input = new BasicPlayer();
        Debug.Log("This is UImanager " + uiManager.GetChatInput());
        if (isLocalPlayer)
            uiManager.GetChatInput().onEndEdit.AddListener((msg) => Send(uiManager.GetChatInput().textComponent.text));

        if (isLocalPlayer && hasAuthority)
        {
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        uiManager.GetChatObject().SetActive(true);
        Message += HandleNewMessage;
    }

    public void HandleNewMessage(string msg)
    {
        uiManager.GetChat().text += msg;
    }

    [Client]
    public void OnDestroy()
    {
        if (hasAuthority)
        {
            Message -= HandleNewMessage;
        }
    }

    [Client]
    public void Send(string msg)
    {
        if (!string.IsNullOrWhiteSpace(msg))
        {
            Debug.Log(msg);
            CmdSendMsg(msg);
            uiManager.GetChatInput().text = string.Empty;
        }
    }

    [Command]
    private void CmdSendMsg(string msg)
    {
        RpcSendToClients($"{msg}");
    }

    [ClientRpc]
    private void RpcSendToClients(string msg)
    {
        Message?.Invoke($"\n{msg}");
    }
}