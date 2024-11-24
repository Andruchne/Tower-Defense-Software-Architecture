using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSlot : MonoBehaviour
{
    [SerializeField] GameObject towerMenuPrefab;
    [SerializeField] GameObject towerPrefab;

    [Description("Particle which is instantiated when tower emerges from the ground")]
    [SerializeField] ParticleSystem emergeParticle;

    [Description("Is only used when emerge is called on this slot")]
    [SerializeField] float riseTime = 1.0f;

    // To position the menu correctly
    private Camera _camera;
    private GameObject _currentMenuCanvas;

    private TowerTypeSelection _towerTypeSelect;

    // To check mouse position for UI
    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    private bool _isMenuOpen;

    private Tweens _tween = new Tweens();

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        OpenCloseMenu();
        UpdateMenuPos();
    }

    private void OnDestroy()
    {
        if (_towerTypeSelect != null)
        {
            _towerTypeSelect.OnTypeSelected -= TowerSelected;
        }
    }

    private void OpenCloseMenu()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseOverObject())
        {
            if (towerMenuPrefab != null && _currentMenuCanvas == null)
            {
                _currentMenuCanvas = Instantiate(towerMenuPrefab, transform);

                // Subscribe to button click event
                _towerTypeSelect = _currentMenuCanvas.GetComponent<TowerTypeSelection>();
                _towerTypeSelect.OnTypeSelected += TowerSelected;

                // Get needed elements in order to check for UI mouse input
                _graphicRaycaster = _currentMenuCanvas.GetComponent<GraphicRaycaster>();
                _eventSystem = EventSystem.current;

                _isMenuOpen = true;
            }
        }
        else if (Input.GetMouseButtonDown(0) && _isMenuOpen && !TowerMenuClicked())
        {
            _towerTypeSelect.OnTypeSelected -= TowerSelected;

            Destroy(_currentMenuCanvas);
            _currentMenuCanvas = null;
            _towerTypeSelect = null;
            _isMenuOpen = false;
        }
    }

    private void UpdateMenuPos()
    {
        // To keep the window at the position it's supposed to be
        if (_currentMenuCanvas != null)
        {
            Vector3 pos = _camera.WorldToScreenPoint(transform.position);

            if (_currentMenuCanvas.transform.GetChild(0).position != pos)
            {
                _currentMenuCanvas.transform.GetChild(0).position = pos;
            }
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
            else if (TowerMenuClicked())
            {
                return true;
            }
        }

        return false;
    }

    private bool TowerMenuClicked()
    {
        if (_currentMenuCanvas != null)
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(_pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject == _currentMenuCanvas.transform.GetChild(0).gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void TowerSelected(TowerInfo towerInfo)
    {
        // Instantiate tower holder
        Transform towerHolder = Instantiate(
            towerPrefab,
            transform.position,
            transform.rotation).transform;

        // Instantiate tower model seperately and add to tower holder
        Instantiate(
            towerInfo.towerModel[0],
            transform.position,
            Quaternion.identity,
            towerHolder);

        Tower tower = towerHolder.GetComponent<Tower>();
        tower.Initialize(towerInfo);

        Destroy(gameObject);
    }

    public void MakeSlotEmerge()
    {
        // Do the shake effect
        float slotHeight = Useful.GetRenderedHeight(transform);
        float additionalOffset = 0.05f;
        float emergeDistance = slotHeight + additionalOffset;

        InstantiateDirtParticle(riseTime);

        transform.position = new Vector3(transform.position.x,
            transform.position.y - emergeDistance,
            transform.position.z);

        _tween.EmergeWithShake(transform, emergeDistance, riseTime);
    }

    private void InstantiateDirtParticle(float riseTime)
    {
        if (emergeParticle != null)
        {
            ParticleSystem[] particleSystems = emergeParticle.GetComponentsInChildren<ParticleSystem>();

            // Set the duration for all particle systems
            foreach (ParticleSystem particleSys in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particleSys.main;
                mainModule.duration = riseTime;
            }

            Instantiate(emergeParticle, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        }
        else { Debug.LogError("Tower Prefab: No emergeParticle to instantiate"); }
    }
}