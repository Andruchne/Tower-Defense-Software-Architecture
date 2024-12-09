using UnityEngine;

/// <summary>
/// Makes the camera move based on the mouse position along the border
/// It maps the speed, based on the distance to the borders
/// Bounds are there to keep the camera inside an area
/// </summary>

public class CameraMover : MonoBehaviour
{
    [SerializeField] Vector2 minCameraBounds;
    [SerializeField] Vector2 maxCameraBounds;
    [Space]
    [SerializeField] float moveDistance = 20.0f;
    [SerializeField] float maxSpeed = 10.0f;
    [SerializeField] float damping = 5.0f;

    private Vector3 _moveSpeed;

    private Vector3 _mousePosition;
    private Vector2 _screenSize;

    private float _leftDistance;
    private float _rightDistance;
    private float _topDistance;
    private float _bottomDistance;

    private bool _active;

    private void Start()
    {
        _screenSize = new Vector2(Screen.width, Screen.height);
        EventBus<OnLevelLoadedEvent>.OnEvent += Activate;
    }

    private void OnDestroy()
    {
        EventBus<OnLevelLoadedEvent>.OnEvent -= Activate;
    }

    private void Update()
    {
        if (!_active) { return; }

        CheckMove();
        MoveCamera();
    }

    private void GetMouseDistance()
    {
        _mousePosition = Input.mousePosition;

        _rightDistance = _screenSize.x - _mousePosition.x;
        _leftDistance = _mousePosition.x;
        _topDistance = _screenSize.y - _mousePosition.y;
        _bottomDistance = _mousePosition.y;
    }

    private void CheckMove()
    {
        GetMouseDistance();

        Vector2 moveVector = Vector2.zero;

        moveVector.x += GetMappedSpeed(_rightDistance);
        moveVector.x -= GetMappedSpeed(_leftDistance);

        moveVector.y += GetMappedSpeed(_topDistance);
        moveVector.y -= GetMappedSpeed(_bottomDistance);

        Vector3 targetSpeed = new Vector3(moveVector.x, 0.0f, moveVector.y);

        _moveSpeed = Vector3.Lerp(_moveSpeed, targetSpeed, Time.deltaTime * damping);
    }

    private void MoveCamera()
    {
        Vector3 movement = _moveSpeed * Time.deltaTime;
        transform.position += movement;

        ClampPosition();
    }

    private void ClampPosition()
    {
        Vector3 min = new Vector3(minCameraBounds.x, transform.position.y, minCameraBounds.y);
        Vector3 max = new Vector3(maxCameraBounds.x, transform.position.y, maxCameraBounds.y);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, min.x, max.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, min.z, max.z)
        );
    }

    private float GetMappedSpeed(float value)
    {
        if (value >= moveDistance) { return 0; }
        else 
        {
            return maxSpeed * Mathf.Clamp01(1.0f - value / moveDistance);
        }
    }

    private void Activate(OnLevelLoadedEvent onLevelLoadedEvent)
    {
        _active = true;
    }
}
