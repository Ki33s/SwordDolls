
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject[] items;
    public float genProb = 0.5f;
    public bool rotated = false;
    // Start is called before the first frame update
    void Start()
    {
       if(Random.Range(0f,1f) < genProb)
        {
            Quaternion rot = transform.rotation;
            if(rotated)
                rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

            Instantiate(items[Random.Range(0, items.Length)], transform.position, rot, transform);
        }
    }
}
