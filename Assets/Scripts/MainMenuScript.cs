using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels;
    int loopIndex = 0;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (loopIndex >= panels.Count)
            {
                SceneManager.LoadScene(1);
                return;
            }
            Tutorial();
            loopIndex++;
        }
    }

    private void Tutorial()
    {
        panels[loopIndex].SetActive(true);
    }
}
