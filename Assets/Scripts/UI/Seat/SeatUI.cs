using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class SeatUI : MonoBehaviour
{
    public Image PlayerImage => _playerImage;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Sprite _avatarLoadingSprite;

    public TextMeshProUGUI NickNameText => _nickNameText;
    [SerializeField] private TextMeshProUGUI _nickNameText;

    public GameObject PocketCards => _pocketCards;
    [SerializeField] private GameObject _pocketCards;

    public Image NickNameBackgroundImage => _nickNameBackgroundImage;
    [SerializeField] private Image _nickNameBackgroundImage;

    [SerializeField] private Animator _animator;

    public GameObject ShowSelectedOption => _showSelectedOption;
    [SerializeField] private GameObject _showSelectedOption;

    public TextMeshProUGUI SelectedOptionText => _selectedOptionText;
    [SerializeField] private TextMeshProUGUI _selectedOptionText;

    private static readonly int LoadingAvatar = Animator.StringToHash("LoadingAvatar");
    private static readonly int Empty = Animator.StringToHash("Empty");
    
    private static Game Game => Game.Instance;

    public void EnableLoadingImage()
    {
        _playerImage.sprite = _avatarLoadingSprite;
        
        _animator.ResetAllTriggers();
        _animator.SetTrigger(LoadingAvatar);
    }

    public void DisableLoadingImage()
    {
        _playerImage.sprite = null;
        
        _animator.ResetAllTriggers();
        _animator.SetTrigger(Empty);
    }

    private void OnEnable()
    {
        Game.EndDealEvent += OnEndDeal;
    }

    private void OnDisable()
    {
        Game.EndDealEvent -= OnEndDeal;
    }

    private void OnEndDeal(WinnerInfo[] winnerInfo)
    {
        ShowSelectedOption.SetActive(false);
        SelectedOptionText.text = "";
    }
}
