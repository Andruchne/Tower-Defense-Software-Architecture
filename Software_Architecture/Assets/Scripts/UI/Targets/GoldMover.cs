using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Moves the GoldGained Canvas up from the spawn position, removing it when hitting transparency
/// </summary>
public class GoldMover : MonoBehaviour
{
    private Vector3 _positionToHold;
    private Camera _camera;

    private Transform _goldToMove;
    private TextMeshProUGUI _text;
    private Image _image;

    private int _goldAmount;

    private void Start()
    {
        _camera = Camera.main;
        _goldToMove = transform.GetChild(0);

        _text = _goldToMove.GetChild(0).GetComponent<TextMeshProUGUI>();
        _image = _goldToMove.GetChild(1).GetComponent<Image>();

        _text.text = "+" + _goldAmount.ToString();

        UpdateTextPos();
    }

    public void Initialize(Vector3 position, int amount)
    {
        _positionToHold = position;
        _goldAmount = amount;
    }

    private void Update()
    {
        UpdateTextPos();
    }

    private void UpdateTextPos()
    {
        // Update position
        _positionToHold = _positionToHold + (new Vector3(0, 0.75f, 0) * Time.deltaTime);
        Color c = _text.color;
        c.a -= 0.75f * Time.deltaTime;
        _text.color = c;
        _image.color = c;

        // Set new position on screen
        Vector3 pos = _camera.WorldToScreenPoint(_positionToHold);
        if (_goldToMove.position != pos)
        {
            _goldToMove.transform.position = pos;
        }

        // Destroy when alpha is zero
        if (c.a <= 0) { Destroy(gameObject); }
    }
}
