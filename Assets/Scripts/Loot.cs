
using UnityEngine;

public class Loot : MonoBehaviour
{
    public int force = 500;
    public int prob = 50;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<Rigidbody>())
        {
            Vector2 xzDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            GetComponent<Rigidbody>().AddForce(new Vector3(xzDir.x, 2f, xzDir.y) * force);
        }
        
    }

}
