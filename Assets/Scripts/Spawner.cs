using UnityEngine;

public class Spawner : ObjectBehaviour
{
    [field: SerializeField] public Transform SpawnPoint { get; private set; }
    [SerializeField] private Transform _door;
    [SerializeField] private Light _light;

    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _spawnColor;
    private Color _targetColor;
    private float _colorShiftSpeed = 2f;

    private float _lightNormalIntensity = 1f;
    private float _lightSpawnIntensity = 2.8f;
    private float _targetLightIntensity;
    private float _lightIntensityShiftSpeed = 2.5f;

    private float _targetDoorLocalYPosition;
    private float _doorMoveSpeed = 3f;

    private float returnToNormalDelay = 2.2f;

    private void Start()
    {
        SpawnManager.Instance.AddSpawnPoint(this);
        _targetColor = _normalColor;
        _targetDoorLocalYPosition = 0f;
        _targetLightIntensity = _lightNormalIntensity;
    }

    private void Update()
    {
        _door.localPosition = Vector3.Lerp(_door.localPosition, new Vector3(_door.localPosition.x, _targetDoorLocalYPosition, _door.localPosition.z), Time.deltaTime * _doorMoveSpeed);
        _light.color = Color.Lerp(_light.color, _targetColor, Time.deltaTime * _colorShiftSpeed);
        _light.intensity = Mathf.Lerp(_light.intensity, _targetLightIntensity, Time.deltaTime * _lightIntensityShiftSpeed);
    }

    public void PlaySpawnAnimation()
    {
        _targetColor = _spawnColor;
        _targetDoorLocalYPosition = -1f;
        _targetLightIntensity = _lightSpawnIntensity;
        Invoke(nameof(ReturnToNormalAnimation), returnToNormalDelay);
    }

    private void ReturnToNormalAnimation()
    {
        _targetColor = _normalColor;
        _targetDoorLocalYPosition = 0f;
        _targetLightIntensity = _lightNormalIntensity;
    }
}
