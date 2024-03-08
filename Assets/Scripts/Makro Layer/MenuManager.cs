using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private ScreenBlend screenFader; 

    void Start()
    {
        // todo blende EXIT aus beim WebGl
    }

    public void StartTutorial()
    {
        GameManager.Instance.IsTutorial = true;
        GameManager.Instance.FadeOver = true;
        screenFader.FadeOut(() => SceneManager.LoadScene("GameScene"));
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
