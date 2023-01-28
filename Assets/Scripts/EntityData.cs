using System.Text;
using UnityEngine;
// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
public enum EntityKind { Unit, Weapon } // !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
public enum Owner { Player, Enemy }
// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!

public class EntityData : MonoBehaviour// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
{// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
    public string universalKey;
    public EntityKind entityKind;// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
    public Owner owner;

    public string GetKey(bool entityKind, bool owner, string prefix = null, string suffix = null)
    {
        return GetKey(this, entityKind, owner, prefix, suffix);
    }
    public static string GetKey(EntityData entityData, bool entityKind, bool owner, string prefix = null, string suffix = null)
    {
        StringBuilder sb = new StringBuilder();
        if (prefix != null)
        {
            sb.Append(prefix);
            sb.Append('_');
        }
        if (entityKind)
        {
            sb.Append(entityData.entityKind);
            sb.Append('_');
        }
        if (owner)
        {
            sb.Append(entityData.owner);
            sb.Append('_');
        }
        sb.Append(entityData.universalKey);
        if (suffix != null)
        {
            sb.Append('_');
            sb.Append(suffix);
        }
        return sb.ToString();
    }
    public static EntityKind[] GetKinds()// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
    {
        return new EntityKind[2]// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
        {
            EntityKind.Unit,
            EntityKind.Weapon,
        };
    }// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
}// !!!!!!!!!! DO NOT FORGET TO ADD NEW ENTITY KIND TO EntityData.GetKinds() !!!!!!!!!!!!!!!!!!!!
