
using UnityEngine;

public class Bleeding : MonoBehaviour
{
    public ParticleSystem blood;
    public int destroyProb = 20;

    public float bleedingMinTime = 0.5f;
    public float bleedingMaxTime = 5f;
    private float bleedingTime = 0;
    public bool bleeding = false;
    // Start is called before the first frame update
    

    public void CheckBleeding()
    {
            if (Random.Range(0, 100) < destroyProb)
            {
                transform.localScale = Vector3.zero;
                bleedingTime = Random.Range(bleedingMinTime, bleedingMaxTime);
                bleeding = true;
                blood.Play();
            }
    }

    // Update is called once per frame
    void Update()
    {
        if (bleeding)
        {
            if (bleedingTime > 0)
                bleedingTime -= Time.deltaTime;
            else
            {
                bleeding = false;
                blood.Stop();
            }
        }
    }
}
