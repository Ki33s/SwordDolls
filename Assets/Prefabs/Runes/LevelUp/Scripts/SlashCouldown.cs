
using UnityEngine;

public class SlashCouldown : MonoBehaviour
{
    public float slashCouldownK = 0.25f;
    private GameObject playerSword;

    private void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().slashCouldown -= playerSword.GetComponent<Sword>().slashCouldown * slashCouldownK;
    }
}
