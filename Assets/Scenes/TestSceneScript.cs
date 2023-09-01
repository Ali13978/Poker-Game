using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log("On start");
    }

    private void OnEnable()
    {
        Debug.Log("On Enable");
    }
}
