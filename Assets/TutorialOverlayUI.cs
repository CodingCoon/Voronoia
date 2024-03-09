using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOverlayUI : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI tutorialText;

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
        skipButton.gameObject.SetActive(tutorialManager.HasNext());
        menuButton.gameObject.SetActive(tutorialManager.TutorialDone);

        if (! GameManager.Instance.IsTutorial)
        {
            GameObject.Destroy(gameObject, 0f);
        }
    }

    public void ToMenu()
    {
        GameManager.Instance.BackToMenu();
    }

    public void NextTutorialHint()
    {
        tutorialManager.Next();
    }

}
