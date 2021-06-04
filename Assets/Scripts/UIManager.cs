using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool showGUI = true;

    private MyNetworkManager m_NetworkManager;

    [Header("Main Menu")] [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button buttonHost;
    [SerializeField] private Button buttonClient;
    [SerializeField] private Button buttonServer;
    [SerializeField] private TMP_InputField inputFieldIP;


    [Header("Character Personalization + Ready Menu")] [SerializeField] private GameObject personalizationMenu;

    [SerializeField] private Button ready;
    [SerializeField] private Button red;
    [SerializeField] private Button blue;
    [SerializeField] private Button green;
    [SerializeField] private Button yellow;
    [SerializeField] private Button black;
    [SerializeField] private Button white;
    [SerializeField] private TMP_InputField _name;
    [SerializeField] private TextMeshProUGUI readyText;


    [Header("In-Game HUD")] [SerializeField] private GameObject inGameHUD;

    [SerializeField] private TextMeshProUGUI textSpeed;
    [SerializeField] private TextMeshProUGUI textLaps;
    [SerializeField] private TextMeshProUGUI textPosition;

    public SetupPlayer myChangingPlayer;


    private void Awake()
    {
        m_NetworkManager = FindObjectOfType<MyNetworkManager>();
        _name.placeholder.color =  red.GetComponent<Image>().color;
        _name.textComponent.color= red.GetComponent<Image>().color;

    }

    private void Start()
    {
        buttonHost.onClick.AddListener(() => StartHost());
        buttonClient.onClick.AddListener(() => StartClient());
        buttonServer.onClick.AddListener(() => StartServer());
        ready.onClick.AddListener(() => Ready());
        blue.onClick.AddListener(() => setColor(blue.GetComponent<Image>().color));
        green.onClick.AddListener(() => setColor(green.GetComponent<Image>().color));
        red.onClick.AddListener(() => setColor(red.GetComponent<Image>().color));
        black.onClick.AddListener(() => setColor(black.GetComponent<Image>().color));
        white.onClick.AddListener(() => setColor(white.GetComponent<Image>().color));
        yellow.onClick.AddListener(() => setColor(yellow.GetComponent<Image>().color));
        ActivateMainMenu();
    }

    private void setColor(Color color){

        _name.placeholder.color = color;
        _name.textComponent.color= color;
        if(myChangingPlayer != null) myChangingPlayer.CmdSetColor(color);
    }

    private void Ready(){

        readyText.text = "Yes";
        ready.gameObject.GetComponent<Image>().color = Color.green;
        if(myChangingPlayer!=null) myChangingPlayer.CmdSetDisplayName(_name.textComponent.text);
        ActivateInGameHUD();

    }

    public void UpdateSpeed(int speed)
    {
        textSpeed.text = "Speed " + speed + " Km/h";
    }

    private void ActivateMainMenu()
    {
        mainMenu.SetActive(true);
        inGameHUD.SetActive(false);
        personalizationMenu.SetActive(false);
    }

    public void ActivateInGameHUD()
    {
        mainMenu.SetActive(false);
        personalizationMenu.SetActive(false);
        inGameHUD.SetActive(true);
    }

    private void ActivatePersonalizationMenu(){

        mainMenu.SetActive(false);
        personalizationMenu.SetActive(true);
        inGameHUD.SetActive(false);

    }

    private void StartHost()
    {
        m_NetworkManager.StartHost();
        ActivatePersonalizationMenu();
    }

    private void StartClient()
    {
        m_NetworkManager.StartClient();
        m_NetworkManager.networkAddress = inputFieldIP.text;
        ActivatePersonalizationMenu();
    }

    private void StartServer()
    {
        m_NetworkManager.StartServer();
        personalizationMenu.SetActive(false);
        MeshRenderer[] all = FindObjectsOfType<MeshRenderer>();
        foreach(MeshRenderer m in all) m.enabled = false;
    }
}