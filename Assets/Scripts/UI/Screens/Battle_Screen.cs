using GLG.UI;
using UnityEngine;

public class Battle_Screen : UIController
{
    [SerializeField] private UICard _cardPrefab;
    [SerializeField] private EffectCard _effectPrefab;
    [SerializeField] private RectTransform _cardsContainer;
    [SerializeField] private RectTransform _effectsContainer;

    public Battle_Screen AddEffect(EffectData effectData)
    {

        return this;
    }
    public Battle_Screen AddCard(CardData cardData)
    {

        return this;
    }
    public Battle_Screen ClearCards()
    {
        return this;
    }

}
