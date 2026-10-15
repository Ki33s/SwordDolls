using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public GameObject groundTile;
    public GameObject groundCollider;
    public Material[] groundMaterials;
    public GameObject[] grassGenerators;
    public GameObject[] treeGenerators;
    public GameObject[] hedges;
    public GameObject[] houses;
    public Material[] enemyMaterials;
    public GameObject crystall;
    public int tilesNum = 5;
    public int tilesLength = 7;
    public int tilesTypes = 2;
    public int tilesPodTypes = 3;
    public float tileTypeChangeProb = 0.2f;
    public float tilePodTypeChangeProb = 0.4f;
    public bool anvillFinish = false;
    public GameObject anvill;

    public GameObject roadTile;
    public int roadLength = 3;
    public int newRoadDeltaAngle = 20;
    public float newHouseK = 0.2f;

    public int minPowerPerLevel = 3;
    public int maxPowerPerLevel = 6;
    public int newLevelMinEnemyPower = 3;

    public float newRoadK = 0.05f;
    public int level = 0;
    private int housePower;
    private int enemiesKilledPower = 0;
    public int NeutralizedCount=>enemiesKilledPower;
    private float newHouseProb = 0f;
    private float newRoadProb = 0f;

    public float newLevelCouldown = 3f;
    public bool newLevelStart = false;

    private List<GameObject> housesAll = new List<GameObject>();
    private List<float> housesRadius = new List<float>();
    private List<Vector3> roadPos = new List<Vector3>();
    private List<int> tileMat = new List<int>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> allEnemies = new List<GameObject>();
    private GameObject nowCrystall;
    public Crystall CurrentCrystal=>nowCrystall?nowCrystall.GetComponent<Crystall>():null;
    private bool crystalShieldDestroyed = false;
    public bool crystalDestroyed = false;

    //public List<GameObject> genTrees = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().nowCity = gameObject;
        GenerateGround();
        var stage=GetComponent<CampaignStage>();
        if(stage && stage.authoredEncounters) { nowCrystall=Instantiate(crystall,new Vector3(0,0,-6),Quaternion.identity,transform); return; }
        housePower = Random.Range(minPowerPerLevel, maxPowerPerLevel+1);
        GenerateRoad();
        GenerateCrystal();
        GenerateTrees();
    }

    private void FixedUpdate()
    {
        TryOpenCrystal();
        if(newLevelStart)
        {
            if (newLevelCouldown > 0)
                newLevelCouldown -= Time.deltaTime;
            else
                NewLevel();
        }
    }

    private void GenerateAnvil()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        GameObject newAnvill = Instantiate(anvill, new Vector3(player.transform.position.x,0, player.transform.position.z), Quaternion.identity, transform);
        player.GetComponent<Sword>().Evolution(newAnvill);
    }

    private void NewLevel()
    {
        bool player = false;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            player = false;

            if(!allEnemies[i])continue;
            if(allEnemies[i].transform.root.GetComponent<Body>())
            {
                if (allEnemies[i].transform.root.GetComponent<Body>().sword && allEnemies[i].transform.root.GetComponent<Body>().sword.GetComponent<SwordHero>())
                    player = true;
            }

            if (!player)
                Destroy(allEnemies[i]);
        }

        Destroy(gameObject);

        Instantiate(GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().city[level],transform.position,Quaternion.identity);
    }

    public void AddEnemy(GameObject newEnemy)
    {
        enemies.Add(newEnemy);
        allEnemies.Add(newEnemy);
    }

    public void RemoveEnemy(GameObject newEnemy)
    {
        if(!crystalDestroyed && enemies.Contains(newEnemy))
        {
            enemiesKilledPower += newEnemy.GetComponent<EnemyPower>().power;
            enemies.Remove(newEnemy);

            if (enemiesKilledPower >= newLevelMinEnemyPower && !crystalShieldDestroyed && (!GetComponent<CampaignStage>() || GetComponent<CampaignStage>().BossDefeated))
            {
                crystalShieldDestroyed = true;
                nowCrystall.GetComponent<Crystall>().CrystalReady();
            }

            if(enemies.Count == 0)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<SwordHero>().CheckLevelUp();
            }
        }
    }

    void TryOpenCrystal()
    {
        if(!crystalDestroyed && !crystalShieldDestroyed && enemiesKilledPower >= newLevelMinEnemyPower && (!GetComponent<CampaignStage>() || GetComponent<CampaignStage>().BossDefeated))
        { crystalShieldDestroyed = true; nowCrystall.GetComponent<Crystall>().CrystalReady(); }
    }
    public void CrystallDestroyed()
    {
        if(crystalDestroyed)return;
        crystalDestroyed = true;

        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject enemyHead = enemies[i].GetComponent<Body>().head;
            Vector3 dir = enemyHead.transform.position - nowCrystall.transform.position;
            enemies[i].GetComponent<MainHealth>().Damage(enemies[i].GetComponent<MainHealth>().maxHealth*3, enemyHead.GetComponent<Rigidbody>(), 1000, dir);
        }

        for(int i = 0; i < housesAll.Count; i++)
        {
            if(housesAll[i].GetComponent<House>())
            housesAll[i].GetComponent<House>().CrystalDestroyed();
        }

        if(GetComponent<CampaignStage>() && GetComponent<CampaignStage>().number == 5)
        {
            CampaignHUD.Active.Win(); return;
        }
        if(!anvillFinish)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<SwordHero>().NewCityLevel();
            newLevelStart = true;
        }
        else
        {
            GenerateAnvil();
        }
        
    }

    void GenerateCrystal()
    {
        float deltaPos = (tilesNum - 1) * tilesLength / 2;
        Vector3 pos = new Vector3(Random.Range(-deltaPos, deltaPos), 0, Random.Range(-deltaPos, deltaPos));

        if(CheckPlaceFree(pos))
        {
            nowCrystall = Instantiate(crystall, pos, Quaternion.Euler(0, Random.Range(0, 360),0), transform);
            housesAll.Add(nowCrystall);
            housesRadius.Add(3);
        }
        else
        {
            GenerateCrystal();
        }
    }

    void GenerateTrees()
    {
        float deltaPos = (tilesNum - 1) * tilesLength / 2;

        int l = 0;

        for (int i = 0; i < tilesNum; i++)
        {
            for (int k = 0; k < tilesNum; k++)
            {
                Instantiate(treeGenerators[tileMat[l]], new Vector3(transform.position.x - deltaPos + i * tilesLength, -0.2f, transform.position.z - deltaPos + k * tilesLength), Quaternion.identity, transform);
                l++;
            }
        }
    }

    public bool CheckRoadFree(Vector3 pos)
    {
        bool free = false;

        float deltaPos = (tilesNum - 1) * tilesLength / 2;

        if (pos.x >= -deltaPos && pos.x <= deltaPos && pos.z >= -deltaPos && pos.z <= deltaPos)
        {
            free = true;
        }

        if (free)
        {
            for (int i = 0; i < housesAll.Count; i++)
            {
                if (pos.x > housesAll[i].transform.position.x - housesRadius[i] && pos.x < housesAll[i].transform.position.x + housesRadius[i] && pos.z > housesAll[i].transform.position.z - housesRadius[i] && pos.z < housesAll[i].transform.position.z + housesRadius[i])
                {
                    free = false;
                }
            }
        }

        if (free)
        {
            for (int i = 0; i < roadPos.Count; i++)
            {
                if (pos.x > roadPos[i].x - roadLength && pos.x < roadPos[i].x + roadLength && pos.z > roadPos[i].z - roadLength && pos.z < roadPos[i].z + roadLength)
                {
                    free = false;
                }
            }
        }

        return free;
    }

    void GenerateRoad()
    {
        float deltaPos = (tilesNum - 1) * tilesLength / 2;
        float x;
        float y;
        int angleRot = 0;

        if(Random.Range(0, 2) == 0)
        {
            x = Random.Range(-deltaPos, deltaPos);
            y = deltaPos;
            angleRot = 180;

            if (Random.Range(0, 2) == 0)
            {
                y *= -1;
                angleRot = 0;
            }
        }
        else
        {
            y = Random.Range(-deltaPos, deltaPos);
            x = deltaPos;
            angleRot = -90;

            if (Random.Range(0, 2) == 0)
            {
                x *= -1;
                angleRot = 90;
            }
        }

        Vector3 startPos = new Vector3(x, -0.1f, y);
        MakeRoadPath(startPos, angleRot + Random.Range(-45,45));

        if(housePower > 0)
        {
            GenerateRoad();
        }

    }

    bool CheckPlaceFree(Vector3 pos)
    {
        bool free = false;

        float deltaPos = (tilesNum - 1) * tilesLength / 2;

        if (pos.x >= -deltaPos && pos.x <= deltaPos && pos.z >= -deltaPos && pos.z <= deltaPos)
        {
            free = true;
        }

        if(free)
        {
            for(int i=0; i< housesAll.Count; i++)
            {
                if(pos.x > housesAll[i].transform.position.x - housesRadius[i] && pos.x < housesAll[i].transform.position.x + housesRadius[i] && pos.z > housesAll[i].transform.position.z - housesRadius[i] && pos.z < housesAll[i].transform.position.z + housesRadius[i])
                {
                    free = false;
                }
            }
        }

        return free;
    }

    void MakeRoadPath(Vector3 pos, int yAngle)
    {
        GameObject newRoad = Instantiate(roadTile, pos, Quaternion.Euler(0, yAngle, 0), transform);
        newRoad.transform.position +=  newRoad.transform.forward * roadLength;
        int newAngle = yAngle + Random.Range(-newRoadDeltaAngle, newRoadDeltaAngle);
        newRoad.transform.rotation = Quaternion.Euler(0, newAngle, 0);
        roadPos.Add(newRoad.transform.position);

        if(housePower > 0 && Random.Range(0f,1f) < newHouseProb)
        {
            int houseNum = Random.Range(0, houses.Length);
            Vector3 housePos = newRoad.transform.position;
            bool right = true;

            if (Random.Range(0, 2) == 0)
            {
                housePos -= newRoad.transform.right * (houses[houseNum].GetComponent<House>().houseLength + roadLength);
            }
            else
            {
                housePos += newRoad.transform.right * (houses[houseNum].GetComponent<House>().houseLength + roadLength);
                right = false;
            }

            if(CheckPlaceFree(housePos))
            {
                newHouseProb = 0;
                int houseRandNum = Random.Range(0, houses.Length);

                GameObject newHouse = Instantiate(houses[houseRandNum], housePos, newRoad.transform.rotation, newRoad.transform);
                
                int houseMat = Random.Range(0, enemyMaterials.Length);
                SetMaterial(newHouse.transform.GetChild(0).gameObject, houseMat);
                newHouse.GetComponent<House>().enemyMaterial = enemyMaterials[houseMat];

                housePower -= newHouse.GetComponent<House>().housePower;

                housesAll.Add(newHouse);
                housesRadius.Add(newHouse.GetComponent<House>().houseRadius);

                if (right)
                {
                    newHouse.transform.localRotation = Quaternion.Euler(0, -90, 0);
                }
                else
                {
                    newHouse.transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            }
        }
        else
        {
            newHouseProb += (1 - newHouseProb) * newHouseK;
        }

        if (CheckPlaceFree(newRoad.transform.position))
        {
            if (Random.Range(0f, 1f) < newRoadProb)
            {
                newRoadProb = 0;
                int angle = Random.Range(-45, 45);

                if (Random.Range(0, 2) == 0)
                    angle += 90;
                else
                    angle -= 90;

                angle += newAngle;
                MakeRoadPath(newRoad.transform.position, angle);
            }
            else
            {
                newRoadProb += (1 - newRoadProb) * newRoadK;
            }

            MakeRoadPath(newRoad.transform.position, newAngle);
        }
    }


    void SetMaterial(GameObject housePart, int materialNum)
    {
        if (housePart.GetComponent<Renderer>())
        {
            housePart.GetComponent<Renderer>().material = enemyMaterials[materialNum];
            //housePart.GetComponent<Renderer>().material.renderQueue = 3105;
        }
            

        for (int i = 0; i < housePart.transform.childCount; i++)
        {
            SetMaterial(housePart.transform.GetChild(i).gameObject, materialNum);
        }
    }

    void GenerateGround()
    {
        var groundVisuals = new GameObject("Ground visuals");
        groundVisuals.transform.SetParent(transform, false);
        groundVisuals.AddComponent<TerrainVisuals>();
        float deltaPos = (tilesNum - 1) * tilesLength / 2;

        int tileType = Random.Range(0, tilesTypes);
        int tilePodType = Random.Range(0, tilesPodTypes);

        int firstTyleType = Random.Range(0, tilesTypes);
        int firstTylePodType = Random.Range(0, tilesPodTypes);

        for (int i=0; i<tilesNum;i++)
        {
            for(int k=0;k<tilesNum;k++)
            {
                if(k==0 && i!=0)
                {
                    tileType = firstTyleType;
                    tilePodType = firstTylePodType;
                }

                if(Random.Range(0f,1f) < tileTypeChangeProb)
                {
                    tileType = Random.Range(0, tilesTypes);
                    tilePodType = Random.Range(0, tilesPodTypes);
                }
                else if(Random.Range(0f, 1f) < tilePodTypeChangeProb)
                {
                    tilePodType = Random.Range(0, tilesPodTypes);
                }

                if (k==0)
                {
                    firstTyleType = tileType;
                    firstTylePodType = tilePodType;
                }

                GameObject newGroundTile = Instantiate(groundTile, new Vector3(transform.position.x-deltaPos + i*tilesLength,-0.2f, transform.position.z - deltaPos + k * tilesLength), Quaternion.Euler(0, Random.Range(0, 360), 0),groundVisuals.transform);
                TerrainVisuals.RemoveCollision(newGroundTile);

                int tileNum = tileType * (tilesTypes-1) * tilesPodTypes + tilePodType;
                newGroundTile.GetComponent<Renderer>().material = groundMaterials[tileNum];
                tileMat.Add(tileNum);

                Instantiate(grassGenerators[tileNum], new Vector3(transform.position.x - deltaPos + i * tilesLength, 0, transform.position.z - deltaPos + k * tilesLength), Quaternion.identity, groundVisuals.transform);

                if (i == 0)
                {
                    GameObject newHedge = Instantiate(hedges[Random.Range(0, hedges.Length)], new Vector3(transform.position.x - deltaPos + i * tilesLength - tilesLength / 2, -1, transform.position.z - deltaPos + k * tilesLength), Quaternion.Euler(0,90,0), transform);
                    newHedge.transform.GetChild(0).GetComponent<Renderer>().material = groundMaterials[tileNum];
                    //newHedge.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = 3105;
                }

                if (i == tilesNum-1)
                {
                    GameObject newHedge = Instantiate(hedges[Random.Range(0, hedges.Length)], new Vector3(transform.position.x - deltaPos + i * tilesLength + tilesLength / 2, -1, transform.position.z - deltaPos + k * tilesLength), Quaternion.Euler(0, -90, 0), transform);
                    newHedge.transform.GetChild(0).GetComponent<Renderer>().material = groundMaterials[tileNum];
                    //newHedge.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = 3105;
                }

                if (k == 0)
                {
                    GameObject newHedge = Instantiate(hedges[Random.Range(0, hedges.Length)], new Vector3(transform.position.x - deltaPos + i * tilesLength, -1, transform.position.z - deltaPos + k * tilesLength - tilesLength / 2), Quaternion.Euler(0, 0, 0), transform);
                    newHedge.transform.GetChild(0).GetComponent<Renderer>().material = groundMaterials[tileNum];
                    //newHedge.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = 3105;
                }

                if (k == tilesNum - 1)
                {
                    GameObject newHedge = Instantiate(hedges[Random.Range(0, hedges.Length)], new Vector3(transform.position.x - deltaPos + i * tilesLength, -1, transform.position.z - deltaPos + k * tilesLength + tilesLength / 2), Quaternion.Euler(0, 180, 0), transform);
                    newHedge.transform.GetChild(0).GetComponent<Renderer>().material = groundMaterials[tileNum];
                    //newHedge.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = 3105;
                }
            }
        }

        GameObject newGroundCollider = Instantiate(groundCollider, transform.position, Quaternion.identity, transform);
        var surface = newGroundCollider.GetComponent<BoxCollider>();
        newGroundCollider.transform.localScale = Vector3.one;
        surface.center = Vector3.down * .5f;
        surface.size = new Vector3(tilesNum * tilesLength, 1, tilesNum * tilesLength);
        surface.isTrigger = false;
        surface.enabled = true;

    }
}
