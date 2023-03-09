using UnityEngine;

public class EntityRegistryItem
{
    public string resourcesPath;
    public string name;
    public string universalKey;
}
[System.Serializable]
public class SkinData
{
    public string id;
}
[CreateAssetMenu(fileName = "EntitiesRegistryData", menuName = "GLG/EntitiesRegistryData")]
public class EntitiesRegistryData : ScriptableObject
{
    public CardData[] cards;
    public EffectData[] effects;
    public SkinData[] skins;
}
