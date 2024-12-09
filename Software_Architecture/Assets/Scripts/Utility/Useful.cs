using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Useful methods, used throughout different scripts
/// </summary>

public class Useful : MonoBehaviour
{
    // Get all children, including children of children
    public static List<GameObject> GetAllChildren(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent)
        {
            children.Add(child.gameObject);
            children.AddRange(GetAllChildren(child));
        }
        return children;
    }

    // Get the most upper parent, which holds all transforms
    public static Transform GetMostUpperTransform(Transform transform)
    {
        Transform upperTransform = transform.parent;
        if (upperTransform.parent != null)
        {
            upperTransform = GetMostUpperTransform(upperTransform);
        }

        return upperTransform;
    }

    // Get the rendered height of the transform
    public static float GetRenderedHeight(Transform transform)
    {
        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);

        // Go through each render component and add them to the bounds
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        // Return total height
        return combinedBounds.size.y;
    }

    // Get the Xth parent of the transform
    public static Transform GetXthParentTransform(Transform transform, int count)
    {
        if (count <= 0) { return transform; }

        Transform parent = transform;

        for (int i = 0; i < count; i++)
        {
            Transform temp = parent.parent;
            if (temp == null) { continue; }

            parent = temp;
        }

        return parent;
    }
}
