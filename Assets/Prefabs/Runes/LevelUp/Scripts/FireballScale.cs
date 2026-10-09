
using UnityEngine;

public class FireballScale : MonoBehaviour
{
    public float fireballScalePlus = 0.5f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().fireballScaleK += fireballScalePlus;
    }
}
