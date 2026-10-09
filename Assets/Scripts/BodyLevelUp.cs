
using UnityEngine;

public class BodyLevelUp : MonoBehaviour
{
    public int endurance;
    public int healthPlus = 10;
    public int agility;
    public float moveSpeedPlus = 0.1f;
    public int strength;
    public int healthStrengthPlus = 5;
    public int forceBackPlus = 50;
    public float forceKPlus = -0.025f;

    public GameObject[] startSkills;

    private int bodyLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Endurance level up
        for(int i = 0; i< endurance; i++)
        {
            GetComponent<MainHealth>().maxHealth += healthPlus;
            GetComponent<MainHealth>().health += healthPlus;
            bodyLevel++;
        }

        //Agility level up
        for(int i = 0; i < agility; i++)
        {
            GetComponent<Body>().bodySpeed += moveSpeedPlus;
            GetComponent<MainHealth>().SetMaxSpeed();
            bodyLevel++;
        }

        //Stregth level up
        for(int i = 0; i<strength;i++)
        {
            GetComponent<MainHealth>().maxHealth += healthStrengthPlus;
            GetComponent<MainHealth>().health += healthStrengthPlus;
            GetComponent<MainHealth>().forceK += forceKPlus;
            GetComponent<Body>().forceBack += forceBackPlus;
            GetComponent<Body>().SetDollId(transform);
            bodyLevel++;
        }

        //Other skills
        for(int i = 0; i < startSkills.Length; i++)
        {
            GameObject nowStartSkill = Instantiate(startSkills[i], transform.position, Quaternion.identity);
            nowStartSkill.GetComponent<BodyAllSkill>().body = gameObject;
            bodyLevel+=3;
        }

        GetComponent<MainHealth>().bodyLevel = bodyLevel;
        GetComponent<MainHealth>().nowHealthBar.GetComponent<HealthBar>().level.text = bodyLevel.ToString();
    }
}
