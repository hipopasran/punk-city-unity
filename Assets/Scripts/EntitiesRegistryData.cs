using UnityEngine;

[CreateAssetMenu(fileName = "EntitiesRegistryData", menuName = "GLG/EntitiesRegistryData")]
public class EntitiesRegistryData : ScriptableObject
{
    public CardData[] cards;
    public EffectData[] effects;
}
