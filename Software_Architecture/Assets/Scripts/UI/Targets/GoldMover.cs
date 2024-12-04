using TMPro;
using UnityEngine;

public class GoldMover : MonoBehaviour
{
    private Vector3 positionToHold;
    private Camera _camera;

    private TextMeshProUGUI text;
    private int goldAmount;

    private void Start()
    {
        _camera = Camera.main;
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = "+" + goldAmount.ToString();
    }

    public void Initialize(Vector3 position, int amount)
    {
        positionToHold = position;
        goldAmount = amount;
    }

    private void Update()
    {
        UpdateTextPos();
    }

    private void UpdateTextPos()
    {
        // Update position
        positionToHold = positionToHold + (new Vector3(0, 0.75f, 0) * Time.deltaTime);
        Color c = text.color;
        c.a -= 0.75f * Time.deltaTime;
        text.color = c;

        // Set new position on screen
        Vector3 pos = _camera.WorldToScreenPoint(positionToHold);
        if (text.transform.position != pos)
        {
            text.transform.position = pos;
        }

        // Destroy when alpha is zero
        if (c.a <= 0) { Destroy(gameObject); }
    }
}
