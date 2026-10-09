
using UnityEngine;

public class DuplicateScale : MonoBehaviour
{
    public float duplicateScalePlus = 0.5f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().duplicateScaleK += duplicateScalePlus;
    }
}
