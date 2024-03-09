using System.Collections.Generic;
using UnityEngine;

public class Random6PlayerSetup : AbstractSetup
{
    private static readonly List<string> PLAYER_NAMES = new List<string>{
        "Porter", 
        "Wit", 
        "Stout", 
        "IPA", 
        "Lager",
        "Saison"};

    private static readonly List<Color> PLAYER_COLORS = new List<Color>() {
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
        return PLAYER_NAMES[i];    
    }

    protected override Color GetColor(int i)
    {
        return PLAYER_COLORS[i];
    }

    protected override Vector2 GetPoint(int i)
    {
        return map.RandomPoint();
    }
}