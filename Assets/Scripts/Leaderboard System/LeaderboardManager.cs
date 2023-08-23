using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards.Exceptions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private List<Leaderboardentity> entries;
    [SerializeField] private object scoresResponse;


    public static LeaderboardManager instance;
    private async void Awake()
    {
        instance = this;
        await UnityServices.InitializeAsync();
        DontDestroyOnLoad(gameObject);
    }

    private int CompareByRankDescending(Leaderboardentity a, Leaderboardentity b)
    {
        // Compare ranks in descending order
        return b.rank.CompareTo(a.rank);
    }

    private void SortEntriesByRankDescending()
    {
        entries.Sort(CompareByRankDescending);
    }

    public async Task<string> GetScores(string LeaderboardId)
    {
        try
        {
            scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);

            string jsonData = JsonConvert.SerializeObject(scoresResponse);
            Debug.Log(jsonData);
            return jsonData;
        }
        catch (UnityException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public async Task<string> GetPlayerScore(string LeaderboardId)
    {
        try
        {
            var scoreResponse =
                await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            string jsonData = JsonConvert.SerializeObject(scoreResponse);
            Debug.Log(jsonData);
            return jsonData;
        }
        catch (LeaderboardsException ex)
        {
            // Catch and handle the HttpNotFoundException
            Debug.LogError("HttpNotFoundException: " + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            // Catch any other exceptions if needed
            Debug.LogError("Exception: " + ex.Message);
            return null;
        }
    }

    public async void UpdateLeaderboard(string leaderboardId, GameObject parentObject, GameObject playerObjectPrefab)
    {
        string jsonData = await GetScores(leaderboardId);

        FillEntriesFromJSON(jsonData);

        SortEntriesByRankDescending();
        
        for(int i = 0; i < entries.Count; i++)
        {
            LeaderboardEntry entry =  Instantiate(playerObjectPrefab, parentObject.transform).GetComponent<LeaderboardEntry>();
            entry.UpdateLeaderbordEntry(entries[i].rank.ToString(), entries[i].playerName);
        }
    }

    private void FillEntriesFromJSON(string jsonData)
    {
        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(jsonData);
        entries = data.results;
    }
}


[System.Serializable]
class LeaderboardData
{
    public int limit;
    public int total;
    public List<Leaderboardentity> results;
}

[System.Serializable]
class Leaderboardentity
{
    public string playerId;
    public string playerName;
    public int rank;
    public float score;
}

[System.Serializable]
class playerdata
{
    public string playerId;
    public string playerName;
    public int rank;
    public float score;
    public string updatedTime;
}

