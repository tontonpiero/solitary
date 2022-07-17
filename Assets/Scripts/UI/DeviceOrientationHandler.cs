using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitary
{
    public class DeviceOrientationHandler : MonoBehaviour
    {
        [ContextMenu("ChangeOrientation")]
        public void ChangeOrientation()
        {
            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                Screen.orientation = ScreenOrientation.Landscape;
            }
            else
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }
    }
}
