using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Sound/Database", order = 1)]
public class SoundBase : ScriptableObject
{
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip moveLeader;
    [SerializeField] private AudioClip splitLeader;
    [SerializeField] private AudioClip improveLeader;
    [SerializeField] private AudioClip nationDied;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip gameWon;
    [SerializeField] private AudioClip fadeOver;
    [SerializeField] private AudioClip taktak;

    internal AudioClip GetAudioClip(string clipTitle)
    {
        switch (clipTitle)
        {
            case "Button Click":
                return buttonClick;
            case "Fade Over":
                return fadeOver;
            case "Tak Tak":
                return taktak;

            case "Move Leader":
                return moveLeader;
            case "Split Leader":
                return splitLeader;
            case "Improve Leader":
                return improveLeader;

            case "Nation Died":
                return nationDied;
            case "Game Over":
                return gameOver;
            case "Game Won":
                return gameWon;

            default: return null;
        }
    }
}