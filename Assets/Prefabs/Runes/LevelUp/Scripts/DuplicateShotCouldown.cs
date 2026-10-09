
using UnityEngine;

public class DuplicateShotCouldown : MonoBehaviour
{
    public float duplicateShotCouldownK = 0.25f;
    private GameObject playerSword;

    private void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().duplicateShotCouldown -= playerSword.GetComponent<Sword>().duplicateShotCouldown * duplicateShotCouldownK;
    }
}
