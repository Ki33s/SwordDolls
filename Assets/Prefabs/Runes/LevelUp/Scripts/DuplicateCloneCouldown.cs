
using UnityEngine;

public class DuplicateCloneCouldown : MonoBehaviour
{
    public float duplicateCloneCouldownK = 0.25f;
    private GameObject playerSword;

    private void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().duplicateCloneCouldown -= playerSword.GetComponent<Sword>().duplicateCloneCouldown * duplicateCloneCouldownK;
    }
}
