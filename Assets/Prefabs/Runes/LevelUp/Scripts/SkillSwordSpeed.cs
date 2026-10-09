using UnityEngine;

public class SkillSwordSpeed : MonoBehaviour
{
    public float speedPlusK = 0.2f;
    public int forcePlus = 20;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().maxSpeed += speedPlusK;
        playerSword.GetComponent<Sword>().forceMove += forcePlus;
        playerSword.GetComponent<Sword>().RefreshMovementPower();
    }
}
