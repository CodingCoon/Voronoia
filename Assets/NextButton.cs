using TMPro;
using UnityEngine;

public class NextButton : MonoBehaviour
{
    [SerializeField] private MainMenuButton nextButton;
    [SerializeField] private TextMeshPro buttonText;
    [SerializeField] private Animator animator;

    void Update()
    {
        UpdateInteractability();
        UpdateText();
    }

    // Called from the UI
    public void NextPhase()
    {
        animator.Play("Click");
        Game.INSTANCE.NextPhase();
    }

    private void UpdateText ()
    {
        if (Game.INSTANCE.PhaseType == PhaseType.ACTION)
        {
            buttonText.text = "APPLY ACTIONS";
        }
        else if (Game.INSTANCE.PhaseType == PhaseType.DEATH)
        {
            buttonText.text = "NEXT ROUND";
        }
        else
        {
            buttonText.text = string.Empty;
        }
    }

    private void UpdateInteractability()
    {
        nextButton.SetInteractable(Game.INSTANCE.PhaseType == PhaseType.EVALUATION || Game.INSTANCE.PhaseType == PhaseType.ACTION);

        if (GameManager.Instance.IsTutorial)
        {
            nextButton.SetInteractable(TutorialManager.Instance.NextRoundButtonEnabled && nextButton.Interactable);
        }
    }
}
