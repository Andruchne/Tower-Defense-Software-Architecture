using System;
using UnityEngine;

/// <summary>
/// Self made tweens, using the LeanTween library
/// Different classes use this, to do the same tween, e.g. TowerSlot, Tower, and Player (The Base)
/// </summary>

public class Tweens
{
    public event Action OnTweenComplete;

    public void EmergeWithShake(Transform transform, float riseHeight, float riseTime, float shakeIntensity = 0.025f, float shakeFrequency = 0.05f)
    {
        Vector3 originalPosition = transform.position;

        float shakeTimer = 0;

        // Move the object up a few shakes
        LeanTween.moveY(transform.gameObject, originalPosition.y + riseHeight, riseTime)
        .setEase(LeanTweenType.linear)
        .setOnUpdate((float value) => {

            shakeTimer += Time.deltaTime;

            if (shakeTimer >= shakeFrequency)
            {
                // Generate random offsets for X and Z axes to shake
                float shakeOffsetX = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
                float shakeOffsetZ = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);

                transform.position = new Vector3(
                    originalPosition.x + shakeOffsetX,
                    transform.position.y,
                    originalPosition.z + shakeOffsetZ);

                // Reset timer for next shake
                shakeTimer = 0f;
            }
        })
        .setOnComplete(() => {
            // Reset X and Z positions to original pos
            transform.position = new Vector3(originalPosition.x, transform.position.y, originalPosition.z);
            OnTweenComplete?.Invoke();
        });
    }

    public void SubmergeWithShake(Transform transform, float sinkHeight, float sinkTime, float shakeIntensity = 0.025f, float shakeFrequency = 0.05f)
    {
        Vector3 originalPosition = transform.position;

        float shakeTimer = 0;

        // Move the object up a few shakes
        LeanTween.moveY(transform.gameObject, originalPosition.y - sinkHeight, sinkTime)
        .setEase(LeanTweenType.linear)
        .setOnUpdate((float value) => {

            shakeTimer += Time.deltaTime;

            if (shakeTimer >= shakeFrequency)
            {
                // Generate random offsets for X and Z axes to shake
                float shakeOffsetX = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
                float shakeOffsetZ = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);

                transform.position = new Vector3(
                    originalPosition.x + shakeOffsetX,
                    transform.position.y,
                    originalPosition.z + shakeOffsetZ);

                // Reset timer for next shake
                shakeTimer = 0f;
            }
        })
        .setOnComplete(() => {
            // Reset X and Z positions to original pos
            transform.position = new Vector3(originalPosition.x, transform.position.y, originalPosition.z);
            OnTweenComplete?.Invoke();
        });
    }

    public void Shake(Transform transform, float shakeTime, float shakeIntensity = 0.025f, float shakeFrequency = 0.05f)
    {
        Vector3 originalPosition = transform.position;
        float shakeTimer = 0;

        // Use LeanTween to handle shaking over time
        LeanTween.value(transform.gameObject, 0, shakeTime, shakeTime)
            .setOnUpdate((float deltaTime) =>
            {
                shakeTimer += Time.deltaTime;

                if (shakeTimer >= shakeFrequency)
                {
                    // Generate random offsets for X and Z axes
                    float shakeOffsetX = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
                    float shakeOffsetZ = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);

                    transform.position = new Vector3(
                        originalPosition.x + shakeOffsetX,
                        originalPosition.y,
                        originalPosition.z + shakeOffsetZ);

                    // Reset shake timer
                    shakeTimer = 0f;
                }
            })
            .setOnComplete(() =>
            {
                // Reset position to original after shaking
                transform.position = originalPosition;
            });
    }
}
