using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods 
{
    public static float AngleDegrees(this Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    public static float AngleDegreesXMirrored(this Vector2 vector,bool mirror)
    {
        if (mirror)
            vector.x *= -1;
        return Mathf.Atan(vector.y/vector.x) * Mathf.Rad2Deg;
    }

    public static bool Contains(this LayerMask mask, int check )
    {
        if(mask.value == (mask.value | 1 << check))
        {
            return true;
        }
        return false;
    }
}
