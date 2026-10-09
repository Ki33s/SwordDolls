
using UnityEngine;
using System.Collections.Generic;

public class LootAllRunes : MonoBehaviour
{
    public List<GameObject> bronzeRunes = new List<GameObject>();
    public List<GameObject> silverRunes = new List<GameObject>();
    public List<GameObject> goldRunes = new List<GameObject>();
    private List<GameObject> aviableRunes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        MakeRunesDeck();
        Instantiate(aviableRunes[Random.Range(0, aviableRunes.Count)], transform.position, Quaternion.identity);
    }

    private void MakeRunesDeck()
    {
        for (int i = 0; i < bronzeRunes.Count; i++)
        {
            for (int l = 0; l < 3; l++)
            {
                aviableRunes.Add(bronzeRunes[i]);
            }
        }

        for (int i = 0; i < silverRunes.Count; i++)
        {
            for (int l = 0; l < 2; l++)
            {
                aviableRunes.Add(silverRunes[i]);
            }
        }

        for (int i = 0; i < goldRunes.Count; i++)
        {
            aviableRunes.Add(goldRunes[i]);
        }
    }
}
