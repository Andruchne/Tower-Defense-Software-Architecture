using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileMoveType { Straight, Curved }

    [SerializeField] GameObject impactPrefab;

    private CurrentTower _currentTower;

    public void Initialize(CurrentTower currentTower)
    {
        _currentTower = currentTower;
    }

    public void Shoot(Vector3 targetPos, ProjectileMoveType moveType, float duration)
    {
        switch (moveType)
        {
            case ProjectileMoveType.Straight:
                {
                    MoveStraight(targetPos, duration);
                    break;
                }
            case ProjectileMoveType.Curved:
                {
                    MoveCurved(targetPos, duration);
                    break;
                }
        }
    }

    protected void Impact()
    {
        if (impactPrefab != null)
        {
            // Ground height is 0.2, we're assuming it stays this way
            Impact impact = Instantiate(
                impactPrefab,
                new Vector3(transform.position.x, 0.2f, transform.position.z),
                Quaternion.identity,
                null).GetComponent<Impact>();

            if (impact != null) { impact.Initialize(_currentTower); }
            else { Debug.LogError("Impact Prefab " + impactPrefab.name + ": Prefab has no impact script attached"); }
        }
        else { Debug.LogError("Projectile Prefab " + gameObject.name + ": Prefab has no impact prefab attached"); }

        Destroy(gameObject);
    }

    // Different move behaviour
    public void MoveStraight(Vector3 targetPos, float duration)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Make the projectile turn to the target position
        transform.rotation = targetRotation;

        LeanTween.move(gameObject, targetPos, duration)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() => { Impact(); });
    }

    public void MoveCurved(Vector3 targetPos, float duration)
    {
        float curveHeight = 1.0f;
        Vector3 startPos = transform.position;

        // Use LeanTween to animate the position and rotation
        LeanTween.value(gameObject, 0, 1, duration).setOnUpdate((float t) =>
        {
            // Calculate position with parabolic curve
            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            float parabola = 4 * curveHeight * t * (1 - t);
            pos.y = Mathf.Lerp(startPos.y, targetPos.y, t) + parabola;

            // Update the position
            transform.position = pos;

            // Calculate direction based on the movement of `t` along the curve
            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, t + Time.deltaTime / duration);
            float nextParabola = 4 * curveHeight * (t + Time.deltaTime / duration) * (1 - (t + Time.deltaTime / duration));
            nextPos.y = Mathf.Lerp(startPos.y, targetPos.y, t + Time.deltaTime / duration) + nextParabola;

            // Look towards the next position on the curve
            Vector3 direction = (nextPos - pos).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
            }
        }).setEase(LeanTweenType.linear)
        .setOnComplete(() => { Impact(); });
    }

}