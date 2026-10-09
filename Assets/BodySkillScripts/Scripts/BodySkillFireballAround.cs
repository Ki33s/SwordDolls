
using UnityEngine;

public class BodySkillFireballAround : MonoBehaviour
{
    public float fireballCouldown = 5f;
    public float fireballCouldownMinus = 1f;
    public int fireballDamageMin = 10;
    public int fireballDamageMax = 20;
    public int fireballBackForce = 250;
    public float fireballAroundScale = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject body = GetComponent<BodyAllSkill>().body;

        if (body.GetComponent<Body>().fireballAroundLevel == 0)
        {
            body.GetComponent<Body>().fireballAroundScale = fireballAroundScale;
            body.GetComponent<Body>().fireballMinDamage = fireballDamageMin;
            body.GetComponent<Body>().fireballMaxDamage = fireballDamageMax;
            body.GetComponent<Body>().fireballBackForce = fireballBackForce;
            body.GetComponent<Body>().fireballCouldown = fireballCouldown;
        }
        else
        {
            int levelNum = body.GetComponent<Body>().fireballAroundLevel % 4;

            switch (levelNum)
            {
                case 0:
                    body.GetComponent<Body>().maxFireballs++;
                    break;

                case 1:
                    body.GetComponent<Body>().fireballMinDamage += fireballDamageMin;
                    body.GetComponent<Body>().fireballMaxDamage += fireballDamageMax;
                    body.GetComponent<Body>().fireballBackForce += fireballBackForce;
                    break;

                case 2:
                    body.GetComponent<Body>().fireballAroundScale += fireballAroundScale;
                    break;

                case 3:
                    body.GetComponent<Body>().fireballCouldown -= fireballCouldownMinus;
                    break;
            }
        }

        body.GetComponent<Body>().fireballAroundLevel ++;
        Destroy(gameObject);
    }
}
