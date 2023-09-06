using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundCompletedUI : MonoBehaviour
{
    public GameObject RoundCompletedObject => _roundCompletedObject;
    [SerializeField] private GameObject _roundCompletedObject;

    private static Game Game => Game.Instance;

    private void OnEnable()
    {
        Game.EndDealEvent += OnEndDeal;
        Game.GameStageBeganEvent += OnStartDeal;
    }

    private void OnDisable()
    {
        Game.EndDealEvent -= OnEndDeal;
        Game.GameStageBeganEvent -= OnStartDeal;
    }

    private void OnEndDeal(WinnerInfo[] winnerInfo)
    {
        _roundCompletedObject.SetActive(true);
    }
    
    private void OnStartDeal(GameStage gameStage)
    {
        _roundCompletedObject.SetActive(false);
    }
}
