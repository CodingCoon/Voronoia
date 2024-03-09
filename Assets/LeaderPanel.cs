using TMPro;
using UnityEngine;

public class LeaderPanel : MonoBehaviour
{
    [SerializeField] private GameObject rootChild;

    [SerializeField] private TextMeshProUGUI idLabel;
    [SerializeField] private TextMeshProUGUI activeRoundsLabel;
    [SerializeField] private TextMeshProUGUI areaLabel;
    [SerializeField] private TextMeshProUGUI incomeLabel;
    [SerializeField] private TextMeshProUGUI powerLabel;

    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private TextMeshProUGUI priceLabel;
    [SerializeField] private TextMeshProUGUI infoLabel;

    void Update()
    {
        Leader curLeader = LeaderSelectionManager.INSTANCE.Leader;
        rootChild.SetActive(curLeader != null);

        if (curLeader != null)
        {
            idLabel.text = "Leader #" + curLeader.Number;
            activeRoundsLabel.text = curLeader.RoundsExist + " rounds active";
            areaLabel.text = (int)curLeader.GetArea() + "m²";
            incomeLabel.text = curLeader.Income + "x";
            powerLabel.text = curLeader.Power + "x";

            IAction action = curLeader.Action;
            actionLabel.text = action.Name;
            priceLabel.text = "" + (int)action.GetPrice();
            infoLabel.text = action.GetDetailedInfos();

            // todo ggf. action texte gelb machen, wenn noch in der Mache
        }
    }
}
