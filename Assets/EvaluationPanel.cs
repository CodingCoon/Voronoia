using System.Text;
using TMPro;
using UnityEngine;

public class EvaluationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI incomeText;

    private bool loaded = false;

    void Update()
    {
        if (Game.INSTANCE.PhaseType == PhaseType.EVALUATION || Game.INSTANCE.PhaseType == PhaseType.DEATH)
        {
            if (! loaded)
            {
                LoadText();
                loaded = true;
            }
        }   
        else
        {
            loaded = false;
            incomeText.text = string.Empty;
        }
    }

    private void LoadText()
    {
        StringBuilder sb = new StringBuilder();

        Voronation playerVoronation = Game.INSTANCE.GetHumanPlayer();
        foreach (Leader leader in playerVoronation.GetLeaders())
        {
            sb.AppendLine("Leader #" + leader.Number);

            foreach (IncomePosition ip in leader.GetPositions())
            {
                if (ip.Value < 0)
                {
                    sb.AppendLine("- " + Mathf.Abs((int) ip.Value) + "\t" + ip.Name);
                }
                else
                {
                    sb.AppendLine("+ " + (int) ip.Value + "\t" + ip.Name);
                }

            }
        }
        incomeText.text = sb.ToString();
    }
}
