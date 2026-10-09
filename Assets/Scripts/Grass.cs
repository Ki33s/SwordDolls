
using UnityEngine;

public class Grass : MonoBehaviour
{
    public float newGrassGenProb = 0.5f;
    public float deltapos = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        if(Random.Range(0f,1f) < newGrassGenProb)
        {
            Instantiate(gameObject, new Vector3(transform.position.x + Random.Range(-deltapos, deltapos), transform.position.y, transform.position.z + Random.Range(-deltapos, deltapos)), Quaternion.Euler(0, Random.Range(0, 360),0), transform);
        }
    }
}
