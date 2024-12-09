using TMPro;
using UnityEngine;

/// <summary>
/// Manages the buttons of the screen to manage the tower (upgrade or destroy)
/// Also handles keeping the menu at the right position
/// </summary>

public class TowerConfigButton : MonoBehaviour
{
    [SerializeField] GameObject confirmDestructPrefab;

    private TowerConfigSelection _towerConfigSelect;
    private Camera _camera;

    private TowerConfirmDestroyButton _currentConfirmWindow;

    // For getting amount for refunding/upgrading
    private CurrentTower _currentTower;
    private int _refundAmount;

    private void Start()
    {
        _towerConfigSelect = GetComponentInParent<TowerConfigSelection>();
        _camera = Camera.main;

        // Not the prettiest way of doing it, but this way, we are informed about the current cost of the tower
        _currentTower = Useful.GetXthParentTransform(transform, 3).GetComponent<TowerUpgradeDescription>().GetCurrentTower();
        _refundAmount = _currentTower.info.cost[_currentTower.currentTier] / 2;
    }

    private void OnDestroy()
    {
        if (_currentConfirmWindow != null)
        {
            _currentConfirmWindow.OnConfirmDestroy -= DestructConfirmed;
            _currentConfirmWindow.OnCancelDestroy -= CancelDestruct;
            Destroy(_currentConfirmWindow.gameObject);
        }
    }

    private void Update()
    {
        UpdateConfirmWindowPos();
    }

    private void UpdateConfirmWindowPos()
    {
        if (_currentConfirmWindow != null)
        {
            Vector3 pos = _camera.WorldToScreenPoint(Useful.GetMostUpperTransform(transform).position);

            for (int i = 0; i < _currentConfirmWindow.transform.childCount; i++)
            {
                if (_currentConfirmWindow.transform.GetChild(i).position != pos)
                {
                    _currentConfirmWindow.transform.GetChild(i).position = pos;
                }
            }
        }
    }

    private void UnbindConfirmWindowEvents()
    {
        _currentConfirmWindow.OnConfirmDestroy -= DestructConfirmed;
        _currentConfirmWindow.OnCancelDestroy -= CancelDestruct;
    }

    private void DestructConfirmed()
    {
        UnbindConfirmWindowEvents();
        Destroy(_currentConfirmWindow.gameObject);
        Destroy(_towerConfigSelect.gameObject);
        EventBus<OnGetGoldEvent>.Publish(new OnGetGoldEvent(_refundAmount));
        _towerConfigSelect.InvokeDestruct();
    }

    private void CancelDestruct()
    {
        UnbindConfirmWindowEvents();
        Destroy(_currentConfirmWindow.gameObject);
    }

    public void UpgradeClicked()
    {
        EventBus<OnWithdrawGoldEvent>.Publish(new OnWithdrawGoldEvent(_currentTower.info.cost[_currentTower.currentTier]));
        _towerConfigSelect.InvokeUpgrade();
        Destroy(_towerConfigSelect.gameObject);
    }

    public void DestructClicked()
    {
        _currentConfirmWindow = Instantiate(confirmDestructPrefab)
            .GetComponent<TowerConfirmDestroyButton>();

        _currentConfirmWindow.Initialize(_refundAmount);

        _currentConfirmWindow.OnConfirmDestroy += DestructConfirmed;
        _currentConfirmWindow.OnCancelDestroy += CancelDestruct;
    }
}
