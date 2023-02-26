using UnityEngine;

public class EntityRegistryItem
{
    public string resourcesPath;
    public string name;
    public string universalKey;
}
[CreateAssetMenu(fileName = "EntitiesRegistryData", menuName = "GLG/EntitiesRegistryData")]
public class EntitiesRegistryData : ScriptableObject
{
    public CardData[] cards;
    public EffectData[] effects;
}
