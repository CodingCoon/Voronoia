using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class RandomReligionSetup : MonoBehaviour
{
    [SerializeField] private Map map;
    [SerializeField] private Game game;
    [SerializeField] private Religion religionPrefab;
    [SerializeField] private GameObject religionsFolder;

    [Range(1, 20)][SerializeField] private int amount;

    private List<string> religionsNames = new List<string>{
        "Porter", 
        "Wit", 
        "Stout", 
        "IPA", 
        "Lager",
        "Saison"};
    private List<Color> religionsColors = new List<Color>() {
        FromHex("64766A"),  // Grün
        FromHex("F4F2F3"),  // Eierschale
        FromHex("C0A9BD"),  // Rosa
        FromHex("94A7AE"),  // grau 
        FromHex("FFBB98"),  // Orange
        FromHex("2A403D")}; // Blau

    private List<Vector2> positions = new List<Vector2>()
    {
        new Vector2 (-3,  2),
        new Vector2 ( 3,  2),
        new Vector2 ( 3, -4)
    };

    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 point = map.RandomPoint();
            //            Vector2 point = positions[i];   // map.RandomPoint();
            Religion religion = GameObject.Instantiate(religionPrefab, religionsFolder.transform);
            ITactic tactic = ChooseTactic(i, religion);

            religion.Setup(religionsNames[i% religionsNames.Count], religionsColors[i % religionsColors.Count], tactic, 100);
            religion.name = religionsNames[i % religionsNames.Count];
            religion.AddPreacher(point);
            game.AddReligion(religion);
        }
    }

    private ITactic ChooseTactic(int i, IReligion religion)
    {
        if (i == 0) return null;
        if (i % 2 == 0) return new RiseInfluenceTactic(religion);
        return new RisePowerTactic(religion);
    }

    public static Color FromHex(string hex)
    {
        if (hex.Length < 6)
        {
            throw new System.FormatException("Needs a string with a length of at least 6");
        }

        var r = hex.Substring(0, 2);
        var g = hex.Substring(2, 2);
        var b = hex.Substring(4, 2);
        string alpha;
        if (hex.Length >= 8)
            alpha = hex.Substring(6, 2);
        else
            alpha = "FF";

        return new Color((int.Parse(r, NumberStyles.HexNumber) / 255f),
                        (int.Parse(g, NumberStyles.HexNumber) / 255f),
                        (int.Parse(b, NumberStyles.HexNumber) / 255f),
                        (int.Parse(alpha, NumberStyles.HexNumber) / 255f));
    }
}