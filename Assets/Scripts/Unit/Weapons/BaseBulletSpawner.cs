using UnityEngine;

public abstract class BaseBulletSpawner : MonoBehaviour
{
    public abstract void DoShot(Transform startPoint, Vector3 endPoint);
    public abstract void Dispose();
    public abstract void Prepare();
}
