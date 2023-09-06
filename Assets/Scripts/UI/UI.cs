using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using static AliScripts.AliExtras;

public class UI : MonoBehaviour
{

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        IEnumerable<string> connectionData = NetworkConnectorHandler.CurrentConnector.ConnectionData;
        string relayCode = string.Join(":", connectionData);

        Lobby lobby = LobbyManager.instance.GetJoinedLobby();

        if (LobbyManager.instance.IsHost())
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions();
                options.Data = new Dictionary<string, DataObject>
            {
                {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
            };

                await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        Game.Instance.isTornument = GetBoolFromString(lobby.Data["IsTornument"].Value);
        Game.Instance.isTornumentA = GetBoolFromString(lobby.Data["IsTornumentA"].Value);

        LobbyManager.instance.LeaveLobby(() => { }, () => { });
    }
}   
