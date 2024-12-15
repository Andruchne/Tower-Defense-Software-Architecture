using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] Level levelWave;

    private GameObject[] _spawnPoints;
    private Timer _timer;

    // X = Current Wave || Y = Current EnemyGroup || Z = Current Count of EnemyGroup
    private Vector3 _currentProgress = new Vector3();

    // To check if all enemies have been defeated in level
    private int _targetsInLevel;

    private bool _allEnemiesSpawned;
    private bool _playerDefeated;

    private void Start()
    {
        if (levelWave == null)
        {
            Debug.Log("WaveManager: No level provided to spawn waves. Destroying Manager...");
            Destroy(gameObject);
        }

        _timer = transform.AddComponent<Timer>();
        _timer.OnTimerFinished += SpawnEnemy;

        _timer.Initialize(levelWave.intervalPerSpawn[0], true);
        EventBus<OnStopBreakTime>.OnEvent += StartTimer;
        EventBus<OnPlayerHUDLoaded>.OnEvent += PlayerHUDLoaded;
        EventBus<OnPlayerDefeatedEvent>.OnEvent += DeactivateWaves;

        GetAllSpawnPoints();
        SafetyChecks();
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerFinished -= SpawnEnemy;
        }

        EventBus<OnStopBreakTime>.OnEvent -= StartTimer;
        EventBus<OnPlayerHUDLoaded>.OnEvent -= PlayerHUDLoaded;
        EventBus<OnPlayerDefeatedEvent>.OnEvent -= DeactivateWaves;
    }

    private void PlayerHUDLoaded(OnPlayerHUDLoaded onPlayerHUDLoaded)
    {
        // Keep UI up to date
        EventBus<OnUpdateCurrentWave>.Publish(new OnUpdateCurrentWave(1, levelWave.waves.Length));
    }

    private void Update()
    {
        CheckIfEnemiesDefeated();
    }

    private void CheckIfEnemiesDefeated()
    {
        if (_allEnemiesSpawned && !_playerDefeated)
        {
            if (_targetsInLevel <= 0)
            {
                _allEnemiesSpawned = false;

                if (IsLevelFinished((int)_currentProgress.x))
                {
                    EventBus<OnLevelWon>.Publish(new OnLevelWon());
                    return;
                }

                // Give the player a break
                EventBus<OnQueueUpBreakTime>.Publish(new OnQueueUpBreakTime());
                EventBus<OnUpdateCurrentWave>.Publish(new OnUpdateCurrentWave((int)_currentProgress.x + 1, levelWave.waves.Length));
            }
        }
    }

    private void SafetyChecks()
    {
        if (_spawnPoints.Length <= 0)
        {
            Debug.LogError("WaveManager: No spawnPoints found in Scene. Destroying manager...");
            Destroy(this);
        }
        else if (levelWave == null)
        {
            Debug.LogError("WaveManager: No level object was given. Destroying manager...");
            Destroy(this);
        }
    }

    private void GetAllSpawnPoints()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }
    private void SpawnEnemy()
    {
        int randomPos = Random.Range(0, _spawnPoints.Length);
        Transform spawnPos = _spawnPoints[randomPos].transform;

        GameObject enemyPrefab = levelWave.waves[(int)_currentProgress.x].
            enemyGroups[(int)_currentProgress.y].
            enemy.enemyPrefab;

        ITargetable target = Instantiate(enemyPrefab,spawnPos.position, spawnPos.rotation)
            .GetComponent<ITargetable>();

        _targetsInLevel++;
        target.OnTargetDestroyed += DiscardTarget;

        NextStage();
    }

    private void DiscardTarget(ITargetable target)
    {
        target.OnTargetDestroyed -= DiscardTarget;
        _targetsInLevel--;
    }

    private void NextStage()
    {
        int currentX = (int)_currentProgress.x;
        int currentY = (int)_currentProgress.y;
        int currentZ = (int)_currentProgress.z;
        currentZ++;

        // If all enemies of array are spawned, move onto next batch
        int enemyCount = levelWave.waves[(int)_currentProgress.x].enemyGroups[(int)_currentProgress.y].count;

        if (currentZ >= enemyCount)
        {
            currentZ = 0;
            currentY++;
        }
        
        // Check if wave is finished
        int countOfEnemyGroups = levelWave.waves[(int)_currentProgress.x].enemyGroups.Length;
        if (currentY >= countOfEnemyGroups)
        {
            currentY = 0;
            currentX++;

            _timer.StopTimer();

            if (currentX < levelWave.waves.Length)
            {
                _timer.SetWaitTime(levelWave.intervalPerSpawn[currentX]);
            }

            _allEnemiesSpawned = true;
        }

        // Update current progress
        _currentProgress = new Vector3(currentX, currentY, currentZ);
    }

    private bool IsLevelFinished(int currentWave)
    {
        // If all waves have been done
        int countOfWaves = levelWave.waves.Length;
        if (currentWave >= countOfWaves)
        {
            return true;
        }

        return false;
    }

    private void StartTimer(OnStopBreakTime onStopBreakTime)
    {
        _timer.ResetTimer(true);
    }

    private void DeactivateWaves(OnPlayerDefeatedEvent onPlayerDefeatedEvent)
    {
        _playerDefeated = true;
        _timer.StopTimer();
    }

    public bool IsActive()
    {
        return _timer.GetActive();
    }
}
