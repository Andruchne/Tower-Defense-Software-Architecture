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

    private Transform _currentHoveredObject;

    // To store original scale of selectable transform
    private Dictionary<Transform, Vector3> _originalScales = new Dictionary<Transform, Vector3>(); 

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
            if (hitTransform != _currentHoveredObject)
            {
                // Reset the previous object's scale
                if (_currentHoveredObject != null)
                {
                    ResetScale(_currentHoveredObject);
                }

                _currentHoveredObject = hitTransform;

                // Store the original scale if not already stored
                if (!_originalScales.ContainsKey(_currentHoveredObject))
                {
                    RemoveInvalidEntries();
                    _originalScales[_currentHoveredObject] = _currentHoveredObject.localScale;
                }

                ScaleUp(_currentHoveredObject);
            }
        }
        else
        {
            // If no object is hit, reset scale
            if (_currentHoveredObject != null)
            {
                ResetScale(_currentHoveredObject);
                _currentHoveredObject = null;
            }
        }
    }

    private void RemoveInvalidEntries()
    {
        // This avoids having invalid keys inside the originalScale dictionary
        List<Transform> entriesToRemove = new List<Transform>();

        // Check all entries
        foreach (Transform transform in _originalScales.Keys)
        {
            if (transform == null)
            {
                entriesToRemove.Add(transform);
            }
        }

        // Remove invalid entries
        foreach (Transform invalid in entriesToRemove)
        {
            _originalScales.Remove(invalid);
        }
    }

    private void ScaleUp(Transform target)
    {
        Vector3 originalScale = _originalScales[target];
        LeanTween.scale(target.gameObject, 
            originalScale * scaleMultiplier, 
            tweenDuration).
            setEase(LeanTweenType.easeOutQuad);
    }

    private void ResetScale(Transform target)
    {
        if (_originalScales.TryGetValue(target, out Vector3 originalScale))
        {
            LeanTween.scale(target.gameObject, 
                originalScale, 
                tweenDuration).
                setEase(LeanTweenType.easeOutQuad);
        }
    }
}
