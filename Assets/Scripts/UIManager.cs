using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class UIManager : MonoBehaviour
{
    public bool showGUI = true;

    private MyNetworkManager m_NetworkManager;
    private PolePositionManager  _manager;


    #region GUIBUTTONS

    [Header("Main Menu")] [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button buttonHost;
    [SerializeField] private Button buttonClient;
    [SerializeField] private Button buttonServer;
    [SerializeField] private TMP_InputField inputFieldIP;


    [Header("Play Menu")] [SerializeField] private GameObject playMenu;
    [SerializeField] private Button playButton;
    [SerializeField] private Button spectateButton;
    [SerializeField] private Button trainButton;
    [SerializeField] private TextMeshProUGUI currentPlayers;



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


    [Header("Pause Menu")] [SerializeField] private GameObject pauseHUD;
    
    [SerializeField] private Button resume;
    [SerializeField] private Button disconnect;
    [SerializeField] private Button quit;


    [Header("Wrong Way Warning")] [SerializeField] private GameObject warning;

    #endregion
    [HideInInspector]public SetupPlayer myChangingPlayer;

    [Header("CountDown")] [SerializeField] private GameObject countDown;

    [SerializeField] private TextMeshProUGUI numbersInCountDown;

    private void Awake()
    {
        m_NetworkManager = FindObjectOfType<MyNetworkManager>();

        _manager = FindObjectOfType<PolePositionManager>();

        _name.placeholder.color =  red.GetComponent<Image>().color;
        _name.textComponent.color= red.GetComponent<Image>().color;
        pauseHUD.SetActive(false);
        warning.SetActive(false);
    }
   
    
    private void Update()
    {
        textPosition.text = _manager.GetRaceProgress();

    }



    private void Start()
    {

        buttonHost.onClick.AddListener(() => StartHost());
        buttonClient.onClick.AddListener(() => StartClient());
        buttonServer.onClick.AddListener(() => StartServer());
        ready.onClick.AddListener(() => Ready());
        blue.onClick.AddListener(() => SetColor(blue.GetComponent<Image>().color));
        green.onClick.AddListener(() => SetColor(green.GetComponent<Image>().color));
        red.onClick.AddListener(() => SetColor(red.GetComponent<Image>().color));
        black.onClick.AddListener(() => SetColor(black.GetComponent<Image>().color));
        white.onClick.AddListener(() => SetColor(white.GetComponent<Image>().color));
        yellow.onClick.AddListener(() => SetColor(yellow.GetComponent<Image>().color));
        playButton.onClick.AddListener(() => Play());
        spectateButton.onClick.AddListener(() => Spectate());
        trainButton.onClick.AddListener(() => Train());
        resume.onClick.AddListener(() => Resume());
        disconnect.onClick.AddListener(() => Disconnect());
        quit.onClick.AddListener(() => Quit());
        ActivateMainMenu();
    }

    public void Pause(){

        pauseHUD.SetActive(true);

    }
    private void Resume(){

        pauseHUD.SetActive(false);

    }

    private void Disconnect(){

        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopHost();
        

    }

    private void Quit(){

        Application.Quit();

    }

  
    private void SetColor(Color color){

        _name.placeholder.color = color;
        _name.textComponent.color= color;
        if(myChangingPlayer != null) myChangingPlayer.CmdSetColor(color);

    }

    private void Ready()
    {
        
        readyText.text = "Yes";
        ready.gameObject.GetComponent<Image>().color = Color.green;
        if(myChangingPlayer!=null) {
        myChangingPlayer.CmdSetDisplayName(_name.textComponent.text);
         }
        ActivateInGameHUD();
    }

    public void StartCountDown()
    {
        ActivateCountDown();
    }



    public void UpdateSpeed(int speed)
    {
        textSpeed.text = "Speed " + speed + " Km/h";
    }

    public void UpdateRaceRank(string rank) {
        textPosition.text = rank;
    }

     public void UpdateCurrentLap(int lap, int totalLaps) {
        textLaps.text = "LAP: " + lap + "/" + totalLaps;
    }
    public void UpdatePlayerText(int i){

        currentPlayers.text = i + "/4";

    }

    public void UpdateWarning(bool b)
    {

        warning.SetActive(b);

    }

    public void UpdateTextCountDown(string newText)
    {
        numbersInCountDown.text = newText;
    }
    

    private void ActivateMainMenu()
    {
        mainMenu.SetActive(true);
        inGameHUD.SetActive(false);
        playMenu.SetActive(false);
        personalizationMenu.SetActive(false);
        countDown.SetActive(false);
    }

    public void ActivateInGameHUD()
    {
        mainMenu.SetActive(false);
        personalizationMenu.SetActive(false);
        playMenu.SetActive(false);
        inGameHUD.SetActive(true);
        countDown.SetActive(false);
    }

    private void ActivatePersonalizationMenu(){
        
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        personalizationMenu.SetActive(true);
        inGameHUD.SetActive(false);
        countDown.SetActive(false);

    }
    private void ActivatePlayMenu(){

        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        personalizationMenu.SetActive(false);
        inGameHUD.SetActive(false);
        countDown.SetActive(false);
    }

    private void ActivateCountDown()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        personalizationMenu.SetActive(false);
        inGameHUD.SetActive(false);
        countDown.SetActive(true);
    }

    private void Play(){
        

        NetworkClient.AddPlayer();
        ActivatePersonalizationMenu();

    }

    private void Spectate(){

        Debug.Log("Not Implemented!");
    }

    private void Train(){

        Debug.Log("Not Implemented!");

    }

    private void StartHost()
    {
        m_NetworkManager.StartHost();
        ActivatePlayMenu();
    }

    private void StartClient()
    {
        m_NetworkManager.StartClient();
        m_NetworkManager.networkAddress = inputFieldIP.text;
        ActivatePlayMenu();
    }

    private void StartServer()
    {
        m_NetworkManager.StartServer();
        mainMenu.SetActive(false);
        MeshRenderer[] all = FindObjectsOfType<MeshRenderer>();
        foreach(MeshRenderer m in all) m.enabled = false;
    }

    
}