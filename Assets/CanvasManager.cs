using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Game game;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject evaluationPanel;

    void Update()
    {
        nextButton.interactable = game.PhaseType == PhaseType.EVALUATION || game.PhaseType == PhaseType.ACTION;

        if (GameManager.Instance.IsTutorial)
        {
            nextButton.interactable = TutorialManager.Instance.NextRoundButtonEnabled && nextButton.interactable;
        }

        evaluationPanel.gameObject.SetActive(game.PhaseType == PhaseType.EVALUATION);
    }

    // Called from the UI
    public void NextPhase()
    {
        game.NextPhase();
    }
}