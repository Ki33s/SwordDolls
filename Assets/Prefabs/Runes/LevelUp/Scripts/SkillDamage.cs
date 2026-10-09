
using UnityEngine;

public class SkillDamage : MonoBehaviour
{
    public float damagePlusPerc = 0.2f;
    private GameObject playerSword;
    
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        int minDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().minDamage * damagePlusPerc);
        int maxDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().maxDamage * damagePlusPerc);

        playerSword.GetComponent<ImpactTrigger>().minDamage += minDamagePlus;
        playerSword.GetComponent<ImpactTrigger>().maxDamage += maxDamagePlus;

        if(playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
        {
            int tipminDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().tipminDamage * damagePlusPerc);
            int tipmaxDamagePlus = Mathf.RoundToInt(playerSword.GetComponent<SwordHero>().tipmaxDamage * damagePlusPerc);

            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().minDamage += tipminDamagePlus;
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().maxDamage += tipmaxDamagePlus;
        }
    }
}
