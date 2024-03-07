using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool IsTutorial { get; set; } = false;
    public bool tutorialHook;


    public override void Awake()
    {
        base.Awake();
        IsTutorial = tutorialHook;
    }

    void Update()
    {
        
    }

    public void BackToMenu()
    {
        IsTutorial = false;
        SceneManager.LoadScene("Menu");
    }
}
