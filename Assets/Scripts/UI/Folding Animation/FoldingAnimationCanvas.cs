using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoldingAnimationCanvas : MonoBehaviour
{
    #region Singleton
    public static FoldingAnimationCanvas instance;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    #endregion

    [SerializeField] Image cardOneImage;
    [SerializeField] Image cardTwoImage;

    public void EnableFoldingCardCanvas(Sprite cardOne, Sprite cardTwo)
    {
        cardTwoImage.sprite = cardOne;
        cardOneImage.sprite = cardTwo;
        gameObject.SetActive(true);
    }
}
