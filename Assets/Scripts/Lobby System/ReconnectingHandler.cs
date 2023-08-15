using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReconnectingHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject DisconnectedPannel;
    private float countdownTimer = 10.0f; // Initial countdown time

    private bool isInternetConnected = true;
    string relayCode;

    private void Start()
    {
        IEnumerable<string> connectionData = NetworkConnectorHandler.CurrentConnector.ConnectionData;
        relayCode = string.Join(":", connectionData);

        StartCoroutine(CheckInternetConnection());
    }

    private void UpdateTimerText(string text)
    {
        if (timerText != null)
        {
            timerText.text = text;
        }
    }

    private IEnumerator CheckInternetConnection()
    {
        while (true)
        {
            UpdateTimerText("Reconnecting in - " + Mathf.RoundToInt(countdownTimer));
            countdownTimer -= 1.0f;

            if (countdownTimer <= 0)
            {
                UpdateTimerText("Reconnecting...");

                NetworkReachability reachability = Application.internetReachability;

                if (reachability != NetworkReachability.NotReachable)
                {
                    // Internet connection is available
                    if (!isInternetConnected)
                    {
                        Debug.Log("Internet connection is now active.");
                        isInternetConnected = true;
                        countdownTimer = 10.0f; // Reset the countdown

                        //Player is back online;
                        
                        if (!string.IsNullOrEmpty(relayCode))
                        {
                            NetworkConnectorHandler.JoinGame(NetworkConnectorType.UnityRelay, relayCode);
                        }

                        DisconnectedPannel.SetActive(false);
                    }
                }
                else
                {
                    if (isInternetConnected)
                    {
                        // Internet connection is not available
                        Debug.LogWarning("Internet connection lost.");
                        isInternetConnected = false;

                        //Player is offline

                        DisconnectedPannel.SetActive(true);
                    }
                }

                countdownTimer = 10.0f; // Reset the countdown
            }

            // Check the internet connection every second
            yield return new WaitForSeconds(1.0f);
        }
    }
}
