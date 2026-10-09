
using UnityEngine;

public class DepthMask : MonoBehaviour
{
    public GameObject target;
    public Vector3 deltaVector;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = target.transform.position + deltaVector;
    }
}
