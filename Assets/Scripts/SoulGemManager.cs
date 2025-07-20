using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class SoulGemManager : ObjectBehaviour
{
    public static SoulGemManager Instance { get; private set; }
    private List<SoulGem> _soulGems = new List<SoulGem>();
    private float _gemSpawnRadius = 0.1f;
    private float _attractionRadius = 1.8f;
    private float _absorbRadius = 0.15f;
    private float _soulGemAttractionSpeed = 6f;
    public static event Action OnSoulGemCollected;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnemyManager.OnEnemyDeath += OnEnemyDeath;
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemyDeath -= OnEnemyDeath;
        Instance = null;
    }

    private void OnEnemyDeath(BaseEnemy enemy)
    {
        for (int i = 0; i < enemy.PowerReward; i++)
        {
            var rndOfset = Random.insideUnitCircle * _gemSpawnRadius;
            var gemGO = Instantiate(Game.Assets.SoulGemPrefab, enemy.Transform.position + new Vector3(rndOfset.x, 0, rndOfset.y), Quaternion.LookRotation(Random.insideUnitSphere - enemy.Transform.position), this.transform);
            var soulGem = gemGO.GetComponent<SoulGem>();
            soulGem.Init();
            _soulGems.Add(soulGem);
        }
    }

    private void Update()
    {
        List<SoulGem> gemsToRemove = new List<SoulGem>();
        foreach (SoulGem soulGem in _soulGems)
        {
            float distance = Vector3.Distance(soulGem.Transform.position, Player.Instance.Transform.position);
            
            if (distance < _attractionRadius)
            {
                var direction = (Player.Instance.Transform.position - soulGem.Transform.position).normalized;

                soulGem.Rigidbody.AddForce(_soulGemAttractionSpeed * direction, ForceMode.Force);
                //soulGem.Transform.position = Vector3.Lerp(soulGem.Transform.position, Player.Instance.Transform.position, Time.deltaTime * _soulGemAttractionSpeed);
                distance = Vector3.Distance(soulGem.Transform.position, Player.Instance.Transform.position);

                if (distance < _absorbRadius)
                {
                    OnSoulGemCollected?.Invoke();
                    Destroy(soulGem.gameObject);
                    gemsToRemove.Add(soulGem);
                }
            }
        }

        foreach (SoulGem gem in gemsToRemove)
        {
            _soulGems.Remove(gem);
        }
    }
}
