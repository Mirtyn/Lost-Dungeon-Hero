using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : ObjectBehaviour
{
    public static SpawnManager Instance { get; private set; }
    [SerializeField] private List<Spawner> _spawnPoints = new List<Spawner>();

    private float _time;
    private float _nextSpawnTime = 3f;
    private int _waveCount;

    public float GetTime
    {
        get { return _time; }
    }

    [System.Serializable]
    public class EnemySpawnInfo
    {
        public EnemyType EnemyType;
        public float Weight_LowDiff = 1f;
        public float Weight;
        public float Weight_HighDiff = 2f;
    }

    [SerializeField] private List<EnemySpawnInfo> _enemyTypes = new List<EnemySpawnInfo>();

    private float _spawnDelayAddedPerEnemyMin_LowDiff = 1.2f;
    private float _spawnDelayAddedPerEnemyMax_LowDiff = 2.5f;
    private float _spawnDelayAddedPerEnemyMin_HighDiff = .3f;
    private float _spawnDelayAddedPerEnemyMax_HighDiff = 1.2f;

    private int _numEnemiesMin_LowDiff = 1;
    private int _numEnemiesMax_LowDiff = 4;
    private int _numEnemiesMin_HighDiff = 15;
    private int _numEnemiesMax_HighDiff = 25;

    private void Awake()
    {
        Instance = this;
    }

    public void AddSpawnPoint(Spawner spawnPoint)
    {
        _spawnPoints.Add(spawnPoint);
    }

    private void Update()
    {
        _time += Time.deltaTime;

        if (_time >= _nextSpawnTime)
        {
            // 5 min (300 sec)
            float difficulty = Mathf.Clamp01(_time / 300f);

            // Interpolate spawn delay and enemy count based on difficulty
            float spawnDelayMin = Mathf.Lerp(_spawnDelayAddedPerEnemyMin_LowDiff, _spawnDelayAddedPerEnemyMin_HighDiff, difficulty);
            float spawnDelayMax = Mathf.Lerp(_spawnDelayAddedPerEnemyMax_LowDiff, _spawnDelayAddedPerEnemyMax_HighDiff, difficulty);
            int numEnemiesMin = Mathf.RoundToInt(Mathf.Lerp(_numEnemiesMin_LowDiff, _numEnemiesMin_HighDiff, difficulty));
            int numEnemiesMax = Mathf.RoundToInt(Mathf.Lerp(_numEnemiesMax_LowDiff, _numEnemiesMax_HighDiff, difficulty));

            int numEnemies = Random.Range(numEnemiesMin, numEnemiesMax + 1);

            // Add spawn delay for each enemy spawned
            _nextSpawnTime = _time;
            for (int i = 0; i < numEnemies; i++)
            {
                _nextSpawnTime += Random.Range(spawnDelayMin, spawnDelayMax);
            }

            SpawnWave(numEnemies, difficulty);
        }
    }

    private void SpawnWave(int numEnemies, float difficulty)
    {
        if (_spawnPoints.Count == 0 || _enemyTypes.Count == 0) return;

        _waveCount++;

        for (int i = 0; i < numEnemies; i++)
        {
            var spawner = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
            var enemyType = GetWeightedRandomEnemyType(difficulty);

            spawner.PlaySpawnAnimation();

            if (enemyType == EnemyType.Fodder)
            {
                EnemyManager.Instance.SpawnFodderEnemy(spawner.SpawnPoint.position);
            }

            if (enemyType == EnemyType.Heavy)
            {
                EnemyManager.Instance.SpawnHeavyEnemy(spawner.SpawnPoint.position);
            }

            if (enemyType == EnemyType.Quick)
            {
                EnemyManager.Instance.SpawnQuickEnemy(spawner.SpawnPoint.position);
            }
        }
    }

    private EnemyType GetWeightedRandomEnemyType(float difficulty)
    {
        foreach (var enemyType in _enemyTypes)
        {
            enemyType.Weight = Mathf.Lerp(enemyType.Weight_LowDiff, enemyType.Weight_HighDiff, difficulty);

            if (enemyType.Weight < 0f) enemyType.Weight = 0f;
        }

        float totalWeight = _enemyTypes.Sum(e => e.Weight);
        float randomValue = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var enemyType in _enemyTypes)
        {
            cumulative += enemyType.Weight;
            if (randomValue <= cumulative)
                return enemyType.EnemyType;
        }

        // Fallback in case no type was selected (should not happen)
        return _enemyTypes[0].EnemyType;
    }
}

public enum EnemyType
{
    Fodder,
    Heavy,
    Quick,
}
