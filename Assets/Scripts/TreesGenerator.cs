using UnityEngine;

public class TreesGenerator : MonoBehaviour
{
    public GameObject[] trees;
    public int minAmount = 2;
    public int maxAmount = 5;
    public float deltaPos = 3.5f;
    // Start is called before the first frame update
     void Start()
    {
        GameObject city = GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().nowCity;

        int randNum = Random.Range(minAmount, maxAmount + 1);

        for (int i = 0; i < randNum; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-deltaPos, deltaPos), transform.position.y - 0.1f, transform.position.z + Random.Range(-deltaPos, deltaPos));

            if(city.GetComponent<City>().CheckRoadFree(pos))
            {
                GameObject newTree = Instantiate(trees[Random.Range(0, trees.Length)], pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            }
        }
    }
}
