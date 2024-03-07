using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Game game;
    [SerializeField] private Button nextButton;


    void Update()
    {
        if (GameManager.Instance.IsTutorial)
        {
            nextButton.interactable = TutorialManager.Instance.NextRoundButtonEnabled;
        }
    }

    public void NextPhase()
    {
        game.NextPhase();
    }
}