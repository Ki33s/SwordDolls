
using UnityEngine;

public class HealthPart : MonoBehaviour
{
    public float maxSpeed = 10;
    public float speedK = 1;
    private Rigidbody rig;

     void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckMaxSpeed();
    }

    void CheckMaxSpeed()
    {
        float velocityMagnitude = rig.velocity.magnitude;
        if (velocityMagnitude > maxSpeed)
            rig.velocity *= maxSpeed / velocityMagnitude;

    }
}
