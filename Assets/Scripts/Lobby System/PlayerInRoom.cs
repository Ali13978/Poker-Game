using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerInRoom : MonoBehaviour
{
    [SerializeField] private Image playerProfilePic;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Button KickBtn;

    public void UpdatePlayerUI(Sprite _playerProfilePic, string _playerName, UnityAction KickBtnAction)
    {
        playerProfilePic.sprite = _playerProfilePic;
        playerNameText.text = _playerName;
        KickBtn.onClick.AddListener(KickBtnAction);
    }
}
