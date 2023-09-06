using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class BuyinPannel : NetworkBehaviour
{
    public static BuyinPannel Instance;
    [SerializeField] GameObject buyinPannel;
    [SerializeField] Button openBuyinPannelBtn;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] Button buyinBtn;
    [SerializeField] Button cancelBtn;

    private static PlayerSeats PlayerSeats => PlayerSeats.Instance;
    private uint currentMoney;
    private ISaveLoadSystem _saveLoadSystem;
    private static PlayerSeats playerSeats => PlayerSeats.Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        _saveLoadSystem = ReadonlySaveLoadSystemFactory.Instance.Get();

        buyinPannel.SetActive(false);

        openBuyinPannelBtn.onClick.AddListener(() => {
            buyinPannel.SetActive(true);
            PlayerData playerData = _saveLoadSystem.Load<PlayerData>();

            currentMoney = playerData.Money;
            moneyText.text = playerData.Money + "$";
        });

        buyinBtn.onClick.AddListener(() => {

            if (currentMoney < 2)
                return;

            Player player = PlayerSeats.Players.Find(x => x != null && x.OwnerClientId == OwnerClientId);
            currentMoney -= 2;
            
            uint currentStack = player.Stack;
            currentStack += 1000;
            Debug.Log("Player stack: " + currentStack);

            //player.SetStack(currentStack);

            player.SetStackAmountClientRpc(currentStack);
            StopBuyIn();
            buyinPannel.SetActive(false);
        });

        cancelBtn.onClick.AddListener(() => {
            buyinPannel.SetActive(false);
        });
    }

    //[ServerRpc]
    //private void SetStackServerRPC(ulong playerId, uint stack)
    //{
    //    if (!IsHost)
    //        return;

    //    Player player = PlayerSeats.Players.Find(x => x != null && x.OwnerClientId == playerId);
    //    player.SetStackAmountClientRpc(stack);
    //}

    public void StopBuyIn()
    {
        openBuyinPannelBtn.gameObject.SetActive(false);
    }

    public void ContinueBuyIn()
    {
        openBuyinPannelBtn.gameObject.SetActive(true);
    }
}
