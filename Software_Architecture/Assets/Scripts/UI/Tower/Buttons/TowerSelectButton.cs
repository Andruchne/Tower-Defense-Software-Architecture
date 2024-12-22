using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This is the button script, for when a tower type is supposed to be selected
/// It handles notifying the slot about which tower to build
/// Checking if it's able to be selected in the first place, is also included here
/// </summary>

public class TowerSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject towerDescriptionPrefab;
    [SerializeField] TowerInfo towerInfo;
    [SerializeField] TextMeshProUGUI costText;

    // Used to position preview next to menu
    [SerializeField] RectTransform menu;

    private ExtendedButton _button;

    // To invoke onTypeSelected event
    private TowerTypeSelection _towerTypeSelect;

    // To position menu and check if to remove preview
    private GameObject _currentPreviewCanvas;
    private bool _removalActive;

    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    private void Start()
    {
        EventBus<OnGetGoldEvent>.OnEvent += CheckCostAndGold;

        _towerTypeSelect = GetComponentInParent<TowerTypeSelection>();
        if (_towerTypeSelect == null)
        {
            Debug.LogError("TowerSelectButton: Unable to get TowerTypeSelection script. Destroying Button...");
            Destroy(gameObject);
            return;
        }

        _button = GetComponent<ExtendedButton>();
        if (_button == null)
        {
            Debug.LogError("TowerSelectButton: Unable to get ExtendedButton script. Destroying Button...");
            Destroy(gameObject);
            return;
        }

        costText.text = towerInfo.cost[0].ToString();

        CheckButtonState();
    }

    private void Update()
    {
        CheckPreviewForRemoval();
        UpdatePreviewPos();
    }

    private void OnDestroy()
    {
        EventBus<OnGetGoldEvent>.OnEvent -= CheckCostAndGold;

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
            EventBus<OnWithdrawGoldEvent>.Publish(new OnWithdrawGoldEvent(int.Parse(costText.text)));
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
            // Position next to menu, calculating pos using width and global scale of menu
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

    private void CheckCostAndGold(OnGetGoldEvent onGoldEvent)
    {
        CheckButtonState();
    }

    private void CheckButtonState()
    {
        if (GameManager.Instance.GetPlayerGold() >= int.Parse(costText.text))
        {
            _button.interactable = true;
        }
        else { _button.interactable = false;  }
    }
}
