using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardsManager : MonoBehaviour
{
    public static DailyRewardsManager instance;
    private void Awake()
    {
        instance = this;
        PlayerPrefs.DeleteKey("LastLoginDate");
    }

    private DateTime lastLoginDate;
    private int daysSinceLastLogin;

    private ISaveLoadSystem _saveLoadSystem;

    private void Start()
    {
        _saveLoadSystem = ReadonlySaveLoadSystemFactory.Instance.Get();
    }

    public void CheckForDailyRewards(Action dailyRewardAvailableAction)
    {
        if (!PlayerPrefs.HasKey("LastLoginDate"))
            dailyRewardAvailableAction?.Invoke();

        else
        {
            string lastLoginDateString = PlayerPrefs.GetString("LastLoginDate");
            if (!string.IsNullOrEmpty(lastLoginDateString))
            {
                lastLoginDate = DateTime.Parse(lastLoginDateString);
                TimeSpan timeSinceLastLogin = DateTime.Now - lastLoginDate;
                daysSinceLastLogin = timeSinceLastLogin.Days;

                // Check if it's a new day and grant reward
                if (daysSinceLastLogin >= 1)
                {
                    dailyRewardAvailableAction?.Invoke();
                }
            }
        }
        // Update the last login date
        PlayerPrefs.SetString("LastLoginDate", DateTime.Now.ToString());
    }

    public void GrantDailyReward()
    {
        // TODO: Add code to grant the daily reward
        Debug.Log("Award granted");
        PlayerData _playerData = _saveLoadSystem.Load<PlayerData>();
        PlayerData newPlayerData = new PlayerData(_playerData.NickName, _playerData.Money + 50, _playerData.Stack);
        _saveLoadSystem.Save(newPlayerData);
    }
}
