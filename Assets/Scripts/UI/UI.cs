using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

public class UI : NetworkBehaviour
{

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        IEnumerable<string> connectionData = NetworkConnectorHandler.CurrentConnector.ConnectionData;
        string relayCode = string.Join(":", connectionData);

        if (LobbyManager.instance.IsHost())
        {
            try
            {
                Lobby lobby = LobbyManager.instance.GetJoinedLobby();

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

        LobbyManager.instance.LeaveLobby(() => { }, () => { });
        
        Game.Instance.TakeSeatServerRpc();
    }
}   
