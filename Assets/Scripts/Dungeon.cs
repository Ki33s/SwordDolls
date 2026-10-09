using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public int floorTilesNum = 5;
    public int anvilLength = 2;
    public float deltaPos = 5;
    public int height = 1;
    public int deleteLevelDelta = 2;
    private int level = 0;
    private int enemyPower = 0;
    private int anvillNum = 1;
    private int anvillLevel = 1;

    public GameObject levelUp;

    public List<GameObject> allEnemies = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();
    public Material[] enemyMaterials;
    private List<GameObject> enemyList = new List<GameObject>();
    public GameObject pillar;
    public GameObject startAnvil;
    public GameObject anvill;
    public int pillarGenProb = 50;

    //floor
    public GameObject[] floorTiles;
    public GameObject[] groundMoods;
    public int moodGenProb = 10;

    //walls
    public GameObject[] wallTiles;
    public GameObject wallWoods;
    public GameObject[] doorsTiles;
    public int probWallWoods = 50;
    public Material[] floorMaterials;
    public Material[] transMaterials;


    //Chest
    public int chestGenProb = 50;
    public GameObject chestGo;
    //BoxesBarrelsEtc
    public int minBoxes = 0;
    public int maxBoxes = 10;
    public GameObject[] boxes;

    //private int floorNowNum;
    private int floorNowMaterial;

    private float deltaCentered;

    private GameObject nowDoor;
    private int enemiesCount;

    // Start is called before the first frame update
    void Start()
    {

        deltaCentered = (floorTilesNum * deltaPos) / 2 - deltaPos / 2;

        //GenerateStartAnvil();
        AddNewEnemies();

        for (int i = 0; i < floorTilesNum; i++)
        {
            int nowTile = Random.Range(0, wallTiles.Length);

            for (int k = 0; k < height; k++)
            {
                Vector3 pos = new Vector3(transform.position.x - deltaCentered + deltaPos * i, transform.position.y + deltaPos * k, transform.position.z - 2.5f);

                GameObject newTile = Instantiate(wallTiles[nowTile], pos, Quaternion.Euler(0, 0, 0));
                newTile.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                newTile.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
                newTile.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];
            }
        }

        GenerateLevel();
        
        
    }

    void AddNewEnemies()
    {
        for(int i = 0; i< allEnemies.Count; i++)
        {
            if (allEnemies[i].GetComponent<EnemyPower>().power == anvillLevel)
                enemies.Add(allEnemies[i]);
        }
    }

    void GenerateAnvill()
    {
        floorNowMaterial = Random.Range(0, floorMaterials.Length);

        OpenDoor();
        GenerateFloor(anvilLength, floorTilesNum);
        GenerateWalls(anvilLength, floorTilesNum);
        GameObject newAnvill = Instantiate(anvill, new Vector3(transform.position.x, 0, transform.position.z + anvilLength * deltaPos - deltaPos +1f), Quaternion.identity);
        newAnvill.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
        DeleteLevel();

        anvillNum++;
        anvillLevel++;
        AddNewEnemies();

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + anvilLength * deltaPos);
    }

    public void GenerateLevel()
    {
        floorNowMaterial = Random.Range(0, floorMaterials.Length);

        OpenDoor();
        GenerateFloor(floorTilesNum, floorTilesNum);
        GenerateWalls(floorTilesNum, floorTilesNum);
        GeneratePillars();
        GenerateChest();
        GenerateBoxes();
        AllEnemies();
        DeleteLevel();

        level++;
        anvillNum++;

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z +  floorTilesNum * deltaPos);
    }

    void DeleteLevel()
    {
        float deletePosZ = (level - deleteLevelDelta) * floorTilesNum * deltaPos;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        for(int i = 0; i <allObjects.Length; i++)
        {
            if(allObjects[i].transform.position.z < deletePosZ)
            if(allObjects[i].tag != "MainCamera" && allObjects[i].tag != "SceneManager")
            {
                if(!allObjects[i].transform.root.GetComponent<Body>())
                    Destroy(allObjects[i]);
            }
        }
    }

    public void EnemyDie()
    {
        enemiesCount--;

        if (enemiesCount == 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<SwordHero>().CheckLevelUp();
            //ProceedNextLevel();
        } 
    }

    public void ProceedNextLevel()
    {
        if (anvillNum % 3 == 0)
            GenerateAnvill();
        else
            GenerateLevel();
    }

    void OpenDoor()
    {
        if(nowDoor)
           nowDoor.GetComponent<OpenDoor>().opened = true;
    }

    void GenerateChest()
    {
        if(Random.Range(0,100) < chestGenProb)
        {
            float delta = floorTilesNum * deltaPos;
            GameObject chest = Instantiate(chestGo, new Vector3(transform.position.x + Random.Range(0, delta) - delta/2, 0, transform.position.z + Random.Range(0, delta)), Quaternion.EulerAngles(0, Random.Range(0, 360), 0));
            chest.GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
        }
    }

    void GenerateBoxes()
    {
        int randNum = Random.Range(minBoxes, maxBoxes);
        float delta = floorTilesNum * deltaPos;

        for (int i =0; i< randNum; i++)
        {
            GameObject box = Instantiate(boxes[Random.Range(0,boxes.Length)], new Vector3(transform.position.x + Random.Range(0, delta) - delta / 2, 0, transform.position.z + Random.Range(0, delta)), Quaternion.EulerAngles(0,Random.Range(0,360),0));
            box.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
        }
    }

        void GeneratePillars()
    {
        if(Random.Range(0,100) < pillarGenProb)
        {
            float delta = floorTilesNum * deltaPos / 4;
            GameObject pillar1 = Instantiate(pillar, new Vector3(transform.position.x - delta, 0, transform.position.z + delta), Quaternion.identity);
            pillar1.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
            pillar1.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
            pillar1.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];

            GameObject pillar2 = Instantiate(pillar, new Vector3(transform.position.x + delta, 0, transform.position.z + delta), Quaternion.identity);
            pillar2.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
            pillar2.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
            pillar2.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];

            GameObject pillar3 =  Instantiate(pillar, new Vector3(transform.position.x + delta, 0, transform.position.z + 3 * delta - 2.5f), Quaternion.identity);
            pillar3.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
            pillar3.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
            pillar3.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];

            GameObject pillar4 =  Instantiate(pillar, new Vector3(transform.position.x - delta, 0, transform.position.z + 3 * delta - 2.5f), Quaternion.identity);
            pillar4.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
            pillar4.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
            pillar4.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];
        }
    }

    void GenerateStartAnvil()
    {
        GameObject newAnvill = Instantiate(startAnvil, new Vector3(transform.position.x, transform.position.y, transform.position.z + floorTilesNum * deltaPos/2), Quaternion.identity);
        newAnvill.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
    }

    void GenerateFloor(int levelLength, int levelWidth)
    {
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelLength; j++)
            {
                float deltaZ = deltaPos * j;
                Vector3 pos = new Vector3(transform.position.x - deltaCentered + deltaPos * i, transform.position.y, transform.position.z + deltaZ);
                GameObject newTile = Instantiate(floorTiles[Random.Range(0, floorTiles.Length)], pos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
                newTile.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];

                if (Random.Range(0, 100) < moodGenProb)
                {
                    GenerateMood(pos);
                }
            }
        }
    }

    void AllEnemies()
    {
        enemyPower+=2;

        int totalPower = enemyPower;

        enemyList.Clear();

        for(int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].GetComponent<EnemyPower>().power <= totalPower)
            {
                enemyList.Add(enemies[i]);
            }
        }

        SpawnEnemy(totalPower);
        
    }

    void SpawnEnemy(int totalPower)
    {
        int enemyNum = Random.Range(0, enemyList.Count);
        //enemiesCount = 0;

        if(enemyList[enemyNum].GetComponent<EnemyPower>().power <= totalPower)
        {
            totalPower -= enemyList[enemyNum].GetComponent<EnemyPower>().power;

            GameObject nowEnemy = Instantiate(enemyList[enemyNum], new Vector3(Random.Range(1, (floorTilesNum - 1) * deltaPos) + transform.position.x - deltaCentered, 0.1f, transform.position.z + deltaCentered), Quaternion.identity);
            nowEnemy.GetComponent<Body>().rendererModel.GetComponent<Renderer>().material = enemyMaterials[Random.Range(0, enemyMaterials.Length)];
            enemiesCount++;
        }
        else
        {
            enemyList.RemoveAt(enemyNum);
        }

        if (totalPower > 0)
            SpawnEnemy(totalPower);
    }

    void GenerateMood(Vector3 pos)
    {
        GameObject newMood = Instantiate(groundMoods[Random.Range(0, groundMoods.Length)], pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
        newMood.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
    }

    void GenerateWalls(int levelLength, int levelWidth)
    {
        //int entranceDoorNum = Random.Range(0, 4);
        int entranceDoorTile = Random.Range(0, levelWidth);
        bool woods = false;


        //walls bottom

       /* */

        //walls top


        if (Random.Range(0, 100) < probWallWoods)
            woods = true;

        for (int i = 0; i < levelWidth; i++)
        {
            int nowTile = Random.Range(0, wallTiles.Length);

            for (int k = 0; k < height; k++)
            {
                float deltaZ = levelLength * deltaPos - 2.5f;
                Vector3 pos = new Vector3(transform.position.x - deltaCentered + deltaPos * i, transform.position.y + deltaPos * k, transform.position.z + deltaZ);

                if (entranceDoorTile == i && k == 0)
                {
                    nowDoor = Instantiate(doorsTiles[Random.Range(0, doorsTiles.Length)], pos, Quaternion.Euler(0, 180, 0));
                    nowDoor.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                    nowDoor.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
                    nowDoor.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];

                    for(int j = 0; j < nowDoor.transform.childCount; j++)
                    {
                        if(nowDoor.transform.GetChild(j).GetComponent<Wall>())
                        {
                            nowDoor.transform.GetChild(j).GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
                            nowDoor.transform.GetChild(j).GetComponent<Wall>().trans = transMaterials[floorNowMaterial];
                        }
                    }
                }
                else
                {
                    GameObject newTile = Instantiate(wallTiles[nowTile], pos, Quaternion.Euler(0, 180, 0));
                    newTile.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                    newTile.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
                    newTile.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];

                    if (woods)
                    {
                        GameObject newWoods = Instantiate(wallWoods, pos, Quaternion.Euler(0, 180, 0), newTile.transform);
                        newWoods.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                    }
                }
            }
        }

        //walls left

        if (Random.Range(0, 100) < probWallWoods)
            woods = true;

        for (int i = 0; i < levelLength; i++)
        {
            int nowTile = Random.Range(0, wallTiles.Length);

            for (int k = 0; k < height; k++)
            {
                Vector3 pos = new Vector3(transform.position.x - deltaCentered - 2.5f, transform.position.y + deltaPos * k, transform.position.z + deltaPos * i);

                GameObject newTile = Instantiate(wallTiles[nowTile], pos, Quaternion.Euler(0, 90, 0));
                newTile.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                newTile.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
                newTile.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];

                if (woods)
                {
                    GameObject newWoods = Instantiate(wallWoods, pos, Quaternion.Euler(0, 90, 0), newTile.transform);
                    newWoods.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                }
            }
        }

        //walls right

        if (Random.Range(0, 100) < probWallWoods)
            woods = true;

        for (int i = 0; i < levelLength; i++)
        {
            int nowTile = Random.Range(0, wallTiles.Length);

            for (int k = 0; k < height; k++)
            {
                Vector3 pos = new Vector3(transform.position.x + deltaCentered + 2.5f, transform.position.y + deltaPos * k, transform.position.z + deltaPos * i);

                GameObject newTile = Instantiate(wallTiles[nowTile], pos, Quaternion.Euler(0, 270, 0));
                newTile.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                newTile.GetComponent<Wall>().main = floorMaterials[floorNowMaterial];
                newTile.GetComponent<Wall>().trans = transMaterials[floorNowMaterial];

                if (woods)
                {
                    GameObject newWoods = Instantiate(wallWoods, pos, Quaternion.Euler(0, 270, 0), newTile.transform);
                    newWoods.transform.GetChild(0).GetComponent<Renderer>().material = floorMaterials[floorNowMaterial];
                }
            }
        }
    }
}
