
using UnityEngine;

public class GrassGenerator : MonoBehaviour
{
    public GameObject[] grass;
    public int minGrassAmount = 5;
    public int maxGrassAmount = 20;
    public float deltaPos = 3.5f;
    // Start is called before the first frame update
    void Start()
    {
        int randNum = Random.Range(minGrassAmount, maxGrassAmount + 1);

        for (int i = 0; i < randNum; i++)
        {
            Instantiate(grass[Random.Range(0, grass.Length)], new Vector3(transform.position.x + Random.Range(-deltaPos, deltaPos), transform.position.y, transform.position.z + Random.Range(-deltaPos, deltaPos)), Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
        }
    }
}
