using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOverlayUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
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
        nextButton.interactable = tutorialManager.HasNext();
    }

    public void NextTutorialHint()
    {
        tutorialManager.Next();
    }

}
