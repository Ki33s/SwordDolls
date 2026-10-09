
using UnityEngine;

public class FireballAround : MonoBehaviour
{
    public GameObject fireball;
    public int rotateSpeed = 180;
    public float rotateRadius = 2f;
    public float maxRadiusTime = 1f;
    public int forceBack = 0;
    public int minDamage = 5;
    public int maxDamage = 10;
    public int tipforceBack = 0;
    public int tipminDamage = 5;
    public int tipmaxDamage = 10;
    public float duplicateScaleK = 0.25f;
    public int dollId = 0;
    public bool body = true;
    public int impactType = 0;
    public bool swordSkill = false;
    public bool rotateAroundAdd = false;

    public int fireballType = 0;

    private GameObject nowFireball;
    public GameObject followTarget;
    private float nowRotateRadius = 0f;
    private float maxRadiusTimer = 0f;
    private bool maxRadiusReached = false;
    // Start is called before the first frame update
    void Start()
    {
        if(!followTarget || !fireball){Destroy(gameObject);return;}
        nowFireball = Instantiate(fireball, transform.position, followTarget.transform.rotation, transform);
        nowFireball.transform.localScale = new Vector3(duplicateScaleK, duplicateScaleK, duplicateScaleK);

         if (rotateAroundAdd)
            nowFireball.AddComponent<RotationRigth>();

        nowFireball.GetComponent<BulletTrigger>().forceBack = forceBack;
        nowFireball.GetComponent<BulletTrigger>().dollId = dollId;
        nowFireball.GetComponent<BulletTrigger>().minDamage = minDamage;
        nowFireball.GetComponent<BulletTrigger>().maxDamage = maxDamage;
        nowFireball.GetComponent<BulletTrigger>().impactType = impactType;

        if (nowFireball.GetComponent<Weapon>())
        {
            if (nowFireball.GetComponent<Weapon>().tip)
            {
                nowFireball.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().minDamage = tipminDamage;
                nowFireball.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().maxDamage = tipmaxDamage;
                nowFireball.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().forceBack = tipforceBack;

            }

        }

        //nowFireball.transform.localPosition = new Vector3(0, 0, nowFireball.transform.localPosition.z + rotateRadius);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!followTarget){Destroy(gameObject);return;}
        if(nowFireball)
        {
            if(followTarget)
            {
                transform.position = followTarget.transform.position;
                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
            }
           

            if (!maxRadiusReached)
            {
                if (maxRadiusTimer < maxRadiusTime)
                {
                    maxRadiusTimer += Time.deltaTime;
                    nowRotateRadius = rotateRadius * maxRadiusTimer / maxRadiusTime;
                    nowFireball.transform.localPosition = new Vector3(0, 0, nowRotateRadius);
                }
                else
                {
                    nowFireball.transform.localPosition = new Vector3(0, 0, rotateRadius);
                    maxRadiusReached = true;
                }
            }
        }
        else
        {
            if(!swordSkill)
            {
                if (body)
                {
                    if (followTarget.transform.root.GetComponent<Body>())
                        followTarget.transform.root.GetComponent<Body>().fireballsCount--;
                }
            }
            else
            {
                switch(fireballType)
                {
                    case 0:
                        followTarget.GetComponent<Sword>().duplicatesAroundCount--;
                        break;

                    case 1:
                        followTarget.GetComponent<Sword>().fireballAroundCount--;
                        break;
                }
                
            }
            
            Destroy(gameObject);
        }
    }
}
