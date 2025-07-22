using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : ObjectBehaviour
{
    public static SpawnManager Instance { get; private set; }
    [SerializeField] private List<Spawner> _spawnPoints = new List<Spawner>();

    private float _time;
    private float _maxDifficulty = 500f;
    private float _doubleWavesStartTime = 600f;
    private float _doubleWavesMaxDifficultyTime = 1000f;
    private float _minChanceForDoubleWaves = 0.1f; 
    private float _maxChanceForDoubleWaves = 0.98f;

    private float _corruptionWavesStartTime = 800f;
    private float _corruptionMaxDifficultyTime = 1800f;
    private float _minChanceForCorruption = 0f;
    private float _maxChanceForCorruption = 1f;

    private float _corruptionEnemiesStartTime = 700f;
    private float _corruptEnemiesMaxDifficultyTime = 1800f;
    private float _minChanceForCorruptEnemies = 0f;
    private float _maxChanceForCorruptEnemies = .5f;

    private float _chanceForDoubleWaves;
    private float _chanceForCorruptionWave;
    private float _chanceForCorruptEnemy;
    private float _nextSpawnTime = 3f;
    private int _waveCount;

    public float GetTime
    {
        get { return _time; }
    }

    public void AddTime(float time)
    {
        _time += time;
        _nextSpawnTime += time;
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

    private float _spawnDelayAddedPerEnemyMin_LowDiff = 1.4f;
    private float _spawnDelayAddedPerEnemyMax_LowDiff = 2.5f;
    private float _spawnDelayAddedPerEnemyMin_HighDiff = .085f;
    private float _spawnDelayAddedPerEnemyMax_HighDiff = .28f;

    private int _numEnemiesMin_LowDiff = 1;
    private int _numEnemiesMax_LowDiff = 2;
    private int _numEnemiesMin_HighDiff = 27;
    private int _numEnemiesMax_HighDiff = 43;

    private bool _stopSpawning = false;

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
        if (_stopSpawning) return;

        HandleSpawning();
    }

    private void HandleSpawning()
    {
        if (Game.IsPlayerDead)
        {
            _stopSpawning = true;
            // No return, let it spawn once more, cuz why not...
        }

        _time += Time.deltaTime;

        if (_time >= _nextSpawnTime)
        {
            float difficulty = _time / _maxDifficulty;

            // Interpolate spawn delay and enemy count based on difficulty
            float spawnDelayMin = Mathf.Lerp(_spawnDelayAddedPerEnemyMin_LowDiff, _spawnDelayAddedPerEnemyMin_HighDiff, difficulty);
            float spawnDelayMax = Mathf.Lerp(_spawnDelayAddedPerEnemyMax_LowDiff, _spawnDelayAddedPerEnemyMax_HighDiff, difficulty);
            int numEnemiesMin = Mathf.RoundToInt(Mathf.LerpUnclamped(_numEnemiesMin_LowDiff, _numEnemiesMin_HighDiff, difficulty));
            int numEnemiesMax = Mathf.RoundToInt(Mathf.LerpUnclamped(_numEnemiesMax_LowDiff, _numEnemiesMax_HighDiff, difficulty));

            bool isDoubleWave = false;
            bool isCorruptWave = false;
            if (_time >= _doubleWavesStartTime)
            {
                _chanceForDoubleWaves = Mathf.Lerp(_minChanceForDoubleWaves, _maxChanceForDoubleWaves, (_time - _doubleWavesStartTime) / (_doubleWavesMaxDifficultyTime - _doubleWavesStartTime));
                isDoubleWave = Random.value < _chanceForDoubleWaves;
            }

            if (_time >= _corruptionWavesStartTime)
            {
                _chanceForCorruptionWave = Mathf.Lerp(_minChanceForCorruption, _maxChanceForCorruption, (_time - _corruptionWavesStartTime) / (_corruptionMaxDifficultyTime - _corruptionWavesStartTime));
                isCorruptWave = Random.value < _chanceForCorruptionWave;
            }

            int numEnemies = Random.Range(numEnemiesMin, numEnemiesMax + 1);

            if (isDoubleWave)
            {
                //Debug.Log($"Double wave spawned at time: {_time}, Wave Count: {_waveCount + 1}");
                numEnemies *= 2; // Double the number of enemies for double wave
            }

            // Add spawn delay for each enemy spawned
            _nextSpawnTime = _time;
            float spawnDelay = 0f;
            for (int i = 0; i < numEnemies; i++)
            {
                spawnDelay += isDoubleWave ? Random.Range(spawnDelayMin, spawnDelayMax) * 0.6f : Random.Range(spawnDelayMin, spawnDelayMax);
            }

            _nextSpawnTime += spawnDelay;

            SpawnWave(numEnemies, difficulty, isCorruptWave);
        }
    }

    private void SpawnWave(int numEnemies, float difficulty, bool isCorruptWave)
    {
        if (_spawnPoints.Count == 0 || _enemyTypes.Count == 0) return;

        _waveCount++;

        for (int i = 0; i < numEnemies; i++)
        {
            var spawner = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
            var enemyType = GetWeightedRandomEnemyType(difficulty);
            bool corrupt = isCorruptWave;

            if (_time >= _corruptionEnemiesStartTime && !corrupt)
            {
                _chanceForCorruptEnemy = Mathf.Lerp(_minChanceForCorruptEnemies, _maxChanceForCorruptEnemies, (_time - _corruptionEnemiesStartTime) / (_corruptEnemiesMaxDifficultyTime - _corruptionEnemiesStartTime));
                corrupt = Random.value < _chanceForCorruptEnemy;
            }

            spawner.PlaySpawnAnimation();

            if (enemyType == EnemyType.Fodder)
            {
                EnemyManager.Instance.SpawnFodderEnemy(spawner.SpawnPoint.position, corrupt);
            }

            if (enemyType == EnemyType.Heavy)
            {
                EnemyManager.Instance.SpawnHeavyEnemy(spawner.SpawnPoint.position, corrupt);
            }

            if (enemyType == EnemyType.Quick)
            {
                EnemyManager.Instance.SpawnQuickEnemy(spawner.SpawnPoint.position, corrupt);
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
