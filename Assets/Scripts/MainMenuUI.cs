using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Login-Pannel")]
    [SerializeField] GameObject loginPannel;

    [Header("MainMenu-Pannel")]
    [SerializeField] GameObject mainMenuPannel;

    private void TurnOffAllPannels()
    {
        loginPannel.SetActive(false);
        mainMenuPannel.SetActive(false);
    }

    public void StartMainMenuPannel()
    {
        TurnOffAllPannels();
        mainMenuPannel.SetActive(true);
    }
}
