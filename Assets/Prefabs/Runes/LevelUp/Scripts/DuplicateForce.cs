
using UnityEngine;

public class DuplicateForce : MonoBehaviour
{
    public float duplicateForceKPlus = 0.25f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().duplicateForceK += duplicateForceKPlus;
    }
}
