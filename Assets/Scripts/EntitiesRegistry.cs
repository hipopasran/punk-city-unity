using System.Collections.Generic;
using UnityEngine;

public class EntitiesRegistry : MonoBehaviour
{
    [SerializeField] private EntitiesRegistryData _data;
    private static bool _initialized = false;
    private static EntitiesRegistry _i;

    public static EntitiesRegistry i
    {
        get
        {
            if (!_initialized)
            {
                _i = FindObjectOfType<EntitiesRegistry>();
                _i.Initialize();
            }

            return _i;
        }
    }
    public EntitiesRegistryData Data => _data;
    public CardData[] Cards => _data.cards;
    public Dictionary<string, CardData> CardsRegistry { get; private set; }
    public EffectData[] Effects => _data.effects;
    public Dictionary<string, EffectData> EffectsRegistry { get; private set; }
    public SkinData[] Skins => _data.skins;

    private void Initialize()
    {
        if(_initialized) return;
        _initialized = true;
        _i = this;
        // initialize all dictionarites here
        CardsRegistry = new Dictionary<string, CardData>();
        foreach (var item in _data.cards)
        {
            CardsRegistry.Add(item.id, item);
        }

        EffectsRegistry = new Dictionary<string, EffectData>();
        foreach (var item in _data.effects)
        {
            EffectsRegistry.Add(item.id, item);
        }
    }
    
    private void OnEnable()
    {
        Initialize();
    }


}
