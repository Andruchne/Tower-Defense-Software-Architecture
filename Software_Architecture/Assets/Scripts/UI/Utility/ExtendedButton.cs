using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // For IPointerEnterHandler and IPointerExitHandler

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
        Image[] childImages = GetComponentsInChildren<Image>();
        TextMeshProUGUI[] childTexts = GetComponentsInChildren<TextMeshProUGUI>();

        // Apply changes to image
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

        // Apply changes to texts
        foreach (TextMeshProUGUI text in childTexts)
        {
            Color textColor = text.color;

            if (tintColor.a < 1.0f)
            {
                // Dividing alpha, as text opacity is barely visible at higher values
                textColor.a = tintColor.a / 3.0f;
            }
            // Apply normal alpha, if it was changed prior
            else
            {
                textColor.a = tintColor.a;
            }

            text.color = textColor;
        }
    }
}