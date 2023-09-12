using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFoldingCompleted : MonoBehaviour
{
    public void OnFoldingCompletion()
    {
        FoldingAnimationCanvas.instance.gameObject.SetActive(false);
    }
}
