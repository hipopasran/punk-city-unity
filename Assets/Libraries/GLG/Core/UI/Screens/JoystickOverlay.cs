using GLG.UI;
using SimpleInputNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickOverlay : UIController
{
    public Joystick joystick;

    private void Start()
    {
        GameParametersHub.onNeedToStopMoving += joystick.ForceStop;
    }
    private void OnDestroy()
    {
        GameParametersHub.onNeedToStopMoving -= joystick.ForceStop;
    }
}
