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
[System.Serializable]
public class WeaponData
{
    public string id;
    public AttackKind attackKind;
    public AttackPreparingKind attackPreparingKind;
    public WeaponKind weaponKind;
    public float additionalEquipDelay = 0f;
}
[CreateAssetMenu(fileName = "EntitiesRegistryData", menuName = "GLG/EntitiesRegistryData")]
public class EntitiesRegistryData : ScriptableObject
{
    public CardData[] cards;
    public EffectData[] effects;
    public SkinData[] skins;
    public WeaponData[] weapons;    
}
