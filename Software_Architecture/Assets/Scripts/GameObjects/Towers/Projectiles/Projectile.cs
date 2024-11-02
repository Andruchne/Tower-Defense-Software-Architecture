using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileMoveType { Straight, Curved }

    [SerializeField] GameObject impactPrefab;

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
            Instantiate(
                impactPrefab,
                new Vector3(transform.position.x, 0.2f, transform.position.z),
                Quaternion.identity,
                null);
        }
        else
        {
            Debug.LogError("Projectile Prefab " + gameObject.name + ": Prefab has no impact prefab attached");
        }

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
        // Variable could be exposed, but no need for it for our purposes 
        float curveHeight = 0.5f;

        Vector3 startPos = transform.position;

        // Horizontal movmement
        LeanTween.move(gameObject, targetPos, duration).setEase(LeanTweenType.linear);

        // Vertical movement for the curve
        LeanTween.value(gameObject, startPos.y, targetPos.y, duration).setOnUpdate((float y) =>
        {
            Vector3 pos = transform.position;

            // Calculate the height for the curve
            float height = Mathf.Sin(Mathf.PI * (pos.x - startPos.x) / (targetPos.x - startPos.x)) * curveHeight;
            transform.position = new Vector3(pos.x, startPos.y + height, pos.z);

            Vector3 direction = (targetPos - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion xAxisRotation = Quaternion.Euler(-30, 0, 0);

                transform.rotation = targetRotation * xAxisRotation;
            }
        }).setEase(LeanTweenType.easeOutQuad)
        // Execute Impact() when move is finished
        .setOnComplete(() => { Impact(); });
    }
}