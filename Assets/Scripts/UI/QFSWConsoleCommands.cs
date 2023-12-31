using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QFSWConsoleCommands : MonoBehaviour
{
    private ISaveLoadSystem _saveLoadSystem;

    private void Start()
    {
        _saveLoadSystem = ReadonlySaveLoadSystemFactory.Instance.Get();
    }

    [Command]
    private void GiveChipsAndMoney(uint money, uint chips)
    {
        PlayerData playerData = _saveLoadSystem.Load<PlayerData>();

        PlayerData newPlayerData = new PlayerData(playerData.NickName, playerData.Money + money, playerData.Stack + chips);

        _saveLoadSystem.Save(newPlayerData);
    }
}
