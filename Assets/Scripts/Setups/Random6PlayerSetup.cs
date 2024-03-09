using System.Collections.Generic;
using UnityEngine;

public class Random6PlayerSetup : AbstractSetup
{
    private static readonly List<string> NAMES = new List<string>{
        "Porter", 
        "Wit", 
        "Stout", 
        "IPA", 
        "Lager",
        "Saison"};

    private static readonly List<Color> COLORS = new List<Color>() {
        FromHex("64766A"),  // Grün
        FromHex("F4F2F3"),  // Eierschale
        FromHex("C0A9BD"),  // Rosa
        FromHex("94A7AE"),  // grau 
        FromHex("FFBB98"),  // Orange
        FromHex("2A403D")}; // Blau
  
    protected override int GetPlayerCount()
    {
        return 6;
    }

    protected override ITactic GetTactic(int i, Voronation religion)
    {
        if (i == 0) return null;
        if (i % 2 == 0) return new RiseInfluenceTactic(religion);
        return new RisePowerTactic(religion);
    }

    protected override string GetName(int i)
    {
        return NAMES[i];    
    }

    protected override Color GetColor(int i)
    {
        return COLORS[i];
    }

    protected override Vector2 GetPoint(int i)
    {
        float degree = 60 * i;
        float angle = Mathf.PI * degree / 180;
        float distance = 4f;

        Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        return point * distance;
    }
}