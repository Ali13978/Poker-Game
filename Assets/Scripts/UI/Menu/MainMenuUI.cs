using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Core;
using SFB;
#if UNITY_ANDROID || UNITY_IOS
using NativeFilePickerNamespace;
#endif

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

    [Header("Loading-Pannel")]
    [SerializeField] GameObject loadingPannel;

    [Header("Mainmenu-Pannel")]
    [SerializeField] GameObject mainMenuPannel;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] Image playerImage;
    [SerializeField] Button startGameBtn;
    [SerializeField] Button ProfileBtn;

    [Header("StartGame-Pannel")]
    [SerializeField] GameObject startGamePannel;
    [SerializeField] Button createRoomBtn;
    [SerializeField] Button joinRoomBtn;
    [SerializeField] TMP_InputField joinCodeInput;
    [SerializeField] Button startGamePannelBackBtn;

    [Header("PlayerProfile-Pannel")]
    private ISaveLoadSystem _saveLoadSystem;
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

        PlayerData playerData = _saveLoadSystem.Load<PlayerData>();

        if (playerData.Equals(default(PlayerData)) == true)
            EnableMainMenuPannel();
        else
            EnableSetNamePannel();
    }
    #endregion

    #region OpenImage

    public Sprite OpenImage()
    {
        // Set the filters for allowed file extensions
        ExtensionFilter[] extensions = new ExtensionFilter[]
        {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
        };

#if UNITY_ANDROID && !UNITY_EDITOR
        string[] paths = new string[1];
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
                Debug.Log("Operation cancelled");
            else
            {
                paths[0] = path;
                Debug.Log("Picked file: " + path);
            }
        }, new string[] { "image/*" });
        Debug.Log(paths[0]);
#else
        // On other platforms, use the default standalone file browser
        //string[] paths = new string[1];
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Profile Photo", "C:/", extensions, false);
        //NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        //{
        //    if (path == null)
        //        Debug.Log("Operation cancelled");
        //    else
        //    {
        //        paths[0] = path;
        //        Debug.Log("Picked file: " + path);
        //    }
        //}, new string[] { "image/*" });
        //Debug.Log(paths[0]);
#endif

        // Check if a file was selected
        if (!string.IsNullOrEmpty(paths[0]))
        {
            // Load the selected image file as a byte array
            byte[] imageData = File.ReadAllBytes(paths[0]);

            // Create a new texture from the image data
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            // Convert the texture to a Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            return sprite;
        }

        return defaultPlayerSprite; // Return null if no image was selected
    }
#endregion

    #region Bytes to Texture
    public static Texture2D BytesToTexture2D(byte[] byteArray)
    {
        Texture2D texture = new Texture2D(2, 2); // Create a new 2x2 texture, we'll replace it with the decoded image.

        if (byteArray != null && byteArray.Length > 0)
        {
            // Load the image data into the texture
            bool success = ImageConversion.LoadImage(texture, byteArray);

            if (!success)
            {
                Debug.LogError("Failed to convert byte array to Texture2D.");
                return null;
            }
        }

        return texture;
    }

    public static Sprite BytesToSprite(byte[] byteArray)
    {
        Texture2D texture = BytesToTexture2D(byteArray);

        if (texture != null)
        {
            // Create a sprite using the texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            return sprite;
        }
        else
        {
            Debug.LogError("Failed to convert byte array to Sprite.");
            return null;
        }
    }
#endregion

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        _saveLoadSystem = ReadonlySaveLoadSystemFactory.Instance.Get();

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
            try
            {
                EnableLoadingPannel();
                NetworkConnectorHandler.CreateGame(NetworkConnectorType.UnityRelay);
            }
            catch (RelayServiceException e)
            {
                EnableMainMenuPannel();
                Debug.Log(e);
            }
        });

        joinRoomBtn.onClick.AddListener(() =>
        {
            try
            {
                if (string.IsNullOrEmpty(joinCodeInput.text))
                    return;
                EnableLoadingPannel();
                startGamePannel.SetActive(false);
                NetworkConnectorHandler.JoinGame(NetworkConnectorType.UnityRelay);
            }
            catch(RelayServiceException e)
            {
                EnableMainMenuPannel();
                Debug.Log(e);
            }
        });

        //Set Name Pannel
        setNameDoneBtn.onClick.AddListener(() =>
        {
            Debug.Log(setNameInputField.text);
            if (string.IsNullOrEmpty(setNameInputField.text))
                return;
        PlayerPrefs.SetString("playerName", setNameInputField.text);
            PlayerData playerData = new (setNameInputField.text);
        _saveLoadSystem.Save(playerData);
        EnableMainMenuPannel();
        });

        //Player Profile Pannel
        profileEditPlayerNameBtn.onClick.AddListener(() =>{

            EnableSetNamePannel();
        });

        profileEditImageBtn.onClick.AddListener(() => {
            Sprite sprite = OpenImage();
            if (sprite == null)
                return;

            byte[] rawTexture = TextureConverter.GetRawTexture(sprite.texture);
            PlayerAvatarData avatarData = new (rawTexture);

            _saveLoadSystem.Save(avatarData);
            EnableMainMenuPannel();
        });

        profileBackBtn.onClick.AddListener(() => {

            EnableMainMenuPannel();
        });
    }

    private void TurnOffAllPannels()
    {
        loginPannel.SetActive(false);
        loadingPannel.SetActive(false);
        mainMenuPannel.SetActive(false);
        setNamePannel.SetActive(false);
        playerProfilePannel.SetActive(false);
    }

    private void EnableMainMenuPannel()
    {
        TurnOffAllPannels();
        mainMenuPannel.SetActive(true);
        PlayerData playerData = _saveLoadSystem.Load<PlayerData>();
        PlayerAvatarData playerAvatarData = _saveLoadSystem.Load<PlayerAvatarData>();

        playerImage.sprite = BytesToSprite(playerAvatarData.CodedValue);
        playerNameText.text = playerData.NickName;
    }

    private void EnableSetNamePannel()
    {
        TurnOffAllPannels();
        setNamePannel.SetActive(true);
    }
    private void EnableLoadingPannel()
    {
        TurnOffAllPannels();
        loadingPannel.SetActive(true);
    }

    private void EnablePlayerProfilePannel()
    {
        TurnOffAllPannels();
        playerProfilePannel.SetActive(true);

        PlayerData playerData = _saveLoadSystem.Load<PlayerData>();
        PlayerAvatarData playerAvatarData = _saveLoadSystem.Load<PlayerAvatarData>();

        profilePlayerNameText.text = "Player Name: " + playerData.NickName;
        profilePlayerIdText.text = "Player Id: " + AuthenticationService.Instance.PlayerId;
        profilePlayerImage.sprite = BytesToSprite(playerAvatarData.CodedValue);

        Debug.Log(AuthenticationService.Instance.PlayerInfo);
    }
}
