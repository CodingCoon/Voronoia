using System.Globalization;
using UnityEngine;

public abstract class AbstractSetup : MonoBehaviour
{
    [SerializeField] protected Map map;
    [SerializeField] private Game game;
    [SerializeField] private Voronation religionPrefab;
    [SerializeField] private GameObject religionsFolder;

    public void GeneratePlayers()
    {
        for (int i = 0; i < GetPlayerCount(); i++)
        {
            Vector2 point = GetPoint(i);
            Voronation religion = GameObject.Instantiate(religionPrefab, religionsFolder.transform);
            ITactic tactic = GetTactic(i, religion);
            float money = GetMoney(i);
            string name = GetName(i);

            religion.Setup(name, GetColor(i), tactic, money);
            religion.name = name;
            religion.AddPreacher(point);
            game.AddReligion(religion);
        }
    }

    protected abstract int GetPlayerCount();
    protected abstract ITactic GetTactic(int i, Voronation religion);

    protected abstract string GetName(int i);

    protected abstract Color GetColor(int i);
    protected abstract Vector2 GetPoint(int i);

    protected virtual float GetMoney(int i)
    { 
        return 100; 
    }   

    public static Color FromHex(string hex)
    {
        if (hex.Length != 6) throw new System.FormatException("Needs a string with a length of at least 6");

        var r = hex.Substring(0, 2);
        var g = hex.Substring(2, 2);
        var b = hex.Substring(4, 2);

        return new Color((int.Parse(r, NumberStyles.HexNumber) / 255f),
                        (int.Parse(g, NumberStyles.HexNumber) / 255f),
                        (int.Parse(b, NumberStyles.HexNumber) / 255f),
                        (int.Parse("FF", NumberStyles.HexNumber) / 255f));
    }


}
