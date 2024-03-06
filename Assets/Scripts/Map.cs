using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map INSTANCE;

    [SerializeField] private float halfMapSize = 10f;

    public float HalfMapSize => halfMapSize;

    private void Awake()
    {
        INSTANCE = this;
    }

    public Vector2 RandomPoint()
    {
        float max =  halfMapSize;
        float min = -halfMapSize;

        float randomX = Random.Range(min, max);
        float randomZ = Random.Range(min, max);
        
        return new Vector2(randomX, randomZ);
    }

}
