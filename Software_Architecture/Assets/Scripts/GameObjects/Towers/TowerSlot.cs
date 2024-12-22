using System.ComponentModel;
using UnityEngine;

/// <summary>
/// The tower slot, from which one can buy/select the desired tower
/// It instantiates the needed menu (TowerTypeSelection), listening to it's events
/// </summary>

public class TowerSlot : MonoBehaviour
{
    [SerializeField] GameObject towerMenuPrefab;
    [SerializeField] GameObject towerPrefab;

    [Description("Particle which is instantiated when tower emerges from the ground")]
    [SerializeField] ParticleSystem emergeParticle;

    [Description("Is only used when emerge is called on this slot")]
    [SerializeField] float riseTime = 1.0f;

    [Space]
    [Header("Unit Test Settings [ Leave empty if not needed ]")]
    [SerializeField] bool initializeAtStart;
    [SerializeField] TowerInfo tower2B;

    private TowerTypeSelection _towerTypeSelect;

    private MenuOpener _menuOpener;

    private Tweens _tween = new Tweens();

    private void Start()
    {
        _menuOpener = GetComponent<MenuOpener>();

        if (_menuOpener == null)
        {
            Debug.LogError("TowerSlot: MenuOpener script not attached to TowerSlot prefab. Destroying Slot...");
            Destroy(gameObject);
            return;
        }

        _menuOpener.OnMenuOpened += GetTypeSelection;
        _menuOpener.OnMenuClosed += RemoveTypeSelection;

        if (initializeAtStart) { TowerSelected(tower2B); }
    }

    private void OnDestroy()
    {
        _menuOpener.OnMenuOpened -= GetTypeSelection;
        _menuOpener.OnMenuClosed -= RemoveTypeSelection;
    }

    private void GetTypeSelection()
    {
        _towerTypeSelect = _menuOpener.GetCurrentMenu().GetComponent<TowerTypeSelection>();
        _towerTypeSelect.OnTypeSelected += TowerSelected;
    }

    private void RemoveTypeSelection()
    {
        if (_towerTypeSelect == null) { return; }

        _towerTypeSelect.OnTypeSelected -= TowerSelected;
        _towerTypeSelect = null;
    }

    private void TowerSelected(TowerInfo towerInfo)
    {
        // Instantiate tower holder
        Transform towerHolder = Instantiate(
            towerPrefab,
            transform.position,
            transform.rotation,
            transform.parent).transform;

        // Instantiate tower model seperately and add to tower holder
        Instantiate(
            towerInfo.towerModel[0],
            transform.position,
            Quaternion.identity,
            towerHolder.GetChild(0));

        Tower tower = towerHolder.GetComponent<Tower>();
        tower.Initialize(towerInfo);

        // For Unit Testing
        if (initializeAtStart) { tower.SetClickable(false); }

        Destroy(gameObject);
    }

    private void ReactivateClickable()
    {
        _menuOpener.SetClickable(true);
        _tween.OnTweenComplete -= ReactivateClickable;
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
        _tween.OnTweenComplete += ReactivateClickable;

        // In case it's called before Start()
        if (_menuOpener == null) 
        { 
            _menuOpener = GetComponent<MenuOpener>(); 
            if (_menuOpener == null) { return; }
        }
        _menuOpener.SetClickable(false);
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