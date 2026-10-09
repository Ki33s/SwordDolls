
using UnityEngine;

public class SkillSpeed : MonoBehaviour
{
    public float speedMovePlusK = 0.2f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().head.transform.root.GetComponent<Body>().bodySpeed += playerSword.GetComponent<Sword>().head.transform.root.GetComponent<Body>().bodySpeed * speedMovePlusK;
        playerSword.GetComponent<Sword>().head.transform.root.GetComponent<MainHealth>().SetMaxSpeed();
    }
}
