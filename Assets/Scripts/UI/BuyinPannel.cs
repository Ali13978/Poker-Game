using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyinPannel : MonoBehaviour
{
    public static BuyinPannel Instance;
    [SerializeField] GameObject buyinPannel;
    [SerializeField] Button openBuyinPannelBtn;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] Button buyinBtn;
    [SerializeField] Button cancelBtn;

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

            Player myPlayer = playerSeats.LocalPlayer;
            currentMoney -= 2;
            
            uint currentStack = myPlayer.Stack;
            currentStack += 1000;
            Debug.Log("Player stack: " + currentStack);
            myPlayer.SetStack(currentStack);

            StopBuyIn();
            buyinPannel.SetActive(false);
        });

        cancelBtn.onClick.AddListener(() => {
            buyinPannel.SetActive(false);
        });
    }

    public void StopBuyIn()
    {
        buyinBtn.gameObject.SetActive(false);
    }

    public void ContinueBuyIn()
    {
        buyinBtn.gameObject.SetActive(true);
    }
}
