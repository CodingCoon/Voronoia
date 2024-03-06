using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        // todo blende EXIT aus beim WebGl
    }

    public void StartTutorial()
    {
        GameManager.Instance.IsTutorial = true;
        SceneManager.LoadScene("GameScene");
    }

    public void StartGame()
    {
        GameManager.Instance.IsTutorial = false;
        SceneManager.LoadScene("GameScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
