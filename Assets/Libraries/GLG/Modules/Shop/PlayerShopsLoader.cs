using GLG;
using UnityEngine;

public class PlayerShopsLoader : MonoBehaviour
{
    private void Awake()
    {
        Kernel.Economic.PlayerShopsManager.Load();
    }
}
