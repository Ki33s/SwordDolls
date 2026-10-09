
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject[] lootGo;

    public GameObject cap;
    public GameObject openSound;
    public float timeOpen = 2f;
    public int angleOpen = 120;
    private float timer = 0f;
    private bool openned = false;
    private bool lootDrooped = false;

    private void FixedUpdate()
    {
        if(openned && ! lootDrooped)
        {
            if(timer < timeOpen)
            {
                timer += Time.deltaTime;
                cap.transform.eulerAngles = new Vector3(cap.transform.eulerAngles.x - angleOpen / timeOpen * Time.deltaTime, cap.transform.eulerAngles.y, cap.transform.eulerAngles.z);
            }
            else
            {
                DropLoot();
            }
        }
    }

    private void DropLoot()
    {
        lootDrooped = true;

        int totalProb = 0;

        for(int i = 0; i < lootGo.Length; i++)
        {
            totalProb += lootGo[i].GetComponent<Loot>().prob;
        }

        int randNum = Random.Range(0, totalProb);
        int nowNum = 0;

        for (int i = 0; i < lootGo.Length; i++)
        {
            nowNum += lootGo[i].GetComponent<Loot>().prob;

            if(randNum < nowNum)
            {
                SpawnLoot(i);
                i = lootGo.Length;
            }
        }
    }

    private void SpawnLoot(int id)
    {
        Instantiate(lootGo[id], transform.position, Quaternion.identity);
    }

    void OnTriggerEnter(Collider myTrigger)
    {
        if(!openned)
        {
            if (myTrigger.tag == "Player")
            {
                openned = true;
                Instantiate(openSound, transform.position, Quaternion.identity);
            }
        }
    }
}
