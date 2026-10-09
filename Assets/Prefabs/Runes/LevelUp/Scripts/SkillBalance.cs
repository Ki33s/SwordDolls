
using UnityEngine;

public class SkillBalance : MonoBehaviour
{
    public float damagePlusPerc = 0.025f;
    public int forcePlus = 25;
    public float forceResist = 0.025f;
    public float speedPlusK = 0.05f;
    public int forceSpeedPlus = 5;
    public float sizePlusK = 0.025f;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        if(damagePlusPerc > 0)
        {
            int minDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().minDamage * damagePlusPerc);
            int maxDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().maxDamage * damagePlusPerc);

            playerSword.GetComponent<ImpactTrigger>().minDamage += minDamagePlus;
            playerSword.GetComponent<ImpactTrigger>().maxDamage += maxDamagePlus;
        }
        
        if(forcePlus > 0)
        {
            playerSword.GetComponent<Impact>().forceBack += forcePlus;
            playerSword.GetComponent<ImpactTrigger>().forceK -= forceResist;
        }
        

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
        {
            if(damagePlusPerc > 0)
            {
                int tipminDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().tipminDamage * damagePlusPerc);
                int tipmaxDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().tipmaxDamage * damagePlusPerc);

                playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().minDamage += tipminDamagePlus;
                playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().maxDamage += tipmaxDamagePlus;
            }
            

            if(forcePlus > 0)
            {
                playerSword.GetComponent<Sword>().tip.GetComponent<Impact>().forceBack += forcePlus;
                playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().forceK -= forceResist;
            }
        }

        if(speedPlusK > 0)
        {
            playerSword.GetComponent<Sword>().maxSpeed += speedPlusK;
            playerSword.GetComponent<Sword>().forceMove += forceSpeedPlus;
        }
        
        if(sizePlusK > 0)
        {
            playerSword.transform.localScale = new Vector3(playerSword.transform.localScale.x + sizePlusK, playerSword.transform.localScale.y + sizePlusK, playerSword.transform.localScale.z + sizePlusK);
        }
        
    }
}
