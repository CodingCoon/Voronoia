using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool IsTutorial { get; set; } = false;
    public bool tutorialHook;


    void Start()
    {
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
