using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        OpenCloseMenu();
        UpdateMenuPos();
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
            }
        }
        else if (Input.GetMouseButtonDown(0) && _isMenuOpen && !MenuClicked())
        {
            OnMenuClosed?.Invoke();
            Destroy(_currentMenuCanvas);
            _currentMenuCanvas = null;
            _isMenuOpen = false;
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

    public GameObject GetCurrentMenu()
    {
        return _currentMenuCanvas;
    }
}
