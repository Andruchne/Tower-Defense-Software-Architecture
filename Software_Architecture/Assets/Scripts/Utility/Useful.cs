using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Useful : MonoBehaviour
{
    // Recursively get all children, including children of children
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

    // Gets the most upper parent, which holds all transforms
    public static Transform GetMostUpperTransform(Transform transform)
    {
        Transform upperTransform = transform.parent;
        if (upperTransform.parent != null)
        {
            upperTransform = GetMostUpperTransform(upperTransform);
        }

        return upperTransform;
    }

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

    public static Transform GetXthParentTransform(Transform transform, int count)
    {
        if (count <= 0) { return transform; }

        Transform parent = transform;

        for (int i = 0; i < count; i++)
        {
            Transform temp = transform.parent;
            if (temp == null) { continue; }

            parent = temp;
        }

        return parent;
    }
}
