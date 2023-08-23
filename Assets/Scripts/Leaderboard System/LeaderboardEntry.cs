using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text playerNameText;

    public void UpdateLeaderbordEntry(string Rank, string PlayerName)
    {
        rankText.text = "#" + Rank;
        playerNameText.text = PlayerName;
    }
}