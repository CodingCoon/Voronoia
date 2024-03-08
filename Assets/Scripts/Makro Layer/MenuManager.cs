using UnityEngine;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        // todo blende EXIT aus beim WebGl
    }

    public void StartTutorial()
    {
        GameManager.Instance.StartTutorial();
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void Exit()
    {
        GameManager.Instance.Exit();
    }
}
