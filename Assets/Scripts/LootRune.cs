
using UnityEngine;

public class LootRune : MonoBehaviour
{
    public GameObject useRune;
    public GameObject runeAdded;
    public GameObject Fx;
    public int runeType = 1;
    public GameObject nowLevelUp;
    public int runeId;
    
    public void DestroyRune()
    {
        GameObject newFx = Instantiate(Fx, transform.position, Quaternion.identity);
        Destroy(newFx, 3);
        Destroy(gameObject);
    }
}
