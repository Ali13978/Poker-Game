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

    private enum PanelType
    {
        loginPannel,
        setNamePannel,
        loadingPannel,
        mainMenuPannel,
        playerProfilePannel,
        lobbyPannel
    };

    private enum MenuPannelType
    {
        mainPannel,
        tournamentPannel
    };

    private Dictionary<PanelType, GameObject> panelsDictionary;
    private Dictionary<MenuPannelType, GameObject> menuPanelsDictionary;

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
    [SerializeField] GameObject mainPannel;
    [SerializeField] GameObject TournamentsPannel;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] Image playerImage;
    [SerializeField] TMP_Text playerChipsText;
    [SerializeField] Button startGameBtn;
    [SerializeField] Button quickJoinBtn;
    [SerializeField] Button ProfileBtn;
    [SerializeField] Button mainmenuBtn;
    [SerializeField] Button TournamentsBtn;
    private Action enableMainMenuAction;

    [Header("Tournaments Pannel")]
    [SerializeField] Button tournamentABtn;
    [SerializeField] TMP_Text tournamentABtnText;
    [SerializeField] TMP_Text tournamentACurrentStageText;
    [SerializeField] uint TournamentAEntryFee;
    [SerializeField] Button tournamentBBtn;
    [SerializeField] TMP_Text tournamentBBtnText;
    [SerializeField] TMP_Text tournamentBCurrentStageText;
    [SerializeField] uint TournamentBEntryFee;
    private Action enableTournamentPannelAction;

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
    private Action enablePlayerProfilePannelAction;

    [Header("Lobby-Pannel")]
    [SerializeField] GameObject lobbyPannel;
    [SerializeField] GameObject PlayerInLobbyPrefab;
    [SerializeField] TMP_Text lobbyCodeText;
    [SerializeField] TMP_Text lobbyNumberOfPlayersText;
    [SerializeField] Transform lobbyPlayersHolder;
    //[SerializeField] Button lobbyStartGameBtn;
    [SerializeField] Button lobbyLeaveBtn;
    [SerializeField] TMP_Text lobbyTimerText;
    [SerializeField] int minPlayersReq;
    [SerializeField] float lobbyStartGameTimer = 60f;
    private bool isLobbyTimerStarted;
    public Action lobbyPlayersEditedAction;
    private Action enableLobbyPannelAction;
    public Action<string> joinRelayAction;
    string relayCode = null;

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

        if (!playerData.Equals(default(PlayerData)))
        {
            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
        }
        else
            EnablePannel(PanelType.setNamePannel, () => { });
    }
    #endregion

    #region OpenImage

    private Sprite OpenImage()
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
    private static Texture2D BytesToTexture2D(byte[] byteArray)
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

    private static Sprite BytesToSprite(byte[] byteArray)
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
        panelsDictionary = new Dictionary<PanelType, GameObject>
        {
            { PanelType.loginPannel, loginPannel },
            { PanelType.setNamePannel, setNamePannel },
            { PanelType.loadingPannel, loadingPannel },
            { PanelType.mainMenuPannel, mainMenuPannel },
            { PanelType.playerProfilePannel, playerProfilePannel },
            { PanelType.lobbyPannel, lobbyPannel }
        };

        menuPanelsDictionary = new Dictionary<MenuPannelType, GameObject>
        {
            {MenuPannelType.mainPannel, mainPannel },
            {MenuPannelType.tournamentPannel, TournamentsPannel }
        };

        await UnityServices.InitializeAsync();
        _saveLoadSystem = ReadonlySaveLoadSystemFactory.Instance.Get();

        //Initializing All Actions
        InitializeActions();

        //Login Pannel
        EnablePannel(PanelType.loginPannel, () => { });

        //Main Menu Pannel
        mainmenuBtn.onClick.AddListener(() => {
            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
        });

        TournamentsBtn.onClick.AddListener(() => {
            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            EnableMenuPannel(MenuPannelType.tournamentPannel, enableTournamentPannelAction);
        });

        startGameBtn.onClick.AddListener(() =>
        {
            startGamePannel.SetActive(true);
        });

        quickJoinBtn.onClick.AddListener(() => {
            EnablePannel(PanelType.loadingPannel, () => { });

            LobbyManager.instance.QuickJoinLobby(() => {
                EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            }, () => {
                EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            });
        });

        ProfileBtn.onClick.AddListener(() =>
        {
            EnablePannel(PanelType.playerProfilePannel, enablePlayerProfilePannelAction);
        });
        
        //Start Game Pannel
        startGamePannelBackBtn.onClick.AddListener(() =>
        {
            startGamePannel.SetActive(false);
        });

        createRoomBtn.onClick.AddListener(() =>
        {
            //try
            //{
            //    EnablePannel(PanelType.loadingPannel, () => { });
            //    startGamePannel.SetActive(false);

            //    NetworkConnectorHandler.CreateGame(NetworkConnectorType.UnityRelay);
            //}
            //catch (RelayServiceException e)
            //{
            //    EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            //    Debug.Log(e);
            //}


            EnablePannel(PanelType.loadingPannel, () => { });
            startGamePannel.SetActive(false);

            LobbyManager.instance.CreateLobby(true, () => {
                EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            }, () => {
                EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            });
        });

        joinRoomBtn.onClick.AddListener(() =>
        {
            //try
            //{
            //    if (string.IsNullOrEmpty(joinCodeInput.text))
            //        return;
            //        EnablePannel(PanelType.loadingPannel, () => { });
            //        startGamePannel.SetActive(false);
            //        NetworkConnectorHandler.JoinGame(NetworkConnectorType.UnityRelay);
            //    }
            //    catch(RelayServiceException e)
            //    {
            //        EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            //        Debug.Log(e);
            //    }

            if (string.IsNullOrEmpty(joinCodeInput.text))
                return;

            EnablePannel(PanelType.loadingPannel, () => { });
            startGamePannel.SetActive(false);

            LobbyManager.instance.JoinLobby(joinCodeInput.text, () => {
                EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            }, () => {
                EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            });
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

            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
        });

        //Player Profile Pannel
        profileEditPlayerNameBtn.onClick.AddListener(() =>{

            EnablePannel(PanelType.setNamePannel, () => { });
        });

        profileEditImageBtn.onClick.AddListener(() => {
            Sprite sprite = OpenImage();
            if (sprite == null)
                return;

            byte[] rawTexture = TextureConverter.GetRawTexture(sprite.texture);
            PlayerAvatarData avatarData = new (rawTexture);

            _saveLoadSystem.Save(avatarData);
            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
        });

        profileBackBtn.onClick.AddListener(() => {

            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
        });

        //Lobby Pannel

        //lobbyStartGameBtn.onClick.AddListener(() => {
        //    Debug.Log("Game Started");
        //    try
        //    {
        //        EnablePannel(PanelType.loadingPannel, () => { });
        //        startGamePannel.SetActive(false);

        //        NetworkConnectorHandler.CreateGame(NetworkConnectorType.UnityRelay);
        //    }
        //    catch (RelayServiceException e)
        //    {
        //        EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
        //        EnableMenuPannel(MenuPannelType.mainPannel, () => { });
        //        Debug.Log(e);
        //    }
        //});


        lobbyLeaveBtn.onClick.AddListener(() => {
            EnablePannel(PanelType.loadingPannel, () => { });

            LobbyManager.instance.LeaveLobby(() => {
                EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            }, () => {
                EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            });
        });
    }

    private void Update()
    {
        if (!isLobbyTimerStarted)
            return;

        HandleLobbyTimer();
    }
    
    private void HandleLobbyTimer()
    {
        Unity.Services.Lobbies.Models.Lobby joinedLobby = LobbyManager.instance.GetJoinedLobby();
        if (joinedLobby == null)
            return;
        if (joinedLobby.Players.Count < minPlayersReq)
            return;
        lobbyStartGameTimer -= Time.deltaTime;
        int Timer = (int)lobbyStartGameTimer;
        lobbyTimerText.text = Timer.ToString();
        if (lobbyStartGameTimer <= 0)
        {
            float lobbyTimerMax = 200f;
            lobbyStartGameTimer = lobbyTimerMax;
            lobbyTimerText.text = "0";

            //time elapsed do something
            lobbyTimerText.gameObject.SetActive(false);

            if (!LobbyManager.instance.IsHost())
                return;
            try
            {
                EnablePannel(PanelType.loadingPannel, () => { });

                NetworkConnectorHandler.CreateGame(NetworkConnectorType.UnityRelay);
            }
            catch (RelayServiceException e)
            {
                EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
                Debug.Log(e);
            }
        }
    }

    private void InitializeActions()
    {
        enableMainMenuAction = () => {
            PlayerData playerData = _saveLoadSystem.Load<PlayerData>();
            PlayerAvatarData playerAvatarData = _saveLoadSystem.Load<PlayerAvatarData>();
            
            playerChipsText.text = playerData.Stack.ToString();
            playerImage.sprite = BytesToSprite(playerAvatarData.CodedValue);
            playerNameText.text = playerData.NickName;

            //tornuments stuff
            TournamentAData tournamentAData = _saveLoadSystem.Load<TournamentAData>();
            TournamentBData tournamentBData = _saveLoadSystem.Load<TournamentBData>();
            if (tournamentAData == null)
                tournamentAData = new TournamentAData(TournamentAData.tournamentStage.QuarterFinal, false);
            if (tournamentBData == null)
                tournamentBData = new TournamentBData(TournamentBData.tournamentStage.QuarterFinal, false);

            if (tournamentAData.isStarted)
            {
                tournamentABtnText.text = "Continue";
                tournamentACurrentStageText.text = tournamentAData.CurrentStage.ToString();

                tournamentABtn.onClick.AddListener(() => {

                    EnablePannel(PanelType.loadingPannel, () => { });

                    LobbyManager.instance.QuickJoinLobby(() => {
                        EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
                    }, () => {
                        EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                        EnableMenuPannel(MenuPannelType.mainPannel, () => { });
                    });
                });
            }
            else
            {
                tournamentABtnText.text = "Pay n Play";
                tournamentACurrentStageText.text = "Join Now";
                tournamentABtn.onClick.AddListener(() => {

                    PlayerData _playerData = _saveLoadSystem.Load<PlayerData>();

                    if (_playerData.Stack >= TournamentAEntryFee)
                    {
                        EnablePannel(PanelType.loadingPannel, () => { });

                        uint stack = _playerData.Stack - TournamentAEntryFee;
                        PlayerData data = new PlayerData(_playerData.NickName, stack);

                        _saveLoadSystem.Save(data);
                        LobbyManager.instance.QuickJoinLobby(() => {
                            TournamentAData tournamentData = new TournamentAData(TournamentAData.tournamentStage.QuarterFinal, true);
                            _saveLoadSystem.Save(tournamentData);

                            EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
                        }, () => {
                            TournamentAData tournamentData = new TournamentAData(TournamentAData.tournamentStage.QuarterFinal, false);
                            _saveLoadSystem.Save(tournamentData);
                            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
                        });
                    }
                });
            }

            if (tournamentBData.isStarted)
            {
                tournamentBBtnText.text = "Continue";
                tournamentBCurrentStageText.text = tournamentBData.CurrentStage.ToString();
                tournamentBBtn.onClick.AddListener(() => {

                    EnablePannel(PanelType.loadingPannel, () => { });

                    LobbyManager.instance.QuickJoinLobby(() => {
                        EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
                    }, () => {
                        EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                        EnableMenuPannel(MenuPannelType.mainPannel, () => { });
                    });
                });
            }
            else
            {
                tournamentBBtnText.text = "Pay n Play";
                tournamentBCurrentStageText.text = "Join Now";
                tournamentBBtn.onClick.AddListener(() => {
                    PlayerData _playerData = _saveLoadSystem.Load<PlayerData>();

                    if (_playerData.Stack >= TournamentBEntryFee)
                    {
                        EnablePannel(PanelType.loadingPannel, () => { });

                        uint stack = _playerData.Stack - TournamentBEntryFee;
                        PlayerData data = new PlayerData(_playerData.NickName, stack);

                        _saveLoadSystem.Save(data);
                        LobbyManager.instance.QuickJoinLobby(() => {
                            TournamentBData tournamentData = new TournamentBData(TournamentBData.tournamentStage.QuarterFinal, true);
                            _saveLoadSystem.Save(tournamentData);

                            EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
                        }, () => {
                            TournamentBData tournamentData = new TournamentBData(TournamentBData.tournamentStage.QuarterFinal, false);
                            _saveLoadSystem.Save(tournamentData);
                            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
                        });
                    }
                });
            }
        };

        enablePlayerProfilePannelAction = () => {

            PlayerData playerData = _saveLoadSystem.Load<PlayerData>();
            PlayerAvatarData playerAvatarData = _saveLoadSystem.Load<PlayerAvatarData>();

            profilePlayerNameText.text = "Player Name: " + playerData.NickName;
            profilePlayerIdText.text = "Player Id: " + AuthenticationService.Instance.PlayerId;
            profilePlayerImage.sprite = BytesToSprite(playerAvatarData.CodedValue);

            Debug.Log(AuthenticationService.Instance.PlayerInfo);
        };

        enableTournamentPannelAction = () => {
            //TournamentAData tournamentAData = _saveLoadSystem.Load<TournamentAData>();
            //TournamentBData tournamentBData = _saveLoadSystem.Load<TournamentBData>();
            //if (tournamentAData == null)
            //    tournamentAData = new TournamentAData(TournamentAData.tournamentStage.QuarterFinal, false);
            //if (tournamentBData == null)
            //    tournamentBData = new TournamentBData(TournamentBData.tournamentStage.QuarterFinal, false);

            //if (tournamentAData.isStarted)
            //{
            //    tournamentABtnText.text = "Continue";
            //    tournamentACurrentStageText.text = tournamentAData.CurrentStage.ToString();

            //    tournamentABtn.onClick.AddListener(() => {

            //        EnablePannel(PanelType.loadingPannel, () => { });

            //        LobbyManager.instance.QuickJoinLobby(() => { 
            //            EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            //        }, () => {
            //            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            //            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            //        });
            //    });
            //}
            //else
            //{
            //    tournamentABtnText.text = "Pay n Play";
            //    tournamentACurrentStageText.text = "Join Now";
            //    tournamentABtn.onClick.AddListener(() => {

            //        PlayerData playerData = _saveLoadSystem.Load<PlayerData>();

            //        if (playerData.Stack >= TournamentAEntryFee)
            //        {
            //            EnablePannel(PanelType.loadingPannel, () => { });

            //            uint stack = playerData.Stack - TournamentAEntryFee;
            //            PlayerData data = new PlayerData(playerData.NickName, stack);

            //            _saveLoadSystem.Save(data);
            //            LobbyManager.instance.QuickJoinLobby(() => {
            //                TournamentAData tournamentData = new TournamentAData(TournamentAData.tournamentStage.QuarterFinal, true);
            //                _saveLoadSystem.Save(tournamentData);

            //                EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            //            }, () => {
            //                TournamentAData tournamentData = new TournamentAData(TournamentAData.tournamentStage.QuarterFinal, false);
            //                _saveLoadSystem.Save(tournamentData);
            //                EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            //                EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            //            });
            //        }
            //    });
            //}

            //if (tournamentBData.isStarted)
            //{
            //    tournamentBBtnText.text = "Continue";
            //    tournamentBCurrentStageText.text = tournamentBData.CurrentStage.ToString();
            //    tournamentBBtn.onClick.AddListener(() => {

            //        EnablePannel(PanelType.loadingPannel, () => { });

            //        LobbyManager.instance.QuickJoinLobby(() => {
            //            EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            //        }, () => {
            //            EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            //            EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            //        });
            //    });
            //}
            //else
            //{
            //    tournamentBBtnText.text = "Pay n Play";
            //    tournamentBCurrentStageText.text = "Join Now";
            //    tournamentBBtn.onClick.AddListener(() => {
            //        PlayerData playerData = _saveLoadSystem.Load<PlayerData>();

            //        if (playerData.Stack >= TournamentBEntryFee)
            //        {
            //            EnablePannel(PanelType.loadingPannel, () => { });

            //            uint stack = playerData.Stack - TournamentBEntryFee;
            //            PlayerData data = new PlayerData(playerData.NickName, stack);

            //            _saveLoadSystem.Save(data);
            //            LobbyManager.instance.QuickJoinLobby(() => {
            //                TournamentBData tournamentData = new TournamentBData(TournamentBData.tournamentStage.QuarterFinal, true);
            //                _saveLoadSystem.Save(tournamentData);

            //                EnablePannel(PanelType.lobbyPannel, enableLobbyPannelAction);
            //            }, () => {
            //                TournamentBData tournamentData = new TournamentBData(TournamentBData.tournamentStage.QuarterFinal, false);
            //                _saveLoadSystem.Save(tournamentData);
            //                EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
            //                EnableMenuPannel(MenuPannelType.mainPannel, () => { });
            //            });
            //        }
            //    });
            //}
        };

        enableLobbyPannelAction = () => {
            lobbyCodeText.text = LobbyManager.instance.GetLobbyCode();

        };

        lobbyPlayersEditedAction = () => {

            int childCount = lobbyPlayersHolder.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = lobbyPlayersHolder.GetChild(i);
                Destroy(child.gameObject);
            }

            int currentPlayersInLobby =  LobbyManager.instance.GetJoinedLobby().Players.Count;

            for (int i = 0; i < currentPlayersInLobby; i++)
            {
                Unity.Services.Lobbies.Models.Player Player = LobbyManager.instance.GetJoinedLobby().Players[i];
                GameObject player = Instantiate(PlayerInLobbyPrefab, lobbyPlayersHolder.transform);
                PlayerInRoom _player = player.GetComponent<PlayerInRoom>();

                string playerName = Player.Data["PlayerName"].Value;

                _player.UpdatePlayerUI(playerName);
                
            }

            lobbyNumberOfPlayersText.text = "Players:     " + currentPlayersInLobby + "/9";
            
            if(currentPlayersInLobby >= minPlayersReq)
            {
                Debug.Log("Timer Started");
                isLobbyTimerStarted = true;
            }

            else
            {
                lobbyTimerText.text = "Waiting for players";
            }
            //if (LobbyManager.instance.IsHost())
            //    lobbyStartGameBtn.gameObject.SetActive(true);
            //else
            //    lobbyStartGameBtn.gameObject.SetActive(false);
        };

        joinRelayAction = (newRelayCode) => {
            try
            {
                if (relayCode == newRelayCode)
                    return;

                relayCode = newRelayCode;

                if (string.IsNullOrEmpty(relayCode))
                    return;

                EnablePannel(PanelType.loadingPannel, () => { });
                NetworkConnectorHandler.JoinGame(NetworkConnectorType.UnityRelay, relayCode);
            }
            catch (RelayServiceException e)
            {
                EnablePannel(PanelType.mainMenuPannel, enableMainMenuAction);
                EnableMenuPannel(MenuPannelType.mainPannel, () => { });
                Debug.Log(e);
            }
        };
    }

    private void TurnOffAllPannels()
    {
        loginPannel.SetActive(false);
        loadingPannel.SetActive(false);
        mainMenuPannel.SetActive(false);
        setNamePannel.SetActive(false);
        playerProfilePannel.SetActive(false);
        lobbyPannel.SetActive(false);
    }

    private void TurnOffMenuPannels()
    {
        mainPannel.SetActive(false);
        TournamentsPannel.SetActive(false);
    }

    private void EnablePannel(PanelType pannel, Action onEnableAction)
    {
        if (panelsDictionary.ContainsKey(pannel))
        {
            TurnOffAllPannels();
            GameObject panel = panelsDictionary[pannel];
            panel.SetActive(true);
            onEnableAction?.Invoke();
        }
        else
        {
            Debug.LogWarning("PanelType not found in dictionary.");
        }
    }

    private void EnableMenuPannel(MenuPannelType pannel, Action onEnableAction)
    {
        TurnOffMenuPannels();
        GameObject panel = menuPanelsDictionary[pannel];
        panel.SetActive(true);
        onEnableAction?.Invoke();
    }
}
