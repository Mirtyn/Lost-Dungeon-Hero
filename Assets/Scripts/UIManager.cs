using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ObjectBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private TMP_Text _powerText;

    private const string POWER_TEXT_PREFIX = "Power: ";

    [SerializeField] private Image _slot1Image;
    [SerializeField] private Image _slot2Image;
    [SerializeField] private Image _slot3Image;

    [SerializeField] private TMP_Text _slot1Timer;
    [SerializeField] private TMP_Text _slot2Timer;
    [SerializeField] private TMP_Text _slot3Timer;

    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _killsText;
    [SerializeField] private TMP_Text _gemsCollectedText;

    [SerializeField] private RectTransform _powerBar;
    [SerializeField] private RectTransform _powerMeter;
    [SerializeField] private Vector3 _powerMeterStartPos;
    [SerializeField] private float _powerBarMaxHeight = 530.45f;

    //[SerializeField] private RectTransform _powerDangerBar;
    //private float _powerDangerBarHeight = .1f;
    //[SerializeField] private RectTransform _powerOverPowerBar;
    //private float _powerOverHeightFactor = 0.5f;
    //private float _powerOverPowerHeightFactor = 0.9f;
    [SerializeField] private TMP_Text _maxPowerText;
    private Color normalColor => Color.white;
    private Color fullColor => Color.red;
    private Color overPowerColor => Color.magenta;
    private Color targetColor;

    private void Awake()
    {
        Instance = this;
        targetColor = fullColor;
    }

    private void Start()
    {
        _powerMeterStartPos = _powerMeter.localPosition;
        Player.OnPlayerOverPower += () =>
        {
            targetColor = overPowerColor;
            Invoke(nameof(ReturnColor), 1.5f);
        };

        //_powerDangerBar.anchoredPosition = new Vector2(_powerDangerBar.anchoredPosition.x, _powerBarMaxHeight * ((Player.Instance.GetMaxPower * _powerDangerBarHeight) / Player.Instance.GetMaxPower));
        //_powerOverPowerBar.anchoredPosition = new Vector2(_powerOverPowerBar.anchoredPosition.x, _powerBarMaxHeight * ((Player.Instance.GetMaxPower * _powerOverHeightFactor) / Player.Instance.GetMaxPower));
    }

    private void OnDestroy()
    {
        Instance = null;
        Player.OnPlayerOverPower -= () => { };
    }

    private void ReturnColor()
    {
        targetColor = fullColor;
    }

    private void Update()
    {
        if (Game.IsPlayerDead)
        {

            return;
        }
        UpdateStatsUI();
        UpdatePowerUI();
        UpdateGemPowerUI();
    }

    private void UpdateStatsUI()
    {
        _timeText.text = Mathf.FloorToInt(SpawnManager.Instance.GetTime).ToString();
        _killsText.text = Player.Instance.Kills.ToString();
        _gemsCollectedText.text = Player.Instance.StonePickedUp.ToString();
    }

    private void UpdatePowerUI()
    {
        _powerText.text = POWER_TEXT_PREFIX + Player.Instance.GetPower.ToString("F0");

        float completion = Mathf.Clamp01(Player.Instance.GetPower / Player.Instance.GetMaxPower);

        _powerBar.sizeDelta = Vector2.Lerp(_powerBar.sizeDelta, new Vector2(_powerBar.sizeDelta.x, _powerBarMaxHeight * completion), 2f * Time.deltaTime);
        _powerBar.GetComponent<Image>().color = Color.Lerp(normalColor, targetColor, completion);

        _maxPowerText.text = Player.Instance.GetMaxPower.ToString("F0");
        //_powerDangerBar.anchoredPosition = Vector2.Lerp(_powerDangerBar.anchoredPosition, new Vector2(_powerDangerBar.anchoredPosition.x, _powerBarMaxHeight * ((Player.Instance.GetMaxPower * _powerDangerBarHeight) / Player.Instance.GetMaxPower)), 5f * Time.deltaTime);
        //_powerOverPowerBar.anchoredPosition = Vector2.Lerp(_powerOverPowerBar.anchoredPosition, new Vector2(_powerOverPowerBar.anchoredPosition.x, _powerBarMaxHeight * ((Player.Instance.GetMaxPower * _powerOverHeightFactor) / Player.Instance.GetMaxPower)), 5f * Time.deltaTime);

        //// heart beat
        //if (completion < _powerDangerBarHeight)
        //{
        //    _powerMeter.localScale = Vector3.LerpUnclamped(_powerMeter.localScale, new Vector3(1.1f, 1.1f, 1.1f), Mathf.Sin(Time.realtimeSinceStartup));

        //}
        //else
        //{
        //    _powerMeter.localScale = Vector3.Lerp(_powerMeter.localScale, Vector3.one, 2f * Time.deltaTime);
        //}

        ////shake
        //if (completion >= _powerOverPowerHeightFactor)
        //{
        //    float shakeIntensity = 20f;
        //    _powerMeter.localPosition = Vector3.Lerp(_powerMeter.localPosition, _powerMeterStartPos + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * shakeIntensity, 3f * Time.deltaTime);
        //}
        //else
        //{
        //    _powerMeter.localPosition = Vector3.Lerp(_powerMeter.localPosition, _powerMeterStartPos, 3f * Time.deltaTime);
        //}
    }

    private void UpdateGemPowerUI()
    {
        UpdateUIPowerSlot(PowerManager.Instance.PowerSlot1, _slot1Image, _slot1Timer);
        UpdateUIPowerSlot(PowerManager.Instance.PowerSlot2, _slot2Image, _slot2Timer);
        UpdateUIPowerSlot(PowerManager.Instance.PowerSlot3, _slot3Image, _slot3Timer);
    }

    private void UpdateUIPowerSlot(PowerManager.PowerSlot powerSlot , Image slotImage, TMP_Text slotTimer)
    {
        if (powerSlot.PowerType == PowerType.None)
        {
            slotImage.color = Color.clear;
            slotTimer.text = string.Empty;
        }
        else
        {
            slotImage.sprite = GetPowerSprite(powerSlot.PowerType);
            var color = GetPowerColor(powerSlot.PowerType);

            if (powerSlot.Duration > 0f)
            {
                slotTimer.text = powerSlot.Duration.ToString("F0");
            }
            else
            {
                slotTimer.text = string.Empty;
            }

            if (powerSlot.IsActive)
            {
                color.a = 1f;
                slotImage.color = color;
            }
            else
            {
                color.a = 0.7f;
                slotImage.color = color;
            }
        }
    }

    private Color GetPowerColor(PowerType powerType)
    {
        switch (powerType)
        {
            case PowerType.Trap:
                return Game.Assets.BombColor;
            case PowerType.Haste:
                return Game.Assets.HasteColor;
            case PowerType.RapidFire:
                return Game.Assets.RappidFireColor;
            case PowerType.FireBall:
                return Game.Assets.FireBallColor;
            case PowerType.Invisibility:
                return Game.Assets.InvisibilityColor;
            default:
                Debug.LogError("Unknown power type: " + powerType);
                return Color.clear;
        }
    }

    private Sprite GetPowerSprite(PowerType powerType)
    {
        switch (powerType)
        {
            case PowerType.Trap:
                return Game.Assets.BombTexture;
            case PowerType.Haste:
                return Game.Assets.HasteTexture;
            case PowerType.RapidFire:
                return Game.Assets.RappidFireTexture;
            case PowerType.FireBall:
                return Game.Assets.FireBallTexture;
            case PowerType.Invisibility:
                return Game.Assets.InvisibilityTexture;
            default:
                Debug.LogError("Unknown power type: " + powerType);
                return null;
        }
    }
}
