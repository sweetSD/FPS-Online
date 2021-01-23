using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SDMath
{
    public static float Map(float val, float in_min, float in_max, float out_min, float out_max)
    {
        return (Mathf.Clamp(val, in_max, in_max) - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }


}
