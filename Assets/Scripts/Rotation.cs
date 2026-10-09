
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public int rotateSpeed = 180;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
