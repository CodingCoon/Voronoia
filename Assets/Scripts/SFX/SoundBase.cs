using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Sound/Database", order = 1)]
public class SoundBase : ScriptableObject
{
    [SerializedDictionary("ClipType", "Clips")]
    [SerializeField] private SerializedDictionary<string, List<AudioClip>> sounds;

    internal AudioClip GetAudioClip(string clipType)
    {
        if (sounds.ContainsKey(clipType))
        {
            List<AudioClip> clips = sounds[clipType];
            if (clips.Count > 0)
            {
                return clips.GetRandom();
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    internal AudioClip[] GetClips(string clipType)
    {
        if (sounds.ContainsKey(clipType))
        {
            return sounds[clipType].ToArray();
        }
        else
        {
            return new AudioClip[0];
        }
    }


    [Serializable]
    public class SoundEntry
    {
        [SerializeField] private List<AudioClip> availableClips;
    }
}
