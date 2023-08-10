using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class LoginManager : MonoBehaviour
{
    private async void Start()
    {
        InitializationOptions options = new InitializationOptions();
        options.SetProfile(Random.Range(1, 10000).ToString());

        await UnityServices.InitializeAsync(options);
        AuthenticationService.Instance.SignedIn +=()=> {
            StartCoroutine(MainMenuUI.instance.StartLoginLoading());
        };

        if(!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        else
            StartCoroutine(MainMenuUI.instance.StartLoginLoading());
    }
}
