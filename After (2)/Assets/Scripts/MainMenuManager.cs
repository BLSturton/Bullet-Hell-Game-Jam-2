using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Mirror;
using Mirror.Discovery;

public class MainMenuManager : MonoBehaviour
{   // so that we can access and change the data
    [SerializeField] private NetworkManager manager = null;//set outside
    [SerializeField] private NetworkDiscovery _discovery = null;//set outside
                                                                
    List<ServerResponse> discoveredServers = new List<ServerResponse>();

    [SerializeField] private Canvas[] PresentCanvas = null;
    [SerializeField] private Slider[] PresentSliders = null;
    [SerializeField] private AudioSource[] PresentMusic = null;
    [SerializeField] private TMP_Dropdown[] PresentDropdowns = null;

    public InputActionReference[] ActionsFromActionInputs = null;
    [SerializeField] private TMP_Text[] RebindText = null;
    [SerializeField] private GameObject[] StartRebindObject = null; //buttons

    public PlayerInput _playerInput = null;

    [SerializeField] private UniversalAdditionalCameraData CameraRenderer = null;

    [SerializeField] private TMP_Text[] ServerJoinText = null;
    [SerializeField] private Image[] ServerJoinImages = null;//these are the buttons, just hidden

    [SerializeField] private ScrollRect _scrollRect;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
    private bool rebinding;

    private int WidthResolution = 1920;
    private int HeightResolution = 1080;
    private bool Fullscreen;

    [SerializeField] private GameObject _soloPlayer = null;
    [SerializeField] private GameObject _soloEnemy = null;

    public CinemachineVirtualCamera CameraOne;

    public Transform _SoloPlayer;
    public Transform _SoloEnemy;
    public Transform CameraTransform;// make it so the player that was creted is the one being followed... Client or Server?

    public int Countdown = 10;

    public bool inGame;

    [SerializeField] private Transform Blast;
    [SerializeField] private Animator TranformationBlastAnim;
    [SerializeField] private Animator TranformationBlastRot;
    [SerializeField] private TMP_Text Count = null;

    private void Start()
    {
        LoadMenuSettings();
        CameraRenderer.antialiasingQuality = AntialiasingQuality.High;
    }


    public void LoadMenuSettings()//local load
    {
        _playerInput.actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Rebinds", string.Empty));
        UpdateRebindText();
        PresentDropdowns[0].value = PlayerPrefs.GetInt("SavedCameraMISettings", 0);//take this to change the camera invert settings
        //get the cinemachine and implement change
        PresentSliders[0].value = PlayerPrefs.GetFloat("SavedCameraMSSettings", 0.5000f);//take this to change the camera
        //get the cinemachine and implement change
        PresentDropdowns[1].value = PlayerPrefs.GetInt("SavedScreenModeSettings", 1);
        ChangeScreenMode(PresentDropdowns[1].value);
        PresentDropdowns[2].value = PlayerPrefs.GetInt("SavedResolutionSettings", 4);//4 for webgl, 1 for desctop version
        ChangeResolution(PresentDropdowns[2].value);
        PresentDropdowns[3].value = PlayerPrefs.GetInt("SavedVSyncSettings", 0);
        QualitySettings.vSyncCount = PresentDropdowns[3].value;
        PresentDropdowns[4].value = PlayerPrefs.GetInt("SavedAntiAliasingSettings", 0);
        ChangeAntiAliasing(PresentDropdowns[4].value);
        PresentSliders[1].value = PlayerPrefs.GetFloat("SavedGeneralVolumeSettings", 1f);
        PresentSliders[2].value = PlayerPrefs.GetFloat("SavedMusicVolumeSettings", 0.7500f);
        PresentMusic[0].volume = PresentSliders[2].value * PresentSliders[1].value;
        PresentSliders[3].value = PlayerPrefs.GetFloat("SavedAmbientVolumeSettings", 0.7500f);
        PresentMusic[1].volume = PresentSliders[3].value * PresentSliders[1].value;
        PresentSliders[4].value = PlayerPrefs.GetFloat("SavedSoundFXVolumeSettings", 0.7500f);
        PresentMusic[2].volume = PresentSliders[4].value * PresentSliders[1].value;
    }

    public void UpdateUIArrangement(int widthOfScreen, int heightOfScreen)
    {
        //Scaling

        PresentCanvas[0].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 150, heightOfScreen * 0.0009f * 150);
        PresentCanvas[0].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 360, heightOfScreen * 0.0009f * 45);//0.00050505050f  0.00092592592f
        PresentCanvas[0].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 360, heightOfScreen * 0.0009f * 45);
        PresentCanvas[0].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 360, heightOfScreen * 0.0009f * 45);

        PresentCanvas[1].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 2000, heightOfScreen * 0.0009f * 1500);//x1980
        PresentCanvas[1].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[1].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 525, heightOfScreen * 0.0009f * 75);
        PresentCanvas[1].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 525, heightOfScreen * 0.0009f * 75);
        PresentCanvas[1].transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 525, heightOfScreen * 0.0009f * 75);
        PresentCanvas[1].transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 525, heightOfScreen * 0.0009f * 75);
        PresentCanvas[1].transform.GetChild(6).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 525, heightOfScreen * 0.0009f * 75);
        PresentCanvas[1].transform.GetChild(7).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 525, heightOfScreen * 0.0009f * 75);

        PresentCanvas[2].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1155, heightOfScreen * 0.0009f * 862.5f);
        PresentCanvas[2].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 375, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 375, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(6).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 375, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(7).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(8).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 375, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(9).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(10).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 375, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(11).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(12).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 375, heightOfScreen * 0.0009f * 75);
        PresentCanvas[2].transform.GetChild(13).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 375, heightOfScreen * 0.0009f * 75);

        PresentCanvas[3].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1155, heightOfScreen * 0.0009f * 862.5f);
        PresentCanvas[3].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 410, heightOfScreen * 0.0009f * 75);//x390
        PresentCanvas[3].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 45);
        PresentCanvas[3].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 390, heightOfScreen * 0.0009f * 75);
        PresentCanvas[3].transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 20);

        PresentCanvas[4].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1155, heightOfScreen * 0.0009f * 862.5f);

        PresentCanvas[5].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1155, heightOfScreen * 0.0009f * 862.5f);
        PresentCanvas[5].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[5].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 45);
        PresentCanvas[5].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[5].transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 45);
        PresentCanvas[5].transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[5].transform.GetChild(6).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 45);
        PresentCanvas[5].transform.GetChild(7).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[5].transform.GetChild(8).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 45);

        PresentCanvas[6].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1155, heightOfScreen * 0.0009f * 862.5f);
        PresentCanvas[6].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[6].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 20);
        PresentCanvas[6].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[6].transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 20);
        PresentCanvas[6].transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 300, heightOfScreen * 0.0009f * 75);
        PresentCanvas[6].transform.GetChild(6).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 20);
        PresentCanvas[6].transform.GetChild(7).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 320, heightOfScreen * 0.0009f * 75);//x300
        PresentCanvas[6].transform.GetChild(8).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 20);

        PresentCanvas[7].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 930, heightOfScreen * 0.0009f * 660);
        PresentCanvas[7].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 570, heightOfScreen * 0.0009f * 75);
        PresentCanvas[7].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 45);
        PresentCanvas[7].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 240, heightOfScreen * 0.0009f * 45);

        PresentCanvas[8].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1080, heightOfScreen * 0.0009f * 900);
        PresentCanvas[8].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 360, heightOfScreen * 0.0009f * 45);
        PresentCanvas[8].transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 360, heightOfScreen * 0.0009f * 45);
        PresentCanvas[8].transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1041.5f, heightOfScreen * 0.0009f * 798.0662f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1043, heightOfScreen * 0.0009f * 1134.829f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(5).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(6).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(7).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(8).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(9).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(10).GetComponent<RectTransform>().sizeDelta = new Vector2(widthOfScreen * 0.0005f * 1000, heightOfScreen * 0.0009f * 75);

        //Positioning

        PresentCanvas[0].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 0);
        PresentCanvas[0].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005050505f * 0, heightOfScreen * 0.00092592592f * 0);
        PresentCanvas[0].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * -60);
        PresentCanvas[0].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * -120);

        PresentCanvas[1].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 0);
        PresentCanvas[1].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -679.5f, heightOfScreen * 0.0009f * 405);
        PresentCanvas[1].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -577.5f, heightOfScreen * 0.0009f * 315);
        PresentCanvas[1].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -577.5f, heightOfScreen * 0.0009f * 240);
        PresentCanvas[1].transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -577.5f, heightOfScreen * 0.0009f * 165);
        PresentCanvas[1].transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -577.5f, heightOfScreen * 0.0009f * 90);
        PresentCanvas[1].transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -577.5f, heightOfScreen * 0.0009f * 15);
        PresentCanvas[1].transform.GetChild(7).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -577.5f, heightOfScreen * 0.0009f * -60);

        PresentCanvas[2].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 262.5f, heightOfScreen * 0.0009f * 0);
        PresentCanvas[2].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -24, heightOfScreen * 0.0009f * 273);
        PresentCanvas[2].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 636, heightOfScreen * 0.0009f * 292.5f);
        PresentCanvas[2].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -24, heightOfScreen * 0.0009f * 198);
        PresentCanvas[2].transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 636, heightOfScreen * 0.0009f * 219);
        PresentCanvas[2].transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -24, heightOfScreen * 0.0009f * 123);
        PresentCanvas[2].transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 636, heightOfScreen * 0.0009f * 144);
        PresentCanvas[2].transform.GetChild(7).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -24, heightOfScreen * 0.0009f * 48);
        PresentCanvas[2].transform.GetChild(8).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 636, heightOfScreen * 0.0009f * 69);
        PresentCanvas[2].transform.GetChild(9).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -24, heightOfScreen * 0.0009f * -27);
        PresentCanvas[2].transform.GetChild(10).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 636, heightOfScreen * 0.0009f * -6);
        PresentCanvas[2].transform.GetChild(11).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -24, heightOfScreen * 0.0009f * -102);
        PresentCanvas[2].transform.GetChild(12).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 636, heightOfScreen * 0.0009f * -81);
        PresentCanvas[2].transform.GetChild(13).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 262.5f, heightOfScreen * 0.0009f * -156);

        PresentCanvas[3].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 262.5f, heightOfScreen * 0.0009f * 0);
        PresentCanvas[3].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 25, heightOfScreen * 0.0009f * 292.5f);//x15
        PresentCanvas[3].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 564, heightOfScreen * 0.0009f * 313.5f);
        PresentCanvas[3].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 15, heightOfScreen * 0.0009f * 217.5f);
        PresentCanvas[3].transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 561, heightOfScreen * 0.0009f * 237);

        PresentCanvas[4].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 262.5f, heightOfScreen * 0.0009f * 0);

        PresentCanvas[5].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 262.5f, heightOfScreen * 0.0009f * 0);
        PresentCanvas[5].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -30, heightOfScreen * 0.0009f * 292.5f);
        PresentCanvas[5].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 564, heightOfScreen * 0.0009f * 313.5f);
        PresentCanvas[5].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -30, heightOfScreen * 0.0009f * 217.5f);
        PresentCanvas[5].transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 564, heightOfScreen * 0.0009f * 237);
        PresentCanvas[5].transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -30, heightOfScreen * 0.0009f * 142.5f);
        PresentCanvas[5].transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 564, heightOfScreen * 0.0009f * 162);
        PresentCanvas[5].transform.GetChild(7).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -30, heightOfScreen * 0.0009f * 67.5f);
        PresentCanvas[5].transform.GetChild(8).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 564, heightOfScreen * 0.0009f * 87);

        PresentCanvas[6].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 262.5f, heightOfScreen * 0.0009f * 0);
        PresentCanvas[6].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -30, heightOfScreen * 0.0009f * 292.5f);
        PresentCanvas[6].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 561, heightOfScreen * 0.0009f * 312);
        PresentCanvas[6].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -30, heightOfScreen * 0.0009f * 217.5f);
        PresentCanvas[6].transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 561, heightOfScreen * 0.0009f * 237);
        PresentCanvas[6].transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -30, heightOfScreen * 0.0009f * 142.5f);
        PresentCanvas[6].transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 561, heightOfScreen * 0.0009f * 162);
        PresentCanvas[6].transform.GetChild(7).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -25, heightOfScreen * 0.0009f * 67.5f);//x-30
        PresentCanvas[6].transform.GetChild(8).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 561, heightOfScreen * 0.0009f * 87);

        PresentCanvas[7].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 0);
        PresentCanvas[7].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 142.5f);
        PresentCanvas[7].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -150, heightOfScreen * 0.0009f * -103.2f);
        PresentCanvas[7].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 150, heightOfScreen * 0.0009f * -103.2f);

        PresentCanvas[8].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 0);
        PresentCanvas[8].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -340, heightOfScreen * 0.0009f * -410);
        PresentCanvas[8].transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 340, heightOfScreen * 0.0009f * -410);
        PresentCanvas[8].transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0.75f, heightOfScreen * 0.0009f * 31);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * -0.75f, heightOfScreen * 0.0009f * -180);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 510.4f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 410.4f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 310.4f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 210.4f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 110.4f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * 10.4f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * -89.6f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(7).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * -189.6f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(8).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * -289.6f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(9).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * -389.6f);
        PresentCanvas[8].transform.GetChild(3).transform.GetChild(0).transform.GetChild(10).GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOfScreen * 0.0005f * 0, heightOfScreen * 0.0009f * -489.6f);
    }

    public void OpenCloseCanvas(string Input)
    {
        char[] delimiterChars = { ' ', ',' };

        string[] words = Input.Split(delimiterChars);

        CanvasEnabler(words);
    }

    private void CanvasEnabler(params string[] list)
    {
        PresentCanvas[int.Parse(list[0].ToString())].enabled = true;
        for (int i = 1; i < list.Length; i++)
        {
            PresentCanvas[int.Parse(list[i].ToString())].enabled = false;
        }
    }

    public void StartRebinding(string Input)
    {
        if (rebinding)
        {
            return;
        }
        rebinding = true;

        char[] delimiterChars = { ' ', ',' };

        string[] words = Input.Split(delimiterChars);

        string id = words[0].ToString();
        string idButton = words[1].ToString();
        string idBinding = words[2].ToString();

        StartRebindObject[int.Parse(idButton)].SetActive(false);

        _playerInput.SwitchCurrentActionMap("Menu");// makes the player action map into "Menu" //THIS SHOULD RUN EVERYTIME YOU PAUSE!!! //WE MIGHT NEED AND EMPTY VERSION

        _rebindingOperation = ActionsFromActionInputs[int.Parse(id)].action.PerformInteractiveRebinding(int.Parse(idBinding))
            .WithControlsExcluding("Mouse/delta")
            .WithControlsExcluding("Mouse/position")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)    
            .OnComplete((X) => RebindComplete(Input))
            .OnCancel((X) => RebindComplete(Input))
            .Start();
    }

    private void RebindComplete(string Input)
    {
        char[] delimiterChars = { ' ', ',' };

        string[] words = Input.Split(delimiterChars);

        string id = words[0].ToString();
        string idButton = words[1].ToString();
        string idBinding = words[2].ToString();

        RebindText[int.Parse(idButton)].text = InputControlPath.ToHumanReadableString(
            ActionsFromActionInputs[int.Parse(id)].action.bindings[int.Parse(idBinding)].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);// so that it says "H" not "Keyboard H"

        _rebindingOperation.Dispose();

        StartRebindObject[int.Parse(idButton)].SetActive(true);

        _playerInput.SwitchCurrentActionMap("Humanoid");// makes the player action map into "Humanoid" <== this should depend on what the player was currently doing //THIS SHOULD RUN EVERYTIME YOU UNPAUSE!!!

        rebinding = false;

        PlayerPrefs.SetString("Rebinds", _playerInput.actions.SaveBindingOverridesAsJson());
        //_playerInput.actions.LoadBindingOverridesFromJson(_playerInput.actions.SaveBindingOverridesAsJson());
        //UpdateRebindText();
    }

    public string GetCommandLineArgument(string name)//Gets a command line argument
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    public void StartGame()//or start host, make a game
    {
        if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
        {   //just start the game solo
            manager.StartHost();
            _discovery.AdvertiseServer();
        }
    }

    public void FindServers()
    {
        discoveredServers.Clear();
        ShowServers();
        _discovery.StartDiscovery();//start finding servers to join
    }

    public void ShowServers()
    {
        for (int j = 0; j < 10; j++)
        {
            ServerJoinImages[j].enabled = false;
            ServerJoinText[j].text = " ";
        }
        for (int i = 0; i < discoveredServers.Count; i++)
        {
            ServerJoinImages[i].enabled = true;
            ServerJoinText[i].text = " Hello";//change this to number of players?
        }
    }

    public void JoinServer(int s)
    {
        manager.StartClient(discoveredServers[s].uri);
    }

    public void OnFailedToConnect()
    {
        FindServers();
        OpenCloseCanvas("8 1");
    }

    public void StartGameSolo()
    {
        GameObject player = Instantiate(_soloPlayer, new Vector3(0, 0, 0), Quaternion.identity);
        _SoloPlayer = player.transform;
        GameObject Spike = Instantiate(_soloEnemy, new Vector3(0, 0, 0), Quaternion.identity);
        _SoloEnemy = Spike.transform;

        StartCoroutine(CountDown());


        //_SoloEnemy[0] = Instantiate(_soloEnemy, new Vector3(1.29f, 6.2f, 0), Quaternion.identity).transform;
        //_SoloEnemy[1] = Instantiate(_soloEnemy, new Vector3(-1.29f, 6.2f, 0), Quaternion.identity).transform;
    }

    public void OnDiscoveredServer(ServerResponse info)// if discovered something, join that
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        // cheak if joining the right game
        //open a button and if that button is pressed it will join ,IF DISCOVERED SERVERS IS HIGHER THAN 8 THEN ACTIVATE SCROLL BAR
        //PresentCanvas[8].enabled = false;
        //ServersCanJoin 
        discoveredServers.Add(info);
        ShowServers();

        _scrollRect.vertical = (discoveredServers.Count > 7);
        //manager.StartClient(info.uri);
    }

    public void Back()
    {
        if (inGame)
        {
            _playerInput.SwitchCurrentActionMap("Humanoid");
            PresentCanvas[0].enabled = false;
        }
    }

    public void OutClick()
    {
        if (!inGame)
        {
            OpenCloseCanvas("0 1 2 3 4 5 6 7 8");
        }
    }

    public void DefaultBindings()
    {
        _playerInput.actions.LoadBindingOverridesFromJson(string.Empty);
        PlayerPrefs.SetString("Rebinds", _playerInput.actions.SaveBindingOverridesAsJson());
        UpdateRebindText();
    }

    public void CameraInversionChanged()
    {
        PlayerPrefs.SetInt("SavedCameraMISettings", PresentDropdowns[0].value);
        //PresentDropdowns[0].value;
        //get the cinemachine and implement change
    }

    public void CameraSpeedChanged()
    {
        PlayerPrefs.SetFloat("SavedCameraMSSettings", PresentSliders[0].value);
        //PresentSliders[0].value;
        //get the cinemachine and implement change
    }

    public void ScreenModeChanged()
    {
        PlayerPrefs.SetInt("SavedScreenModeSettings", PresentDropdowns[1].value);
        ChangeScreenMode(PresentDropdowns[1].value);
    }

    public void ScreenResolutionChanged()
    {
        PlayerPrefs.SetInt("SavedResolutionSettings", PresentDropdowns[2].value);
        ChangeResolution(PresentDropdowns[2].value);
    }

    public void VsyncChanged()
    {
        PlayerPrefs.SetInt("SavedVSyncSettings", PresentDropdowns[3].value);
        QualitySettings.vSyncCount = PresentDropdowns[3].value;
    }

    public void AntiAliasingChanged()
    {
        PlayerPrefs.SetInt("SavedAntiAliasingSettings", PresentDropdowns[4].value);
        ChangeAntiAliasing(PresentDropdowns[4].value);
    }

    public void GeneralVolumeChanged()
    {
        PlayerPrefs.SetFloat("SavedGeneralVolumeSettings", PresentSliders[1].value);
        PresentMusic[0].volume = PresentSliders[2].value * PresentSliders[1].value;
        PresentMusic[1].volume = PresentSliders[3].value * PresentSliders[1].value;
        PresentMusic[2].volume = PresentSliders[4].value * PresentSliders[1].value;
    }

    public void MusicVolumeChanged()
    {
        PlayerPrefs.SetFloat("SavedMusicVolumeSettings", PresentSliders[2].value);
        PresentMusic[0].volume = PresentSliders[2].value * PresentSliders[1].value;
    }

    public void AmbienceVolumeChanged()
    {
        PlayerPrefs.SetFloat("SavedAmbientVolumeSettings", PresentSliders[3].value);
        PresentMusic[1].volume = PresentSliders[3].value * PresentSliders[1].value;
    }

    public void SFXVolumeChanged()
    {
        PlayerPrefs.SetFloat("SavedSoundFXVolumeSettings", PresentSliders[4].value);
        PresentMusic[2].volume = PresentSliders[4].value * PresentSliders[1].value;
    }

    private void ChangeScreenMode(int id)
    {
        if (id == 0)
        {
            Fullscreen = true;
        }
        else
        {
            Fullscreen = false;
        }
        Screen.SetResolution(WidthResolution, HeightResolution, Fullscreen);
    }

    private void ChangeResolution(int id)
    {
        if (id == 0)
        {
            WidthResolution = 1920;
            HeightResolution = 1080;
        }
        else if (id == 1)
        {
            WidthResolution = 1768;
            HeightResolution = 992;
        }
        else if (id == 2)
        {
            WidthResolution = 1680;
            HeightResolution = 1050;
        }
        else if (id == 3)
        {
            WidthResolution = 1600;
            HeightResolution = 1024;
        }
        else if (id == 4)
        {
            WidthResolution = 1600;
            HeightResolution = 900;
        }
        else if (id == 5)
        {
            WidthResolution = 1440;
            HeightResolution = 900;
        }
        else if (id == 6)
        {
            WidthResolution = 1366;
            HeightResolution = 768;
        }
        else if (id == 7)
        {
            WidthResolution = 1360;
            HeightResolution = 768;
        }
        else if (id == 8)
        {
            WidthResolution = 1280;
            HeightResolution = 1024;
        }
        else if (id == 9)
        {
            WidthResolution = 1280;
            HeightResolution = 960;
        }
        else if (id == 10)
        {
            WidthResolution = 1280;
            HeightResolution = 800;
        }
        else if (id == 11)
        {
            WidthResolution = 1280;
            HeightResolution = 768;
        }
        else if (id == 12)
        {
            WidthResolution = 1280;
            HeightResolution = 720;
        }
        else if (id == 13)
        {
            WidthResolution = 1152;
            HeightResolution = 864;
        }
        else if (id == 14)
        {
            WidthResolution = 1024;
            HeightResolution = 768;
        }
        else
        {
            WidthResolution = 1920;
            HeightResolution = 1080;
        }
        Screen.SetResolution(WidthResolution, HeightResolution, Fullscreen);
        UpdateUIArrangement(WidthResolution, HeightResolution);
    }

    private void ChangeAntiAliasing(int id)
    {
        if (id == 0)
        {
            CameraRenderer.antialiasing = AntialiasingMode.None;
        }
        else if (id == 1)
        {
            CameraRenderer.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
        }
        else if (id == 2)
        {
            CameraRenderer.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        }
        else
        {
            CameraRenderer.antialiasing = AntialiasingMode.None;
        }
    }

    private void UpdateRebindText()
    {
        RebindText[0].text = InputControlPath.ToHumanReadableString(
        ActionsFromActionInputs[0].action.bindings[1].effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice);// so that it updates the text of the rebinds

        RebindText[1].text = InputControlPath.ToHumanReadableString(
        ActionsFromActionInputs[0].action.bindings[2].effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice);

        RebindText[2].text = InputControlPath.ToHumanReadableString(
        ActionsFromActionInputs[0].action.bindings[3].effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice);

        RebindText[3].text = InputControlPath.ToHumanReadableString(
        ActionsFromActionInputs[0].action.bindings[4].effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice);

        RebindText[4].text = InputControlPath.ToHumanReadableString(
        ActionsFromActionInputs[1].action.bindings[0].effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice);

        RebindText[5].text = InputControlPath.ToHumanReadableString(
        ActionsFromActionInputs[2].action.bindings[0].effectivePath,
        InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void LoadScene(int SceneANumber)//this might not be needed
    {
        SceneManager.LoadScene(SceneANumber);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator CountDown()//i think all this should be inside the Transformation GameObject
    {
        yield return new WaitForSeconds(1f);
        Count.text = (--Countdown).ToString();
        if (Countdown == 0)
        {
            Countdown = 11;
            //startmoving
            TranformationBlastAnim.SetBool("Expand", true);
            StartCoroutine(DoneEpandAnimation());
        }
        StartCoroutine(CountDown());
    }

    private IEnumerator DoneEpandAnimation()
    {
        yield return new WaitForSeconds(2f);
        //TranformationBlastAnim.SetBool("Expand", false);// this should instanly make it small
        //TranformationBlastRot.SetFloat("AnimationSpeed", Random.Range(-2.5000f, 2.5000f));

        //float angle = Random.Range(-360.0000f, 360.0000f);
        //Blast.position = new Vector2(6.83f * Mathf.Cos(angle), 6.83f * Mathf.Sin(angle));
        //set real sprite to transformed sprite
        //set transformed sprite to next transform sprite
    }
}
