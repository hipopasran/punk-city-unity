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
    public Dictionary<string, CardData> Cards { get; private set; }
    public Dictionary<string, EffectData> Effects { get; private set; }

    private void Initialize()
    {
        if(_initialized) return;
        _initialized = true;
        _i = this;
        // initialize all dictionarites here
        Cards = new Dictionary<string, CardData>();
        foreach (var item in _data.cards)
        {
            Cards.Add(item.id, item);
        }

        Effects = new Dictionary<string, EffectData>();
        foreach (var item in _data.effects)
        {
            Effects.Add(item.id, item);
        }
    }
    
    private void OnEnable()
    {
        Initialize();
    }


}
