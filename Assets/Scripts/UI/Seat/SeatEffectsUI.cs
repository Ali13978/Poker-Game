using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeatEffectsUI : MonoBehaviour
{
    public GameObject WinnerSeatEffectObject => _winnerSeatEffectObject;
    [SerializeField] private GameObject _winnerSeatEffectObject;

    public GameObject WinnerSeatTextObject => _winnerSeatTextObject;
    [SerializeField] private GameObject _winnerSeatTextObject;

    [SerializeField] private int _index;

    private static Game Game => Game.Instance;
    private static PlayerSeats PlayerSeats => PlayerSeats.Instance;

    private void OnEnable()
    {
        Game.EndDealEvent += OnEndDeal;
        Game.GameStageBeganEvent += OnStartDeal;
    }

    private void OnDisable()
    {
        Game.EndDealEvent -= OnEndDeal;
    }

    private void OnEndDeal(WinnerInfo[] winnerInfo)
    {
        List<Player> winners = PlayerSeats.Players.FindAll(player => player != null && winnerInfo.Select(info => info.WinnerId).Contains(player.OwnerClientId));

        for (var i = 0; i < winners.Count; i++)
        {
            if (PlayerSeats.Players.IndexOf(winners[i]) != _index)
            {
                continue;
            }

            _winnerSeatEffectObject.SetActive(true);
            _winnerSeatTextObject.SetActive(true);
            return;
        }
    }



    private void OnStartDeal(GameStage gameStage)
    {
        _winnerSeatEffectObject.SetActive(false);
        _winnerSeatTextObject.SetActive(false);
    }
}
