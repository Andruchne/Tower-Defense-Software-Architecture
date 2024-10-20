using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // For IPointerEnterHandler and IPointerExitHandler

/// <summary>
/// A normal UIButton, which also applies the button color effects to its children that hold an image object.
/// Additionally, it detects pointer hover events.
/// </summary>
public class ExtendedButton : Button
{
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        // Apply all the standard effects to the button
        base.DoStateTransition(state, instant);

        // Get the color depending on state and apply effect to all image children
        Color tintColor = Color.white;

        switch (state)
        {
            case SelectionState.Normal:
                tintColor = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                tintColor = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                tintColor = colors.pressedColor;
                break;
            case SelectionState.Disabled:
                tintColor = colors.disabledColor;
                break;
        }

        ApplyColorToChildImages(tintColor, instant);
    }

    private void ApplyColorToChildImages(Color tintColor, bool instant)
    {
        List<Image> childImages = new List<Image>(GetComponentsInChildren<Image>(true));

        // Don't repeat behavior on base image
        childImages.Remove(GetComponent<Image>());

        foreach (Image image in childImages)
        {
            if (instant)
            {
                image.color = tintColor;
            }
            else
            {
                image.CrossFadeColor(tintColor, colors.fadeDuration, true, true);
            }
        }
    }
}