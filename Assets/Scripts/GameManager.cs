using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Manipulation Settings")]
    [SerializeField] private float TimeMultiplier = 1f;
    [SerializeField] private int MaxFrames = -1;
    [Space(15)]
    [SerializeField] private bool _addTime = false;
    [SerializeField] private float _timeAmount = 100f;
    [Space(15)]
    [SerializeField] private bool _addPower = false;
    [SerializeField] private float _powerAmount = 100f;
    [field: Space(15)]
    [field: SerializeField] public Assets Assets { get; private set; }

    public Camera MainCamera { get; private set; }
    public PlayerInput PlayerInput { get; private set; }
    private bool _isPlayerDead = false;
    public bool IsPlayerDead
    {
        get
        {
            return _isPlayerDead;
        }
        set
        {
            _isPlayerDead = value;
            if (_isPlayerDead)
            {
                DeathScreen.Instance.Activate();
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        MainCamera = GameObject.FindWithTag(Tags.MAIN_CAMERA).GetComponent<Camera>();
        PlayerInput = new PlayerInput();
    }

    private void Update()
    {
        Time.timeScale = TimeMultiplier;
        Application.targetFrameRate = MaxFrames;

        if (_addTime)
        {
            _addTime = false;
            SpawnManager.Instance.AddTime(_timeAmount);
        }

        if (_addPower)
        {
            _addPower = false;
            Player.Instance.AddPower(_powerAmount);
        }
    }
}

public static class Tags
{
    public const string MAIN_CAMERA = "MainCamera";
    public const string PLAYER = "Player";
    public const string ENEMY = "Enemy";
    public const string PROJECTILE = "Projectile";
}

public static class Layers
{
    public const int ALL_LAYERS = -1;
    public const int DEFAULT = 1 << 0;
    public const int PLAYER = 1 << 3;
    public const int ENVIRONMENT = 1 << 6;
    public const int ENEMY = 1 << 7;
    public const int ENEMY_WALKABLE_ENVIRONMENT = 1 << 8;
    public const int FLOOR = 1 << 9;
}

[System.Serializable]
public class Assets
{
    [field: Header("Prefabs")]
    [field: SerializeField] public GameObject ArrowPrefab { get; private set; }
    [field: SerializeField] public GameObject TrapPrefab { get; private set; }
    [field: SerializeField] public GameObject FireBallPrefab { get; private set; }
    [field: SerializeField] public GameObject FodderEnemyPrefab { get; private set; }
    [field: SerializeField] public GameObject HeavyEnemyPrefab { get; private set; }
    [field: SerializeField] public GameObject QuickEnemyPrefab { get; private set; }
    [field: SerializeField] public GameObject SoulGemPrefab { get; private set; }
    [field: SerializeField] public GameObject PowerGemPrefab { get; private set; }
    [field: Header("Sprites")]
    [field: SerializeField] public Sprite BombTexture { get; private set; }
    [field: SerializeField] public Sprite FireBallTexture { get; private set; }
    [field: SerializeField] public Sprite QuadDamageTexture { get; private set; }
    [field: SerializeField] public Sprite HasteTexture { get; private set; }
    [field: SerializeField] public Sprite InvisibilityTexture { get; private set; }
    [field: SerializeField] public Sprite RappidFireTexture { get; private set; }
    public Color BombColor => new Color(1f, 0.5f, 0f); // Orange;
    public Color FireBallColor => Color.magenta;
    public Color QuadDamageColor => new Color(0, 0.5f, 1f); // Light blue
    public Color HasteColor => new Color(0, 0.5f, 1f); // Light blue
    public Color InvisibilityColor => new Color(0.5f, 0.5f, 0.5f, 0.5f); // Semi-transparent gray
    public Color RappidFireColor => Color.green;
    //[field: Header("Animations")]
    //[field: SerializeField] public AnimationClip EnemyDeathAnimation { get; private set; }
}
