using System.Collections.Generic;
using UnityEngine;

public class TutorialSetup : AbstractSetup
{

    private static readonly List<string> names = new List<string>{
        "Porter", 
        "Wit", 
        "Stout", 
        "IPA"};

    private static readonly List<Color> colors = new List<Color>() {
        FromHex("64766A"),  // Grün
        FromHex("F4F2F3"),  // Eierschale
        FromHex("C0A9BD"),  // Rosa
        FromHex("2A403D")}; // Blau

    private static readonly List<Vector2> positions = new List<Vector2>()
    {
        new Vector2 (-3, 2),
        new Vector2 ( 3, 2),
        new Vector2 ( 0, 0),
        new Vector2 ( 3, 4),
    };

    protected override int GetPlayerCount()
    {
        return 4;
    }

    protected override ITactic GetTactic(int i, Voronation religion)
    {
        if (i == 0) return null; // player
        return new DoNothingTactic(religion);
    }

    protected override string GetName(int i)
    {
        return names[i];
    }

    protected override Color GetColor(int i)
    {
        return colors[i];
    }

    protected override Vector2 GetPoint(int i)
    {
        return positions[i];
    }

    protected override float GetMoney(int i)
    {
        return i == 1 ? -50 : base.GetMoney(i); 
    }
}