
using UnityEngine;

public class Force : MonoBehaviour
{
    public int forceUp = 100;

    private Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rig.AddForce(new Vector3(0, 100 * forceUp * Time.deltaTime, 0));
    }
}
