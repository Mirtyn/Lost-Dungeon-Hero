using UnityEngine;

public class PowerGem : ObjectBehaviour
{
    public Transform Transform { get; private set; }
    public PowerType PowerType { get; private set; }
    [SerializeField] private GameObject _visual;
    [SerializeField] private Light _lightSource;

    public void Init(PowerType powerType)
    {
        Transform = this.transform;
        PowerType = powerType;

        switch (powerType)
        {
            case PowerType.Trap:
                SetColors(Game.Assets.BombColor);
                break;
            case PowerType.Haste:
                SetColors(Game.Assets.HasteColor);
                break;
            case PowerType.RapidFire:
                SetColors(Game.Assets.RappidFireColor);
                break;
            case PowerType.FireBall:
                SetColors(Game.Assets.FireBallColor);
                break;
            case PowerType.Invisibility:
                SetColors(Game.Assets.InvisibilityColor);
                break;
            default:
                Debug.LogError("Unknown power type: " + powerType);
                break;
        }
    }

    private void SetColors(Color color)
    {
        _visual.GetComponent<Renderer>().material.color = color;
        _visual.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
        _lightSource.color = color;
    }
}
