using TMPro;
using UnityEngine;

public class MoneyLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    private Voronation humanVoronation;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.INSTANCE.PhaseType == PhaseType.START) return;
        if (humanVoronation == null)
        {
            humanVoronation = Game.INSTANCE.GetHumanPlayer();
        }

        label.text = "" + (int)humanVoronation.Money;
    }
}
