using GLG.UI;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Screen : UIController
{
    [SerializeField] private UICard _cardPrefab;
    [SerializeField] private EffectCard _effectPrefab;
    [SerializeField] private RectTransform _cardsContainer;
    [SerializeField] private RectTransform _effectsContainer;
    [SerializeField] private PlayerInfoBlock _playerInfoBlock;
    [SerializeField] private PlayerInfoBlock _enemyInfoBlock;

    private List<UICard> _cards = new List<UICard>();

    public Battle_Screen AddEffect(EffectData effectData)
    {

        return this;
    }
    public UICard AddCard(CardData cardData)
    {
        UICard card = Instantiate(_cardPrefab, _cardsContainer);
        card.ApplyCardData(cardData);
        _cards.Add(card);
        return card;
    }
    public Battle_Screen ClearCards()
    {
        foreach (var item in _cards)
        {
            Destroy(item.gameObject);
        }
        _cards.Clear();
        return this;
    }

    public Battle_Screen SetPlayerData(Profile profile)
    {
        _playerInfoBlock.SetNick(profile.identification);
        return this;
    }
    public Battle_Screen SetEnemyData(Profile profile)
    {
        _enemyInfoBlock.SetNick(profile.identification);
        return this;
    }
    public UICard GetCard(string key)
    {
        foreach (var item in _cards)
        {
            if(item.RelatedCardData.id == key)
            {
                return item;
            }
        }
        return null;
    }
}
