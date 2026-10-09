
using UnityEngine;

public class BodySkillFireballShot : MonoBehaviour
{
    public float fireballShotCouldown = 5f;
    public float fireballShotCouldownMinus = 1f;
    public int fireballShotDamageMin = 10;
    public int fireballShotDamageMax = 20;
    public int fireballShotBackForce = 250;
    public float fireballShotScale = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject body = GetComponent<BodyAllSkill>().body;

        if(body.GetComponent<Body>().fireballShotLevel == 0)
        {
            body.GetComponent<Body>().fireballShotScale = fireballShotScale;
            body.GetComponent<Body>().fireballShotDamageMin = fireballShotDamageMin;
            body.GetComponent<Body>().fireballShotDamageMax = fireballShotDamageMax;
            body.GetComponent<Body>().fireballShotBackForce = fireballShotBackForce;
            body.GetComponent<Body>().fireballShotCouldown = fireballShotCouldown;
        }
        else
        {
            int levelNum = body.GetComponent<Body>().fireballShotLevel % 4;

            switch (levelNum)
            {
                case 0:
                    body.GetComponent<Body>().fireballShotBackForce += fireballShotBackForce;
                    break;

                case 1:
                    body.GetComponent<Body>().fireballShotDamageMin += fireballShotDamageMin;
                    body.GetComponent<Body>().fireballShotDamageMax += fireballShotDamageMax;
                    break;

                case 2:
                    body.GetComponent<Body>().fireballShotScale += fireballShotScale;
                    break;

                case 3:
                    body.GetComponent<Body>().fireballShotCouldown -= fireballShotCouldownMinus;
                    break;
            }
        }

        body.GetComponent<Body>().fireballShotLevel++;
        Destroy(gameObject);
    }

}
