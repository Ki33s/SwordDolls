using UnityEngine;

public class SkillTrigger : MonoBehaviour
{
    public int minDamage;
    public int maxDamage;
    public int forceBack;
    public int dollId;
    public GameObject damageScore;
    private bool damageMade = false;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider hitCollider)
    {
        if(!damageMade)
        {
            if (hitCollider.GetComponent<Impact>())
            {
                if (hitCollider.GetComponent<Impact>().dollId != dollId)
                {
                    if (hitCollider.GetComponent<Health>())
                    {
                            damageMade = true;
                            int damage = Random.Range(minDamage,maxDamage);


                            Vector3 hitPos = hitCollider.ClosestPoint(transform.position);
                            GameObject hitEffect = Instantiate(hitCollider.GetComponent<Impact>().effect, hitPos, Quaternion.LookRotation(transform.position - hitCollider.transform.position, -transform.up));
                            Destroy(hitEffect, 3);

                            Vector3 forceDir = (transform.position - hitCollider.transform.position).normalized;
                            forceDir = new Vector3(forceDir.x, 0, forceDir.z);

                            if (forceBack > 0)
                                hitCollider.GetComponent<Health>().Damage(damage, hitCollider.GetComponent<Rigidbody>(), forceBack, -forceDir);

                            GameObject score = Instantiate(damageScore, hitPos, Quaternion.identity);
                            score.GetComponent<TextMesh>().text = damage.ToString();
                        }
                    }

            }
        }
    }
}
