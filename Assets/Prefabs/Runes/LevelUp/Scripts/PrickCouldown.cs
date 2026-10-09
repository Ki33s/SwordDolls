
using UnityEngine;

public class PrickCouldown : MonoBehaviour
{
    public float prickCouldownK = 0.25f;
    private GameObject playerSword;

    private void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().prickCouldown -= playerSword.GetComponent<Sword>().prickCouldown * prickCouldownK;
    }
}
