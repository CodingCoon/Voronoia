using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchStatusPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button restart;
    [SerializeField] private Button menu;
    private bool leaving;

    private void Awake()
    {
        root.SetActive(false);
        restart.onClick.AddListener(Restart);
        menu.onClick.AddListener(BackToMenu);
    }

    public void ShowResult(Game.GameState state)
    {
        title.text = state == Game.GameState.PLAYER_WON ? "Sieg" :
            state == Game.GameState.PLAYER_LOOSE ? "Niederlage" : "Unentschieden";
        description.text = state == Game.GameState.PLAYER_WON ? "Deine Fraktion hat überlebt." :
            state == Game.GameState.PLAYER_LOOSE ? "Deine Fraktion ist ausgeschieden." :
            "Alle Fraktionen sind gleichzeitig ausgeschieden.";
        root.SetActive(true);
    }

    public void ShowError()
    {
        title.text = "Runde angehalten";
        description.text = "Die Runde konnte nicht sicher abgeschlossen werden. Bitte starte eine neue Partie oder kehre ins Menü zurück.";
        root.SetActive(true);
    }

    private void Restart()
    {
        if (leaving) return;
        leaving = true;
        restart.interactable = menu.interactable = false;
        if (GameManager.Instance.IsTutorial) GameManager.Instance.StartTutorial();
        else GameManager.Instance.StartGame();
    }

    private void BackToMenu()
    {
        if (leaving) return;
        leaving = true;
        restart.interactable = menu.interactable = false;
        GameManager.Instance.BackToMenu();
    }

    private void OnDestroy()
    {
        restart.onClick.RemoveListener(Restart);
        menu.onClick.RemoveListener(BackToMenu);
    }
}