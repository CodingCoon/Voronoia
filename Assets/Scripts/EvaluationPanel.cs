using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using VoronationCore;

public class EvaluationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI incomeText;
    public void Clear() { incomeText.text = string.Empty; gameObject.SetActive(false); }

    public void ShowRound(RoundResult result, FactionId? playerId)
    {
        gameObject.SetActive(true);
        var text = new StringBuilder();
        if (result != null && playerId.HasValue)
        {
            FactionState before = result.Before.FindFaction(playerId.Value);
            foreach (KnightState knight in before.Knights.OrderBy(item => item.Id))
            {
                text.AppendLine("Ritter #" + knight.Id.Number);
                float balance = 0;
                foreach (RoundBooking booking in result.Bookings.Where(item => item.KnightId == knight.Id &&
                    item.Kind != BookingKind.DebtRelief))
                {
                    text.AppendLine(booking.Amount.ToString("+0.##;-0.##;0") + "\t" + booking.Label);
                    balance += booking.Amount;
                }
                text.AppendLine("Saldo: " + balance.ToString("+0.##;-0.##;0"));
            }
            FactionRoundSummary summary = result.FactionSummaries.First(item => item.FactionId == playerId.Value);
            text.AppendLine("Kontostand: " + summary.MoneyAfterAccounting.ToString("0.##"));
        }
        incomeText.text = text.ToString();
    }

    public void ShowDebtRelief(Voronation player, int removedNumber)
    {
        incomeText.text += "\nRitter #" + removedNumber + " ausgeschieden\n+" +
            player.DebtRelief.ToString("0.##") + "\tSchuldenerlass\nKontostand: " + player.Money.ToString("0.##");
    }
}
