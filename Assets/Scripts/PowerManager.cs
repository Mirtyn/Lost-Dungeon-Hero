using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerManager : ObjectBehaviour
{
    public static PowerManager Instance { get; private set; }

    public PowerSlot PowerSlot1 { get; private set; } = new PowerSlot(PowerType.None, 0f);
    public PowerSlot PowerSlot2 { get; private set; } = new PowerSlot(PowerType.None, 0f);
    public PowerSlot PowerSlot3{ get; private set; } = new PowerSlot(PowerType.None, 0f);

    private Queue<PowerType> powerTypeBucketList = new Queue<PowerType>();
    private List<PowerType> powerTypes = new List<PowerType>
    {
        PowerType.Trap,
        PowerType.Trap,
        PowerType.Haste,
        PowerType.Invisibility,
        PowerType.FireBall,
        PowerType.FireBall,
        PowerType.RappidFire
    };

    public ActivePower ActivePower { get; private set; } = 0;

    private const float INVISIBILITY_DURATION = 10f;
    private const float HASTE_DURATION = 25f;
    private const float RAPIDFIRE_DURATION = 20f;

    private int _enemiesKilledMin = 32;
    private int _enemiesKilledMax = 42;
    private int _enemyKillsRequiredForPower;

    private float _pickupRadius = 0.4f;

    private List<Power> _powerGems = new List<Power>();

    public class PowerSlot
    {
        public PowerType PowerType { get; set; }
        public bool IsActive { get; set; }
        public float Duration { get; set; }
        public PowerSlot(PowerType powerType, float duration)
        {
            PowerType = powerType;
            Duration = duration;
        }
    }

    private void Awake()
    {
        Instance = this;
        _enemyKillsRequiredForPower = Random.Range(_enemiesKilledMin, _enemiesKilledMax + 1);
        ShuffleBucketQueue();
    }

    private void Start()
    {
        EnemyManager.OnEnemyDeath += OnEnemyKilled;
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemyDeath -= OnEnemyKilled;
    }

    private void ShuffleBucketQueue()
    {
        List<PowerType> shuffledList = powerTypes.OrderBy(x => Random.value).ToList();
        powerTypeBucketList = new Queue<PowerType>(shuffledList);
    }

    private void OnEnemyKilled(BaseEnemy enemy)
    {
        _enemyKillsRequiredForPower--;
        if (_enemyKillsRequiredForPower <= 0)
        {
            _enemyKillsRequiredForPower = Random.Range(_enemiesKilledMin, _enemiesKilledMax + 1);
            DropRandomPower(enemy.Transform.position);
        }
    }

    private void DropRandomPower(Vector3 pos)
    {
        int randomPower = Random.Range(0, 5);

        PowerType powerType = powerTypeBucketList.Dequeue();
        if (powerTypeBucketList.Count == 0)
        {
            ShuffleBucketQueue();
        }

        var gemGO = Instantiate(Game.Assets.PowerGemPrefab, pos, Quaternion.LookRotation(Random.insideUnitSphere - pos), this.transform);
        var powerGem = gemGO.GetComponent<Power>();
        powerGem.Init(powerType);
        _powerGems.Add(powerGem);
    }

    private void Update()
    {
        HandlePowerGems();
        HandlePlayerInput();
        UpdatePowers();
    }

    private void HandlePowerGems()
    {
        List<Power> gemsToRemove = new List<Power>();
        foreach (Power powerGem in _powerGems)
        {
            float distance = Vector3.Distance(powerGem.Transform.position, Player.Instance.Transform.position);
            if (distance < _pickupRadius)
            {
                if (TryStorePower(powerGem.PowerType))
                {
                    Destroy(powerGem.gameObject);
                    gemsToRemove.Add(powerGem);
                }
            }
        }
        
        foreach (Power gem in gemsToRemove)
        {
            _powerGems.Remove(gem);
        }
    }

    private bool TryStorePower(PowerType powerType)
    {
        if (PowerSlot1.PowerType == PowerType.None)
        {
            PowerSlot1.PowerType = powerType;
            return true;
        }
        else if (PowerSlot2.PowerType == PowerType.None)
        {
            PowerSlot2.PowerType = powerType;
            return true;
        }
        else if (PowerSlot3.PowerType == PowerType.None)
        {
            PowerSlot3.PowerType = powerType;
            return true;
        }

        return false;
    }

    private void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && PowerSlot1.PowerType != PowerType.None && !PowerSlot1.IsActive)
        {
            ActivatePower(PowerSlot1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && PowerSlot2.PowerType != PowerType.None && !PowerSlot2.IsActive)
        {
            ActivatePower(PowerSlot2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && PowerSlot3.PowerType != PowerType.None && !PowerSlot3.IsActive)
        {
            ActivatePower(PowerSlot3);
        }
    }

    private void ActivatePower(PowerSlot powerSlot)
    {
        switch (powerSlot.PowerType)
        {
            case PowerType.Trap:

                powerSlot.PowerType = PowerType.None;
                ProjectileManager.Instance.CreateTrap(Player.Instance.Transform.position);

                break;
            case PowerType.Haste:

                ActivePower |= ActivePower.Haste;
                powerSlot.IsActive = true;
                powerSlot.Duration = HASTE_DURATION;

                break;
            case PowerType.Invisibility:

                ActivePower |= ActivePower.Invisibility;
                powerSlot.IsActive = true;
                powerSlot.Duration = INVISIBILITY_DURATION;

                break;
            case PowerType.FireBall:

                powerSlot.PowerType = PowerType.None;
                ProjectileManager.Instance.CreateFireBall(Player.Instance.Transform.position, Player.Instance.Transform.forward);

                break;
            case PowerType.RappidFire:
                ActivePower |= ActivePower.RappidFire;

                powerSlot.IsActive = true;
                powerSlot.Duration = RAPIDFIRE_DURATION;

                break;
            default:
                Debug.LogError("Unknown power type: " + powerSlot.PowerType);
                return;
        }
    }

    private void UpdatePowers()
    {
        UpdatePowerSlot(PowerSlot1);
        UpdatePowerSlot(PowerSlot2);
        UpdatePowerSlot(PowerSlot3);

        PowerType checkPower = PowerType.Haste;
        if (ActivePower.HasFlag(ActivePower.Haste))
        {
            if (PowerSlot1.PowerType == checkPower && PowerSlot1.IsActive || PowerSlot2.PowerType == checkPower && PowerSlot2.IsActive || PowerSlot3.PowerType == checkPower && PowerSlot3.IsActive) { }
            else
            {
                ActivePower &= ~ActivePower.Haste;
            }
        }

        checkPower = PowerType.Invisibility;
        if (ActivePower.HasFlag(ActivePower.Invisibility))
        {
            if (PowerSlot1.PowerType == checkPower && PowerSlot1.IsActive || PowerSlot2.PowerType == checkPower && PowerSlot2.IsActive || PowerSlot3.PowerType == checkPower && PowerSlot3.IsActive) { }
            else
            {
                ActivePower &= ~ActivePower.Invisibility;
            }
        }

        checkPower = PowerType.RappidFire;
        if (ActivePower.HasFlag(ActivePower.RappidFire))
        {
            if (PowerSlot1.PowerType == checkPower && PowerSlot1.IsActive || PowerSlot2.PowerType == checkPower && PowerSlot2.IsActive || PowerSlot3.PowerType == checkPower && PowerSlot3.IsActive) { }
            else
            {
                ActivePower &= ~ActivePower.RappidFire;
            }
        }
    }

    private void UpdatePowerSlot(PowerSlot powerSlot)
    {
        if (powerSlot.IsActive)
        {
            powerSlot.Duration -= Time.deltaTime;
            if (powerSlot.Duration <= 0f)
            {
                powerSlot.IsActive = false;
                powerSlot.PowerType = PowerType.None;
            }
        }
    }
}

public enum PowerType
{
    None = -1,
    Trap,
    Haste,
    Invisibility,
    FireBall,
    RappidFire,
}

[Flags]
public enum ActivePower
{
    Haste = 1,
    Invisibility = 2,
    RappidFire = 4,
}
