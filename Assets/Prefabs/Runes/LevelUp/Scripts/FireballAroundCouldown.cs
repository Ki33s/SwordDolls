
using UnityEngine;

public class FireballAroundCouldown : MonoBehaviour
{
    public float fireballAroundCouldownK = 0.25f;
    private GameObject playerSword;

    private void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().fireballAroundCouldown -= playerSword.GetComponent<Sword>().fireballAroundCouldown * fireballAroundCouldownK;
    }
}
