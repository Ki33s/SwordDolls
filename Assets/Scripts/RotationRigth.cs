
using UnityEngine;

public class RotationRigth : MonoBehaviour
{
    public int rotateSpeed = 720;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}
