using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script scales selectable objects, when the mouse is hovering over them
/// It uses LeanTween, to apply the scaling behaviour
/// </summary>

public class MouseHoverScale : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float tweenDuration = 0.3f;

    private Transform currentHoveredObject = null;

    // To store original scale of selectable transform
    private Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>(); 

    void Update()
    {
        MouseOverSelectable();
    }

    private void MouseOverSelectable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if mouse over selectable layer object
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Transform hitTransform = hit.transform;

            // If new object is hovered on
            if (hitTransform != currentHoveredObject)
            {
                // Reset the previous object's scale
                if (currentHoveredObject != null)
                {
                    ResetScale(currentHoveredObject);
                }

                currentHoveredObject = hitTransform;

                // Store the original scale if not already stored
                if (!originalScales.ContainsKey(currentHoveredObject))
                {
                    RemoveInvalidEntries();
                    originalScales[currentHoveredObject] = currentHoveredObject.localScale;
                }

                ScaleUp(currentHoveredObject);
            }
        }
        else
        {
            // If no object is hit, reset scale
            if (currentHoveredObject != null)
            {
                ResetScale(currentHoveredObject);
                currentHoveredObject = null;
            }
        }
    }

    private void RemoveInvalidEntries()
    {
        // This avoids having invalid keys inside the originalScale dictionary
        List<Transform> entriesToRemove = new List<Transform>();

        // Check all entries
        foreach (Transform transform in originalScales.Keys)
        {
            if (transform == null)
            {
                entriesToRemove.Add(transform);
            }
        }

        // Remove invalid entries
        foreach (Transform invalid in entriesToRemove)
        {
            originalScales.Remove(invalid);
        }
    }

    private void ScaleUp(Transform target)
    {
        Vector3 originalScale = originalScales[target];
        LeanTween.scale(target.gameObject, 
            originalScale * scaleMultiplier, 
            tweenDuration).
            setEase(LeanTweenType.easeOutQuad);
    }

    private void ResetScale(Transform target)
    {
        if (originalScales.TryGetValue(target, out Vector3 originalScale))
        {
            LeanTween.scale(target.gameObject, 
                originalScale, 
                tweenDuration).
                setEase(LeanTweenType.easeOutQuad);
        }
    }
}
