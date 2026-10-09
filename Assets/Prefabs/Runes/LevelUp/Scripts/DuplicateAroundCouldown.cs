
using UnityEngine;

public class DuplicateAroundCouldown : MonoBehaviour
{
    public float duplicateAroundCouldownK = 0.25f;
    private GameObject playerSword;

    private void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().duplicateAroundCouldown -= playerSword.GetComponent<Sword>().duplicateAroundCouldown * duplicateAroundCouldownK;
    }
}
