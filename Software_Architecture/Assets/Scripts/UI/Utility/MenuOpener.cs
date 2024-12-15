using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Handles opening any menu canvas, at the position of the clicked object
/// </summary>

// Keep in mind: This script only works if the object it is attached to, is in the selectable layer
public class MenuOpener : MonoBehaviour
{
    public event Action OnMenuOpened;
    public event Action OnMenuClosed;

    [SerializeField] GameObject menuPrefab;

    private Camera _camera;
    private GameObject _currentMenuCanvas;

    // To check mouse position for UI
    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    private bool _isMenuOpen;
    private bool _clickable = true;

    // Used to switch between selectable and unselectable states
    private LayerMask _selectableIndex;

    private void Start()
    {
        _camera = Camera.main;
        _selectableIndex = LayerMask.NameToLayer("Selectable");

        EventBus<OnStartedBreakTime>.OnEvent += ActivateMenu;
        EventBus<OnStopBreakTime>.OnEvent += DeactivateMenu;

        EventBus<OnCameraMoved>.OnEvent += MoveUIWithCamera;
    }

    private void OnDestroy()
    {
        EventBus<OnStartedBreakTime>.OnEvent -= ActivateMenu;
        EventBus<OnStopBreakTime>.OnEvent -= DeactivateMenu;

        EventBus<OnCameraMoved>.OnEvent -= MoveUIWithCamera;
    }

    void Update()
    {
        OpenCloseMenu();
    }

    private void DeactivateMenu(OnStopBreakTime onEvent)
    {
        SetClickable(false);

        OnMenuClosed?.Invoke();
        Destroy(_currentMenuCanvas);
        _currentMenuCanvas = null;
        _isMenuOpen = false;
    }

    private void ActivateMenu(OnStartedBreakTime onEvent)
    {
        SetClickable(true);
    }

    private void RemoveCanvas()
    {
        OnMenuClosed?.Invoke();
        Destroy(_currentMenuCanvas);
        _currentMenuCanvas = null;
        _isMenuOpen = false;
    }

    private void UpdateMenuPos()
    {
        // To keep the window at the position it's supposed to be
        if (_currentMenuCanvas != null)
        {
            Vector3 pos = _camera.WorldToScreenPoint(transform.position);

            for (int i = 0; i < _currentMenuCanvas.transform.childCount; i++)
            {
                if (_currentMenuCanvas.transform.GetChild(i).position != pos)
                {
                    _currentMenuCanvas.transform.GetChild(i).position = pos;
                }
            }
        }
    }

    private void OpenCloseMenu()
    {
        if (!_clickable) { return; }

        if (Input.GetMouseButtonDown(0) && IsMouseOverObject())
        {
            if (menuPrefab != null && _currentMenuCanvas == null)
            {
                _currentMenuCanvas = Instantiate(menuPrefab, transform);

                // Get needed elements in order to check for UI mouse input
                _graphicRaycaster = _currentMenuCanvas.GetComponent<GraphicRaycaster>();
                _eventSystem = EventSystem.current;

                _isMenuOpen = true;
                OnMenuOpened?.Invoke();
                SetSelectability(false);

                UpdateMenuPos();
            }
        }
        else if (Input.GetMouseButtonDown(0) && _isMenuOpen && !MenuClicked())
        {
            RemoveCanvas();
            SetSelectability(true);
        }
    }

    private bool IsMouseOverObject()
    {
        int layerMask = LayerMask.GetMask("Selectable");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
            else if (MenuClicked())
            {
                return true;
            }
        }

        return false;
    }

    private bool MenuClicked()
    {
        if (_currentMenuCanvas != null)
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(_pointerEventData, results);

            List<GameObject> children = Useful.GetAllChildren(_currentMenuCanvas.transform);

            foreach (RaycastResult result in results)
            {
                if (children.Contains(result.gameObject))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void MoveUIWithCamera(OnCameraMoved onCameraMoved)
    {
        UpdateMenuPos();
    }

    public GameObject GetCurrentMenu()
    {
        return _currentMenuCanvas;
    }

    public void SetClickable(bool clickable)
    {
        _clickable = clickable;
        SetSelectability(clickable);
    }

    private void SetSelectability(bool selectable)
    {
        if (selectable) { gameObject.layer = _selectableIndex; }
        else { gameObject.layer = 0; }
    }
}
