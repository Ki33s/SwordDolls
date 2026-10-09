using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public int housePower = 1;
    public float houseLength = 3;
    public float houseRadius = 9;
    public int playerSeeRadius = 15;
    public List<GameObject> enemy = new List<GameObject>();
    public GameObject door;
    public BoxCollider spawnedCollider;
    public GameObject enemySpawnPlace;
    public ParticleSystem unSpawnedShield;
    public float enemySpawnTime = 1f;
    public GameObject openSound;
    private bool doorOpened = false;
    private GameObject playerSword;
    public Material enemyMaterial;
    private float enemySpawnTimer = 0f;
    private bool finished = false;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GameObject.FindGameObjectWithTag("Player");
        var stage=GetComponentInParent<CampaignStage>(); if(stage && stage.authoredEncounters) { CrystalDestroyed(); enabled=false; }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!playerSword)playerSword=GameObject.FindGameObjectWithTag("Player");
        if(!playerSword)return;
        if(!doorOpened)
        {
            if((playerSword.transform.position - transform.position).magnitude < playerSeeRadius)
            {
                doorOpened = true;
                door.GetComponent<OpenDoor>().opened = true;
                Instantiate(openSound, door.transform.position, Quaternion.identity);
                SpawnEnemy();
            }
        }
        else 
        {
            if (enemySpawnTimer > 0)
            {
                enemySpawnTimer -= Time.deltaTime;
            }
            else if(housePower > 0)
            {
                SpawnEnemy();
            }
            else if(!finished)
            {
                finished = true;
                spawnedCollider.enabled = true;
                door.GetComponent<OpenDoor>().opened = true;
                unSpawnedShield.loop = false;
            }
        }
    }

    public void CrystalDestroyed()
    {
        housePower = 0;

        if (!doorOpened)
            doorOpened = true;

        if(!finished)
        {
            finished = true;
            spawnedCollider.enabled = true;
            door.GetComponent<OpenDoor>().opened = true;
            unSpawnedShield.loop = false;
        }
        
    }

    void SpawnEnemy()
    {
        enemy.RemoveAll(x => !x || x.GetComponent<EnemyPower>().power > housePower);
        if(enemy.Count == 0) { housePower = 0; return; }
        int randNum = Random.Range(0, enemy.Count);

        if(enemy[randNum].GetComponent<EnemyPower>().power <= housePower)
        {
            GameObject newEnemy = Instantiate(enemy[randNum], enemySpawnPlace.transform.position, enemySpawnPlace.transform.rotation);
            newEnemy.GetComponent<Body>().rendererModel.GetComponent<Renderer>().material = enemyMaterial;
            housePower -= newEnemy.GetComponent<EnemyPower>().power;
            enemySpawnTimer = enemySpawnTime;
            GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().nowCity.GetComponent<City>().AddEnemy(newEnemy);
        }
        else
        {
            enemy.RemoveAt(randNum);
        }
    }
}
