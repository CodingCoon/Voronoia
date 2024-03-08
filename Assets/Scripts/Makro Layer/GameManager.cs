using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool IsTutorial { get; set; } = false;
    public bool FadeOver { get; set; } = false;

    public bool tutorialHook;


    public override void Awake()
    {
        base.Awake();
        IsTutorial = tutorialHook;
    }

    public void BackToMenu()
    {
        IsTutorial = false;
        FadeOver = true;
        ScreenBlend.INSTANCE.FadeOut(() => SceneManager.LoadScene("Menu"));     // todo vermutlich ist das architektonisch nicht so ganz richtig, jemand mit referenz auf dem Blend sollte dem GM sagen was passiert und dann den blend aktivieren, ansonten weiß der GM viel UI 
    }

    public void StartGame()
    {
        IsTutorial = false;
        FadeOver = true;
        ScreenBlend.INSTANCE.FadeOut(() => SceneManager.LoadScene("GameScene"));
    }

    public void StartTutorial()
    {
        IsTutorial = true;
        FadeOver = true;
        ScreenBlend.INSTANCE.FadeOut(() => SceneManager.LoadScene("GameScene"));
    }

    public void Exit()
    {
        ScreenBlend.INSTANCE.FadeOut(() => Application.Quit());
    }
}
