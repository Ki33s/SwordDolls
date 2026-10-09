
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public float hp = 10f;
    public float couldown = 0.2f;
    private float timer = 0;
    private bool destroyed = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider myTrigger)
    {
        if(!destroyed && timer <= 0)
        {
            if (myTrigger.GetComponent<Rigidbody>())
            {
                if (myTrigger.GetComponent<Rigidbody>().velocity.magnitude > 0)
                {
                    hp -= myTrigger.GetComponent<Rigidbody>().velocity.magnitude;

                    if(hp < 0)
                    {
                        destroyed = true;
                        GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
            }
        }
        
    }
}
