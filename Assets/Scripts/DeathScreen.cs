using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : ObjectBehaviour
{
    public static DeathScreen Instance { get; private set; }

    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _killsText;
    [SerializeField] private TMP_Text _gemsCollectedText;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Activate()
    {
        Invoke(nameof(SetVisible), 1.2f);
    }

    private void SetVisible()
    {
        this.gameObject.SetActive(true);

        _timeText.text = Mathf.FloorToInt(SpawnManager.Instance.GetTime).ToString();
        _killsText.text = Player.Instance.Kills.ToString();
        _gemsCollectedText.text = Player.Instance.StonePickedUp.ToString();
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
