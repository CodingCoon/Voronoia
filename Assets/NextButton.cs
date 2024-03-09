using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextButton : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    void Update()
    {
        UpdateInteractability();
        UpdateText();
    }

    // Called from the UI
    public void NextPhase()
    {
        Game.INSTANCE.NextPhase();
    }

    private void UpdateText ()
    {
        if (Game.INSTANCE.PhaseType == PhaseType.ACTION)
        {
            buttonText.text = "Apply actions";
        }
        else if (Game.INSTANCE.PhaseType == PhaseType.DEATH)
        {
            buttonText.text = "Next round";
        }
        else
        {
            buttonText.text = string.Empty;
        }
    }

    private void UpdateInteractability()
    {
        nextButton.interactable = Game.INSTANCE.PhaseType == PhaseType.EVALUATION || Game.INSTANCE.PhaseType == PhaseType.ACTION;

        if (GameManager.Instance.IsTutorial)
        {
            nextButton.interactable = TutorialManager.Instance.NextRoundButtonEnabled && nextButton.interactable;
        }
    }
}
