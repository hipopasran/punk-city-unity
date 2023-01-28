using GLG;
using GLG.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[System.Serializable]
public class IconInfo
{
    public string id;
    public string name;
    public SpriteAtlas atlas;
    public Sprite sprite;
    public Sprite GetSprite()
    {
        return sprite == null ? atlas.GetSprite(id) : sprite;
    }
}

public class DailyRewardOverlay : UIController
{
    public GameObject cardPrefab;
    public RectTransform cardsParent;
    public System.Action onRewardCollected;
    public System.Action onClosed;
    public IconInfo[] icons;
    [SerializeField] private Button _claimButton;
    [SerializeField] private Button _adClaimButton;
    [SerializeField] private Text _multiplierText;

    [Header("Navigation")]
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private RectTransform _content;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    [SerializeField] private ScrollRect _scrollRect;


    private List<UITicket> _cards = new List<UITicket>();
    private string _currentRewardKey;
    private int _currentRewardValue;
    private int _currentRewardMultiplier;
    private int _currentCardIndex;
    private int _currentDay;
    protected override void OnStartShow()
    {
        base.OnStartShow();
        GameParametersHub.NeedToStopMoving();
    }
    protected override void OnEndHide()
    {
        base.OnEndHide();
        onClosed?.Invoke();
        Dispose();
    }
    public DailyRewardOverlay SpawnCard(string id, int reward, int rvMultiplier, int day)
    {
        if (string.IsNullOrEmpty(id)) return this;
        Transform card = Instantiate(cardPrefab).transform;
        card.SetParent(cardsParent, false);
        UITicket block = card.GetComponent<UITicket>();
        block.SetHeader(day.ToString());
        block.SetValue(reward.ToString());
        Sprite sprite = GetIconInfo(id).GetSprite();
        block.SetIcon(sprite);
        _cards.Add(block);
        return this;
    }
    public DailyRewardOverlay SetCurrentCardIndex(int index)
    {
        _currentCardIndex = index;
        _cards[index].SetUnlocked();
        _currentDay = index + 1;
        CoroutinesHelper.DoOnNextFrame(this, () =>
        {
            _scrollRect.normalizedPosition = new Vector2(Mathf.InverseLerp(0f, _cards.Count - 1, index), _scrollRect.normalizedPosition.y);
            _cards[index].SetSelection(true);
        });
        return this;
    }
    public DailyRewardOverlay SetPreviousCardsState(bool state)
    {
        for (int i = _currentCardIndex - 1; i >= 0; i--)
        {
            _cards[i].DetachSecondPart(true);
        }
        return this;
    }
    public DailyRewardOverlay SetNextCardsAnonimous(int startIndex)
    {
        for (int i = _currentCardIndex + 1; i < _cards.Count; i++)
        {
            if (i < _currentCardIndex + startIndex)
            {
                _cards[i].SetUnlocked();
            }
            else
            {
                _cards[i].SetAnonimous();
            }
        }
        return this;
    }
    public DailyRewardOverlay SetCurrentReward(string id, int reward, int rvMultiplier)
    {
        _currentRewardKey = id;
        _currentRewardValue = reward;
        _currentRewardMultiplier = rvMultiplier;
        _multiplierText.text = 'x' + rvMultiplier.ToString();
        return this;
    }
    public DailyRewardOverlay OnClosed(System.Action callback)
    {
        onClosed += callback;
        return this;
    }
    private IconInfo GetIconInfo(string id)
    {
        foreach (var item in icons)
        {
            if (item.id == id) return item;
        }
        return null;
    }
    private void Dispose()
    {
        foreach (var item in _cards)
        {
            Destroy(item.gameObject);
        }
        _cards.Clear();
        onClosed = null;
    }


    public void GetRewardButtonHandler()
    {
        _claimButton.interactable = false;
        _adClaimButton.interactable = false;
        GetReward(false);
    }
    public void AdButtonHandler()  //TODO
    {
        /*
        if (AdWrapper.Rewarded.IsReady)
        {
            _claimButton.interactable = false;
            _adClaimButton.interactable = false;
            GameParametersHub.IsWatchingAd = true;
            AdWrapper.Rewarded.ClearCallbacks().Show($"reward_ingame_rvDailyReward")
                .OnSuccess(() =>
                {
                    StatsWrapper.Track(TrackId.DailyRewardRV, "day", _currentDay);
                    GetReward(true);
                })
                .OnClosed(() =>
                {
                    Hide();
                    GameParametersHub.IsWatchingAd = false;
                });
        }
        else
        {
            ShowNotify(I2.Loc.LocalizationManager.GetTranslation("notify_advNotReady"));
        }
        */
    }

    private void GetReward(bool withMultiplier)
    {
        int multiplier = withMultiplier ? _currentRewardMultiplier : 1;
        switch (_currentRewardKey)
        {
            case "cash":
                RewardCash(_currentRewardValue, multiplier, Hide);
                break;
            case "ticket":
                //RewardTickets(_currentRewardValue, multiplier, Hide);
                break;
            case "key":
                //RewardKeys(_currentRewardValue, multiplier, Hide);
                break;
            default:
                break;
        }
    }
    private void RewardCash(int reward, int multiplier, System.Action callback)
    {
        Kernel.UI.Get<FlyingItemsOverlay>().Spawn
            (
            FlyingItemSpace.CanvasToCanvas,
            0,
            10,
            _cards[_currentCardIndex].IconPosition,
            Kernel.UI.Get<InGameScreen>().Money.MoneyTransform.position,
            1.5f,
            0.1f,
            0.2f,
            null,
            () => { Economic.i.PlayerMoney.AddMoney(reward * multiplier); callback?.Invoke(); }
            );

    }

    private void ShowNotify(string text)
    {
        Kernel.UI.ShowUI<NotifyOverlay>().SetText(text);
    }
}
