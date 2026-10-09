
using UnityEngine;

public class SlashDamage : MonoBehaviour
{
    public float slashDamageKPlus = 0.25f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().slashDamageK += slashDamageKPlus;
    }
}
