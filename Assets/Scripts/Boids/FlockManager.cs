using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager instance;
    public GameObject boidPrefab;
    public GameObject secondBoidPrefab;
    public GameObject goalPrefab;
    private GameObject _goal;
    public int numBoids = 20;
    public GameObject[] boids;
    public Vector3 limits = new(5, 5, 5);
    public Vector3 goalPos = Vector3.zero;

    [Header("Boid Settings")]
    [Range(0.0f, 5.0f)] public float minSpeed = 0.5f;
    [Range(0.0f, 20.0f)] public float maxSpeed = 2.0f;
    [Range(1.0f, 10.0f)] public float neighbourDistance = 3.0f;
    [Range(1.0f, 5.0f)] public float rotationSpeed = 1.0f;
    
    private void Awake()
    {
        boids = new GameObject[numBoids];
        for (int i = 0; i < numBoids; i++)
        {
            Vector3 pos = new Vector3(
                UnityEngine.Random.Range(-limits.x, limits.x),
                UnityEngine.Random.Range(-limits.y, limits.y),
                UnityEngine.Random.Range(-limits.z, limits.z)
                );
            
            GameObject prefab = UnityEngine.Random.Range(0, 100) < 50 ? boidPrefab : secondBoidPrefab;
            
            boids[i] = Instantiate(prefab, pos + transform.position, Quaternion.identity);
            boids[i].transform.parent = transform;
        }
        instance = this;
        
        if (goalPrefab)
        {
            goalPos = this.transform.position;
            _goal = Instantiate(goalPrefab, goalPos, Quaternion.identity);
            _goal.transform.position = goalPos;
        }

    }
    
    private void Update()
    {
        
        if ( UnityEngine.Random.Range(0, 10000) < 200)
        {
            goalPos = GetRandomGoalPos() + transform.position;
            
            if (_goal)
                _goal.transform.position = goalPos;
        }
    }
    
    Vector3 GetRandomGoalPos()
    {
        Vector3 pos = new Vector3(
            UnityEngine.Random.Range(-limits.x * 0.2f, limits.x * 0.2f),
            UnityEngine.Random.Range(-limits.y * 0.5f, limits.y * 0.5f),
            UnityEngine.Random.Range(-limits.z * 0.2f, limits.z * 0.2f)
        );
        return pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, limits * 2);
    }
}
