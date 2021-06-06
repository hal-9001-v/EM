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
        if (isLocalPlayer)
        {
            uiManager.GetChatInput().onEndEdit.AddListener((msg) => Send(uiManager.GetChatInput().textComponent.text));
        }
    }

    public void HandleNewMessage(string msg)
    {
        uiManager.GetChat().text += msg;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        Message += HandleNewMessage;
    }

    [Client]
    public void OnDestroy()
    {
        if (hasAuthority)
        {
            Message -= HandleNewMessage;
        }
    }

    public void Send(string msg)
    {
        if (!string.IsNullOrWhiteSpace(msg))
        {
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
        uiManager.UpdateChatLength();
        Message?.Invoke($"\n{msg}");
    }
}