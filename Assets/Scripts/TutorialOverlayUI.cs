using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOverlayUI : MonoBehaviour
{
    [SerializeField] private SkipButton skipButton;
    [SerializeField] private TextMeshPro tutorialText;

    [SerializeField] private TutorialManager tutorialManager;

    void Start()
    {
        if (!GameManager.Instance.IsTutorial) gameObject.SetActive(false);     
    }

    // Update is called once per frame
    void Update()
    {
        tutorialText.text = tutorialManager.GetHint();
        tutorialText.gameObject.SetActive(tutorialText.text.Length > 0);
        skipButton.gameObject.SetActive(tutorialManager.HasNext() || tutorialManager.TutorialDone);

        if (! GameManager.Instance.IsTutorial)
        {
            GameObject.Destroy(gameObject, 0f);
        }
    }

    // Called from UI
    public void NextTutorialHint()
    {
        if (tutorialManager.TutorialDone)
        {
            GameManager.Instance.BackToMenu();
        }
        else
        {
            tutorialManager.Next();
        }
    }

}
