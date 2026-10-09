
using UnityEngine;
using System.Collections.Generic;

public class LevelUp : MonoBehaviour
{
    public GameObject playerSword;

    public List<GameObject> allRunes = new List<GameObject>();
    public List<GameObject> runes = new List<GameObject>();
    public List<int> runesId = new List<int>();

    public float deltaX = 2;
    public float deltaY = 2;

    public GameObject leftRune;
    public GameObject centerRune;
    public GameObject rightRune;
    // Start is called before the first frame update
    void Start()
    {
        MakeRunesDeck();
        SpawnRunes();
    }

    void MakeRunesDeck()
    {
        GameObject globalPars = GameObject.Find("GlobalParameters");

        allRunes = globalPars.GetComponent<GlobalParameters>().runes;

        for (int i = 0; i < allRunes.Count; i++)
        {
            int runeType = allRunes[i].GetComponent<LootRune>().runeType;

            for(int k = 0; k < runeType; k++)
            {
                runes.Add(allRunes[i]);
                runesId.Add(i);
            }
        }
    }

    void SpawnRunes()
    {
        int randNum = Random.Range(0, runes.Count);
        leftRune = Instantiate(runes[randNum], new Vector3(transform.position.x - deltaX, transform.position.y + deltaY, transform.position.z), Quaternion.identity);
        leftRune.GetComponent<LootRune>().nowLevelUp = gameObject;
        leftRune.GetComponent<LootRune>().runeId = runesId[randNum];
        runes.RemoveAt(randNum);
        runesId.RemoveAt(randNum);

        randNum = Random.Range(0, runes.Count);
        centerRune = Instantiate(runes[randNum], new Vector3(transform.position.x, transform.position.y + deltaY, transform.position.z), Quaternion.identity);
        centerRune.GetComponent<LootRune>().nowLevelUp = gameObject;
        centerRune.GetComponent<LootRune>().runeId = runesId[randNum];
        runes.RemoveAt(randNum);
        runesId.RemoveAt(randNum);

        randNum = Random.Range(0, runes.Count);
        rightRune = Instantiate(runes[randNum], new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, transform.position.z), Quaternion.identity);
        rightRune.GetComponent<LootRune>().nowLevelUp = gameObject;
        rightRune.GetComponent<LootRune>().runeId = runesId[randNum];
        runes.RemoveAt(randNum);
        runesId.RemoveAt(randNum);
    }

    public void Finish()
    {
        playerSword.GetComponent<Sword>().FinshLevelUp();
        leftRune.GetComponent<LootRune>().DestroyRune();
        centerRune.GetComponent<LootRune>().DestroyRune();
        rightRune.GetComponent<LootRune>().DestroyRune();

        Destroy(gameObject);
    }
}
