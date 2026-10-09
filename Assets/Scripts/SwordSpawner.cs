using UnityEngine;

public class SwordSpawner : MonoBehaviour
{
    public float weaponLSpawnProb = 0.5f;
    public GameObject[] mainWeapons;
    public GameObject[] leftWeapons;
    // Start is called before the first frame update
    void Start()
    {
        GameObject newWeapon = Instantiate(mainWeapons[Random.Range(0, mainWeapons.Length)], transform.position, transform.rotation, transform);

        if (!newWeapon.GetComponent<Sword>().twoHanded)
        {
            if (Random.Range(0f, 1f) < weaponLSpawnProb)
            {
                GameObject handlerL = transform.root.GetComponent<Body>().handlerL;
                GameObject leftSword = Instantiate(leftWeapons[Random.Range(0, leftWeapons.Length)], handlerL.transform.position, handlerL.transform.rotation, handlerL.transform);
                transform.root.GetComponent<Body>().EquipLeft(leftSword);
            }
        }
    }
}
