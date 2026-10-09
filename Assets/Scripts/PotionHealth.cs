
using UnityEngine;

public class PotionHealth : MonoBehaviour
{
    public float healthPerc = 0.3f;
    public float expPerc = 0.3f;
    public float startTimer = 1f;
    public GameObject Fx;
    private float timer = 0f;
    private bool started = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!started)
        {
            timer += Time.deltaTime;

            if (timer >= startTimer)
                started = true;
        }
    }

    void OnTriggerEnter(Collider myTrigger)
    {
        if(started)
        {
            if (myTrigger.tag == "Player")
            {
                if(healthPerc > 0)
                {
                    if(myTrigger.GetComponent<Sword>().hasDoll)
                        myTrigger.transform.root.GetComponent<MainHealth>().Heal(healthPerc);
                }
                    

                if (expPerc > 0)
                    myTrigger.GetComponent<SwordHero>().ExpGain(Mathf.RoundToInt(myTrigger.GetComponent<SwordHero>().level * 10 * expPerc));

                GameObject newFx = Instantiate(Fx, transform.position, Quaternion.identity);
                Destroy(newFx, 3);
                Destroy(gameObject);
            }
        }
    }
}
