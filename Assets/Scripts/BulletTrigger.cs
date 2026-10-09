
using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    public int forceBack = 0;
    public int minDamage = 5;
    public int maxDamage = 10;
    public int dollId = 0;
    public int impactType = 0;
    public float lifeTime = 5f;
    public GameObject damageScore;
    public GameObject impactEffect;
    public GameObject impactSound;

    private float time = 0f;


    private void FixedUpdate()
    {
        if(time < lifeTime)
        {
            time += Time.deltaTime;
        }
        else
        {
            GameObject hitEffect = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(hitEffect, 3);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider myTrigger)
    {
        if (myTrigger.GetComponent<Impact>())
        {
            if (myTrigger.GetComponent<Impact>().dollId != dollId)
            {
                GameObject hitEffect = Instantiate(impactEffect, transform.position, Quaternion.LookRotation(transform.position - myTrigger.transform.position, -transform.up));
                Destroy(hitEffect, 3);

                if (impactSound)
                {
                    GameObject nowSound = Instantiate(impactSound, transform.position, Quaternion.identity);
                    nowSound.GetComponent<ImpactSoundEffect>().impactType = impactType;
                }

                if (myTrigger.GetComponent<Health>())
                {
                    int damage = Random.Range(minDamage, maxDamage);
                    if (forceBack > 0)
                    {
                        Vector3 forceDir = (transform.position - myTrigger.transform.position).normalized;
                        forceDir = new Vector3(forceDir.x, 0, forceDir.z);
                        myTrigger.GetComponent<Health>().Damage(damage, myTrigger.GetComponent<Rigidbody>(), forceBack, -forceDir);
                    }


                    GameObject score = Instantiate(damageScore, transform.position, Quaternion.identity);
                    score.GetComponent<TextMesh>().text = damage.ToString();
                }
                    Destroy(gameObject);
            }
        }

    }
}
