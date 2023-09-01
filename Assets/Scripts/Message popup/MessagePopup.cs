using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MessagePopup : MonoBehaviour
{
    [SerializeField] Button doneBtn;
    [SerializeField] TMP_Text headingText;
    [SerializeField] TMP_Text messageText;
    [SerializeField] TMP_Text btnText;

    public static MessagePopup instance;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void SetValues(string _headingText, string _messageText, string _btnText, UnityAction btnAction = default)
    {
        btnAction += () =>{
            gameObject.SetActive(false);
        };
        headingText.text = _headingText;
        messageText.text = _messageText;
        btnText.text = _btnText;
        doneBtn.onClick.AddListener(btnAction);
    }
}
