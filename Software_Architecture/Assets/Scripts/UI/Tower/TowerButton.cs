using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject towerDescriptionPrefab;
    [SerializeField] TowerInfo towerInfo;
    [SerializeField] RectTransform menu;

    // To invoke onTypeSelected event
    private TowerTypeSelection _towerTypeSelect;

    // To position menu and check if to remove preview
    private Camera _camera;
    private GameObject _currentPreviewCanvas;
    private bool _removalActive;

    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    private void Start()
    {
        _camera = Camera.main;
        _towerTypeSelect = GetComponentInParent<TowerTypeSelection>();
    }

    private void Update()
    {
        CheckPreviewForRemoval();
        UpdatePreviewPos();
    }

    private void OnDestroy()
    {
        if (_currentPreviewCanvas != null)
        {
            Destroy(_currentPreviewCanvas);
        }
    }

    public void ButtonClicked()
    {
        if (_towerTypeSelect != null)
        {
            _towerTypeSelect.InvokeTypeSelected(towerInfo);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (towerDescriptionPrefab != null && _currentPreviewCanvas == null)
        {
            _currentPreviewCanvas = Instantiate(towerDescriptionPrefab);

            TowerDescription towerD = _currentPreviewCanvas.GetComponent<TowerDescription>();
            towerD.SetInfo(towerInfo);

            // Get needed elements in order to check for UI mouse input
            _graphicRaycaster = _currentPreviewCanvas.GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
        }

        // Reset preview removal, when going from preview, back to button
        if (_removalActive) { _removalActive = false; }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Activate preview removal
        _removalActive = true;
    }

    private void UpdatePreviewPos()
    {
        if (_currentPreviewCanvas != null && menu != null)
        {
            Vector3 pos = menu.position + new Vector3(menu.rect.width * menu.lossyScale.x, 0, 0);

            // Update only if the position has changed
            if (_currentPreviewCanvas.transform.GetChild(0).position != pos)
            {
                _currentPreviewCanvas.transform.GetChild(0).position = pos;
            }
        }
    }

    private void CheckPreviewForRemoval()
    {
        if (_currentPreviewCanvas != null && _removalActive && !MouseOverPreview())
        {
            Destroy(_currentPreviewCanvas);

            _currentPreviewCanvas = null;
            _removalActive = false;
        }
    }

    private bool MouseOverPreview()
    {
        if (_currentPreviewCanvas != null)
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(_pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject == _currentPreviewCanvas.transform.GetChild(0).gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
