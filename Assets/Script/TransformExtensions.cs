using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static int GetSortingOrder(this Transform transform, float yOffset = 0)
    {
        // Calculate the sorting order based on the Y position of the transform
        // The higher the Y position, the lower the sorting order (rendered behind)
        return -(int)((transform.position.y + yOffset) * 100);
    }
}
