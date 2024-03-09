using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class LeaderSelectionManager : MonoBehaviour
{
    public static LeaderSelectionManager INSTANCE;

    public Leader Leader {  get; private set; }

    private void Awake()
    {
        INSTANCE = this; 
    }

    public void UpdateLeader(Leader leader)
    {
        this.Leader = leader;
    }
}
