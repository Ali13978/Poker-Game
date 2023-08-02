using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
public class MainMenuUI : MonoBehaviour
{
    #region Singleton
    public static MainMenuUI instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Login-Pannel")]
    [SerializeField] GameObject loginPannel;
    [SerializeField] Slider loginSlider;

    [Header("SetName-Pannel")]
    [SerializeField] GameObject setNamePannel;
    [SerializeField] TMP_InputField setNameInputField;
    [SerializeField] Button setNameDoneBtn;

    [Header("Mainmenu-Pannel")]
    [SerializeField] GameObject mainMenuPannel;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] Button startGameBtn;
    [SerializeField] Button ProfileBtn;

    [Header("StartGame-Pannel")]
    [SerializeField] GameObject startGamePannel;
    [SerializeField] Button createRoomBtn;
    [SerializeField] Button joinRoomBtn;
    [SerializeField] Button startGamePannelBackBtn;

    [Header("PlayerProfile-Pannel")]
    [SerializeField] GameObject playerProfilePannel;
    [SerializeField] Sprite defaultPlayerSprite;
    [SerializeField] Image profilePlayerImage;
    [SerializeField] Button profileEditImageBtn;
    [SerializeField] TMP_Text profilePlayerNameText;
    [SerializeField] Button profileEditPlayerNameBtn;
    [SerializeField] TMP_Text profilePlayerIdText;
    [SerializeField] Button profileBackBtn;

    #region login-Pannel

    public IEnumerator StartLoginLoading()
    {
        float value = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < 2.3f)
        {
            float t = elapsedTime / 2.3f;

            value = Mathf.Lerp(0.1f, 0.8f, t);

            loginSlider.value = value;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (PlayerPrefs.HasKey("playerName"))
            EnableMainMenuPannel();
        else
            EnableSetNamePannel();
    }
    #endregion

    #region OpenImage

    private Sprite OpenImage()
    {
#if UNITY_ANDROID
        // On Android, use native file picker to get the image path
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        // Create an intent for file selection
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setType", "image/*");
        intent.Call<AndroidJavaObject>("setAction", "android.intent.action.GET_CONTENT");

        // Start the file picker activity and wait for result
        AndroidJavaObject chooser = intent.CallStatic<AndroidJavaObject>("createChooser", intent, "Select Image");
        AndroidJavaObject uri = currentActivity.Call<AndroidJavaObject>("startActivityForResult", chooser, 1);

        // Convert the selected image URI to a string
        string imagePath = uri.Call<string>("toString");
#else
        // On other platforms, use Unity's built-in file dialog
        string imagePath = UnityEditor.EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");
#endif

        // Load the image at the selected path as a Texture2D
        if (!string.IsNullOrEmpty(imagePath))
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        return defaultPlayerSprite;
    }
    #endregion

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        //Login Pannel
        TurnOffAllPannels();
        loginPannel.SetActive(true);

        //Main Menu Pannel
        startGameBtn.onClick.AddListener(() =>
        {
            startGamePannel.SetActive(true);
        });

        ProfileBtn.onClick.AddListener(() =>
        {
            EnablePlayerProfilePannel();
        });

        //Start Game Pannel
        startGamePannelBackBtn.onClick.AddListener(() =>
        {
            startGamePannel.SetActive(false);
        });

        createRoomBtn.onClick.AddListener(() =>
        {
            NetworkConnectorHandler.CreateGame(NetworkConnectorType.UnityRelay);
        });

        joinRoomBtn.onClick.AddListener(() =>
        {
            NetworkConnectorHandler.JoinGame(NetworkConnectorType.UnityRelay);
        });

        //Set Name Pannel
        setNameDoneBtn.onClick.AddListener(() =>
        {
            Debug.Log(setNameInputField.text);
            if (string.IsNullOrEmpty(setNameInputField.text))
                return;
            PlayerPrefs.SetString("playerName", setNameInputField.text);
            EnableMainMenuPannel();
        });

        //Player Profile Pannel
        profileEditPlayerNameBtn.onClick.AddListener(() =>
        {
            EnableSetNamePannel();
        });

        profileBackBtn.onClick.AddListener(() =>
        {
            EnableMainMenuPannel();
        });
    }

    private void TurnOffAllPannels()
    {
        loginPannel.SetActive(false);
        mainMenuPannel.SetActive(false);
        setNamePannel.SetActive(false);
        playerProfilePannel.SetActive(false);
    }

    private void EnableMainMenuPannel()
    {
        TurnOffAllPannels();
        mainMenuPannel.SetActive(true);
        playerNameText.text = PlayerPrefs.GetString("playerName");
    }

    private void EnableSetNamePannel()
    {
        TurnOffAllPannels();
        setNamePannel.SetActive(true);
    }

    private void EnablePlayerProfilePannel()
    {
        TurnOffAllPannels();
        playerProfilePannel.SetActive(true);

        profilePlayerNameText.text = "Player Name: " + PlayerPrefs.GetString("playerName");
        profilePlayerIdText.text = "Player Id: " + AuthenticationService.Instance.PlayerId;

        Debug.Log(AuthenticationService.Instance.PlayerInfo);
    }
}
