using UnityEngine;

public class TowerConfigButton : MonoBehaviour
{
    [SerializeField] GameObject confirmDestructPrefab;

    private TowerConfigSelection _towerConfigSelect;
    private Camera _camera;

    private TowerConfirmDestroyButton _currentConfirmWindow;


    private void Start()
    {
        _towerConfigSelect = GetComponentInParent<TowerConfigSelection>();
        _camera = Camera.main;
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
        _towerConfigSelect.InvokeDestruct();
    }

    private void CancelDestruct()
    {
        UnbindConfirmWindowEvents();
        Destroy(_currentConfirmWindow.gameObject);
    }

    public void UpgradeClicked()
    {
        _towerConfigSelect.InvokeUpgrade();
        Destroy(_towerConfigSelect.gameObject);
    }

    public void DestructClicked()
    {
        _currentConfirmWindow = Instantiate(confirmDestructPrefab)
            .GetComponent<TowerConfirmDestroyButton>();

        _currentConfirmWindow.OnConfirmDestroy += DestructConfirmed;
        _currentConfirmWindow.OnCancelDestroy += CancelDestruct;
    }
}
