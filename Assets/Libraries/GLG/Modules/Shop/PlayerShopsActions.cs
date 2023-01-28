using UnityEngine;

public class PlayerShopsActions : MonoBehaviour
{
    public void OnExtraMoney(int level)
    {
        //GameParametersHub.moneyMultiplier = GameParametersHub.GameLevelsConfig.cashMultipliersByLevels[level];
    }
    public void OnMagnetForce(int level)
    {
        //GameParametersHub.magnetForceMultiplier = GameParametersHub.GameLevelsConfig.magnetForceMultipliersByLevels[level];
    }
    public void OnPlayerSpeed(int level)
    {
        //GameParametersHub.playerSpeedMultiplier = GameParametersHub.GameLevelsConfig.playerSpeedMultipliersByLevels[level];
    }
    public void OnPickingSpeed(int level)
    {
        //GameParametersHub.pickupSpeedMultiplier = GameParametersHub.GameLevelsConfig.pickingItemsSpeedMultipliersByLevels[level];
    }
}
