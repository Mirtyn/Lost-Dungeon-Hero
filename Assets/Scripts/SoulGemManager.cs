using System;
using System.Collections.Generic;
using UnityEngine;
using static SoulGem;
using Random = UnityEngine.Random;

public class SoulGemManager : ObjectBehaviour
{
    public static SoulGemManager Instance { get; private set; }
    private List<SoulGem> _soulGems = new List<SoulGem>();
    private float _gemSpawnRadius = 0.1f;
    private float _attractionRadius = 2f;
    private float _absorbRadius = 0.2f;
    private float _soulGemAttractionSpeed = 7f;
    private float _leftTimeLeftBeforeLightFades = 4f;
    public static event Action OnSoulGemCollected;

    //private float _soulGemLightDefaultIntensity;
    //private Color _soulGemMaterialDefaultColor;
    //private Color _soulGemEmmisionDefaultColor;
    //[SerializeField] private Color _soulGemDissapearColor = Color.magenta;
    //[SerializeField] private Color _soulGemDissapearColorAlt = Color.blue;
    private float switchDissapearGemColorTimeMin = 0.05f;
    private float switchDissapearGemColorTimeMax = 0.55f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnemyManager.OnEnemyDeath += OnEnemyDeath;
        //_soulGemLightDefaultIntensity = Game.Assets.SoulGemPrefab.GetComponentInChildren<Light>().intensity;
        //_soulGemMaterialDefaultColor = Game.Assets.SoulGemPrefab.GetComponentInChildren<Renderer>().sharedMaterial.color;
        //_soulGemEmmisionDefaultColor = Game.Assets.SoulGemPrefab.GetComponentInChildren<Renderer>().sharedMaterial.GetColor("_EmissionColor");
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
            soulGem.RemainingLifeTime -= Time.deltaTime;

            if (soulGem.RemainingLifeTime <= _leftTimeLeftBeforeLightFades)
            {
                var t = 1f - (soulGem.RemainingLifeTime / _leftTimeLeftBeforeLightFades);
                var switchDissapearGemColorTime = Mathf.Lerp(switchDissapearGemColorTimeMax, switchDissapearGemColorTimeMin, t);

                if (soulGem.PreviousVisualStateChangeTime > soulGem.RemainingLifeTime)
                {
                    soulGem.PreviousVisualStateChangeTime = soulGem.RemainingLifeTime - switchDissapearGemColorTime;

                    soulGem.Visible = !soulGem.Visible;

                    if (!soulGem.Visible)
                    {
                        soulGem.Visual.SetActive(false);
                    }
                    else
                    {
                        soulGem.Visual.SetActive(true);
                    }
                }

                if (soulGem.Visible)
                {
                    //soulGem.Renderer.material.color = Color.Lerp(_soulGemMaterialDefaultColor, targetColor, t);
                    //soulGem.Renderer.material.SetColor("_EmissionColor", Color.Lerp(_soulGemEmmisionDefaultColor, targetColor, t));
                    //soulGem.Light.color = Color.Lerp(_soulGemMaterialDefaultColor, targetColor, t);
                    //soulGem.Light.intensity = Mathf.Lerp(_soulGemLightDefaultIntensity, 0f, t));
                }
            }

            if (soulGem.RemainingLifeTime <= 0f)
            {
                Destroy(soulGem.gameObject);
                gemsToRemove.Add(soulGem);
                continue;
            }

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
