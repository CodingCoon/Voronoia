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
        FromHex("EDADA3"),  // peach
        FromHex("CC446B"),  // red
        FromHex("45556C"),  // gray blue
        FromHex("3A2368"),  // purple
        FromHex("3C8F7B"),  // green
        FromHex("640E40")}; // pink



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