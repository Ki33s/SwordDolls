using UnityEngine;

public class Body : MonoBehaviour
{
    [Header("Physical walking")]
    public bool physicalWalking = true;
    public float walkStepTime = .24f;
    public float runStepTime = .14f;
    public float footLift = .18f;
    public float footPrediction = .22f;
    [Range(.25f, 1.4f)] public float footReach = 1.15f;
    [Range(.1f, 1f)] public float footLegMass = .6f;
    public float footSpring = 600;
    public float footDamping = 28;
    public float maxFootAcceleration = 750;
    PhysicalGait gait;
    public int WalkingSteps => gait != null ? gait.Steps : 0;
    public bool obstacle = true;
    public float bodySpeed = 2;
    public float swordSpeedModificator = 1f;
    public int forceBack = 1000;
    public int headForce = 400;
    public int footsGroundForce = 200;
    public int footsMoveForce = 300;
    public int footsUpForce = 300;
    public int forceObstacle = 600;

    public int followSwordForce = 500;
    public float followSwordMaxSpeed = 5;

    public float stepWidth = 0.28f;
    public float stepLenght = 0.5f;
    public float obstacleDistance = 0.3f;

    public int dollId = 1;

    public GameObject rendererModel;

    private float healthK = 1f;
    public bool stunned = false;

    //Timers
    public float footMoveTime = 0.1f;
    public float footCouldownTime = 0.05f;
    private float footRMovingTimer = 0;
    private float footLMovingTimer = 0;

    //ObstacleTimers
    public float obstacleJumpTime = 0.5f;
    private float obstacleJumpTimer = 0;

    public float obstacleJumpCouldown = 1f;
    private float obstacleJumpNowCouldown = 0f;

    //Skills
    public int fireballAroundLevel = 0;
    public GameObject fireballAroundGo;
    public int maxFireballs = 1;
    public float fireballAroundScale = 0.5f;
    public float fireballCouldown = 5f;
    public int fireballBackForce = 500;
    public int fireballMinDamage = 5;
    public int fireballMaxDamage = 10;
    private float fireballTimer = 0f;
    public int fireballsCount = 0;

    //FireballShot
    public GameObject fireball;
    public int fireballShotLevel = 0;
    public float fireballShotScale = 0.5f;
    public int fireballShotDamageMin = 10;
    public int fireballShotDamageMax = 20;
    public int fireballShotBackForce = 500;
    public float fireballShotCouldown = 5f;
    private float fireballShotTimer = 0;



    private float footRNewMovingTimer = 0;
    private float footLNewMovingTimer = 0;

    public GameObject head;
    private Rigidbody rigHead;
    public GameObject footL;
    private Rigidbody rigFootL;
    public GameObject footR;
    private Rigidbody rigFootR;
    public GameObject upperLegL;
    public GameObject upperLegR;
    public GameObject lowerLegL;
    public GameObject lowerLegR;
    public GameObject handlerR;
    private Rigidbody rigHandlerR;
    public GameObject handlerL;
    private Rigidbody rigHandlerL;

    public GameObject leftSword;
    private FixedJoint leftSwordJoint;

    private GameObject leftSwordTarget = null;
    public float leftSwordFindMagnitude = 2f;
    public float leftSwordFindCouldown = 1f;
    public float leftSwordFindTimer = 0f;

    public GameObject sword;
    readonly System.Collections.Generic.List<Collider[]> gripContacts = new System.Collections.Generic.List<Collider[]>();
    private GameObject targetSword;
    private bool followSword = false;
    public bool bashedSword = false;
    private float bashedSwordTimer = 0;
    public bool leftHandEquipeed = false;
    public bool stay = false;

    public float equipMagnitude = 0.05f;
    public float equipLeftMagnitude = 0.3f;

    public int forceHips = 500;

    private Vector3 pushedDir;
    private bool pushed = false;
    private bool rightFootPooshed = false;

    // Start is called before the first frame update
    void Start()
    {
        rigHead = head.GetComponent<Rigidbody>();
        rigFootL = footL.GetComponent<Rigidbody>();
        rigFootR = footR.GetComponent<Rigidbody>();
        rigHandlerR = handlerR.GetComponent<Rigidbody>();
        rigHandlerL = handlerL.GetComponent<Rigidbody>();

        SetDollId(transform);
    }
    public void SetDollId(Transform goTransform)
    {

        foreach (Transform child in goTransform)
        {
            if (child.GetComponent<Impact>())
            {
                if (child.GetComponent<ImpactTrigger>())
                {
                    child.GetComponent<ImpactTrigger>().dollId = dollId;
                }
                else
                {
                    child.GetComponent<Impact>().dollId = dollId;
                    child.GetComponent<Impact>().forceBack = forceBack;
                }
            }

            if (child.childCount > 0)
                SetDollId(child);
        }
    }

    public void MoveInBody(GameObject newSword)
    {
        if(sword)
            sword.GetComponent<Sword>().DropSword();
        dollId = newSword.GetComponent<ImpactTrigger>().dollId;
        SetDollId(transform);
        BeginGrip(newSword);
        targetSword = newSword;
        followSword = true;

        upperLegR.layer = 3;
        upperLegL.layer = 3;
        lowerLegL.layer = 3;
        lowerLegR.layer = 3;
        footL.layer = 3;
        footR.layer = 3;

        if(newSword.GetComponent<Sword>().twoHanded && leftSword)
        {
            DropLeft();
        }

        CheckBodySkills();
    }

    public void FootPushed(Vector3 dir, Vector3 hitPos)
    {
        if((footL.transform.position - hitPos).magnitude > (footR.transform.position - hitPos).magnitude)
        {
            footLMovingTimer = footMoveTime;
            footLNewMovingTimer = footCouldownTime;
            rightFootPooshed = false;
        }
        else
        {
            footRMovingTimer = footMoveTime;
            footRNewMovingTimer = footCouldownTime;
            rightFootPooshed = true;
        }

        gait?.Release(.16f);
        pushed = true;
        pushedDir = dir;
    }

    public void CheckBodySkills()
    {
        if(fireballAroundLevel > 0)
        {
            fireballMinDamage *= 2;
            fireballMaxDamage *= 2;
        }

        if(fireballShotLevel > 0)
        {
            fireballShotDamageMin *= 2;
            fireballShotDamageMax *= 2;
        }
    }

    public void EquipLeft(GameObject newLeftSword)
    {
        leftSword = newLeftSword;
        leftSwordJoint = handlerL.AddComponent<FixedJoint>();
        leftSwordJoint.connectedBody = newLeftSword.GetComponent<Rigidbody>();
        newLeftSword.GetComponent<ImpactTrigger>().hasDoll = true;
        leftHandEquipeed = true;
    }

    public void DropLeft()
    {
        if(!leftSword){leftHandEquipeed=false;return;}
        leftSword.transform.parent = null;
        Destroy(leftSwordJoint);

        //Destroy(leftSword.GetComponent<Impact>());
        //Destroy(leftSword.GetComponent<ImpactTrigger>());
        leftSword.GetComponent<ImpactTrigger>().isEnabled = false;
        leftSword.GetComponent<Weapon>().dropped = true;

        if (leftSword.GetComponent<Weapon>().tip)
        {
            leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>().isEnabled = false;
        }

        leftSword.GetComponent<Rigidbody>().velocity /= 2;

        ChangeSwordLayer(leftSword.transform, 0);
        leftSword=null;leftHandEquipeed=false;
    }

    void ChangeSwordLayer(Transform go, int newLayer)
    {
        go.gameObject.layer = newLayer;

        for (int i = 0; i < go.childCount; i++)
        {
            ChangeSwordLayer(go.GetChild(i), newLayer);
        }
    }


    public void UpdateHealth(float newK)
    {
        healthK = newK;
        if(sword && sword.GetComponent<Sword>())sword.GetComponent<Sword>().healthK = newK;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!stunned)
        {
            HoldHead();
            if (physicalWalking && !followSword && !bashedSword)
            {
                if (gait == null) gait = new PhysicalGait(this);
                gait.Tick(healthK, stay);
            }
            else
            {
                FootsGround();
                if (!stay) MoveFoots();
            }
                

            if (followSword)
                FollowSword();

            if (bashedSword)
            {
                if (bashedSwordTimer > 0)
                {
                    bashedSwordTimer -= Time.deltaTime;
                }
                else
                {
                    BashedSword();
                }
            }
            else if(sword)
            {
                if(sword.GetComponent<Sword>())
                    if (!sword.GetComponent<Sword>().twoHanded && !leftSword)
                        FindSecondWeapon();
            }


            CheckSkills();
        }
        
    }

    void CheckSkills()
    {
        if(fireballAroundLevel > 0)
        {
            
                if (fireballTimer <= 0)
                {
                    if (fireballsCount < maxFireballs)
                    {
                    fireballsCount++;
                    GameObject nowFireball = Instantiate(fireballAroundGo, head.transform.position, Quaternion.Euler(0, 0, head.transform.eulerAngles.z));
                    nowFireball.GetComponent<FireballAround>().followTarget = head;
                    nowFireball.GetComponent<FireballAround>().swordSkill = false;
                    nowFireball.GetComponent<FireballAround>().forceBack = fireballBackForce;
                    nowFireball.GetComponent<FireballAround>().dollId = dollId;
                    nowFireball.GetComponent<FireballAround>().minDamage = fireballMinDamage;
                    nowFireball.GetComponent<FireballAround>().maxDamage = fireballMaxDamage;

                    fireballTimer = fireballCouldown;
                    }
                   
                }
                else
                {
                    fireballTimer -= Time.deltaTime;
                }
            
        }

        if(fireballShotLevel > 0)
        {
            if(fireballShotTimer <= 0)
            {
                GameObject nowFireballShot = Instantiate(fireball, head.transform.position, Quaternion.Euler(90, Random.Range(0,360), 0));

                nowFireballShot.GetComponent<BulletTrigger>().forceBack = fireballShotBackForce;
                nowFireballShot.GetComponent<BulletTrigger>().dollId = dollId;
                nowFireballShot.GetComponent<BulletTrigger>().minDamage = fireballShotDamageMin;
                nowFireballShot.GetComponent<BulletTrigger>().maxDamage = fireballShotDamageMax;

                nowFireballShot.AddComponent<Bullet>();

                fireballShotTimer = fireballShotCouldown;
            }
            else
            {
                fireballShotTimer -= Time.deltaTime;
            }
        }
    }

    

    void HoldHead()
    {
            rigHead.AddForce(new Vector3(0, 100, 0) * headForce * healthK * Time.deltaTime);
    }

    bool IsObstacle(GameObject go, Vector3 dir)
    {
        bool obstacle = false;

        
        RaycastHit hit;
        if (Physics.Raycast(go.transform.position, dir, out hit, obstacleDistance))
        {
            if (!hit.collider.isTrigger && !hit.transform.GetComponent<Health>())
            {
                if(hit.transform.GetComponent<Impact>())
                {
                    if(hit.transform.tag != "Wall")
                        obstacle = true;
                }
            }
                
            /*if (hit.transform.tag != "Wall")
                if (!hit.transform.GetComponent<Health>() && hit.transform.GetComponent<Impact>())
                {
                    if (hit.transform.GetComponent<Impact>().dollId != dollId)
                        obstacle = true;
                }
                else
                    obstacle = true;*/
        }

        return obstacle;
    }

    void FootsGround()
    {
            rigFootL.AddForce(new Vector3(0, -100, 0) * footsGroundForce * healthK * Time.deltaTime);

            rigFootR.AddForce(new Vector3(0, -100, 0) * footsGroundForce * healthK * Time.deltaTime);
    }

    void MoveFoots()
    {
        Vector2 footLPos = new Vector2(footL.transform.position.x, footL.transform.position.z);
        Vector2 footRPos = new Vector2(footR.transform.position.x, footR.transform.position.z);

        Vector3 velNorm = rigHandlerR.velocity.normalized;
        Vector3 velocity = new Vector3(velNorm.x, 0, velNorm.z);
        velocity = Quaternion.Euler(0, 90, 0) * velocity;

        Vector3 footTarget = new Vector3(handlerR.transform.position.x, 0, handlerR.transform.position.z);

        Vector3 footRTarget = footTarget + velocity * stepWidth;
        Vector3 footLTarget = footTarget - velocity * stepWidth;

        Vector2 footRTargetV2 = new Vector2(footRTarget.x, footRTarget.z);
        Vector2 footLTargetV2 = new Vector2(footLTarget.x, footLTarget.z);

        if (footRMovingTimer > 0)
        {

            footRMovingTimer -= Time.deltaTime;

            if (footRNewMovingTimer > 0)
                footRNewMovingTimer -= Time.deltaTime;

            Vector2 targetDirR = (footRTargetV2 - footRPos).normalized * footsMoveForce * healthK;

            if (pushed && rightFootPooshed)
                targetDirR = pushedDir;

            if (obstacle && IsObstacle(footR, new Vector3(targetDirR.x, 0, targetDirR.y)))
            {
                rigFootR.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                //rigUpperLegR.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                rigHead.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
            }
            else
            {
                rigFootR.AddForce(new Vector3(targetDirR.x, footsUpForce, targetDirR.y) * 100 * Time.deltaTime);
            }
        }

        if (footLMovingTimer > 0)
        {
            footLMovingTimer -= Time.deltaTime;

            if (footLNewMovingTimer > 0)
                footLNewMovingTimer -= Time.deltaTime;

            Vector2 targetDirL = (footLTargetV2 - footLPos).normalized * footsMoveForce * healthK;

            if (pushed && !rightFootPooshed)
                targetDirL = pushedDir;

            if (obstacle && IsObstacle(footL, new Vector3(targetDirL.x, 0, targetDirL.y)))
            {
                rigFootL.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                //rigUpperLegL.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                rigHead.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
            }
            else
            {
                rigFootL.AddForce(new Vector3(targetDirL.x, footsUpForce, targetDirL.y) * 100 * Time.deltaTime);
            }
        }

        float magnitudeFootL = (footLTargetV2 - footLPos).magnitude;
        float magnitudeFootR = (footRTargetV2 - footRPos).magnitude;

        if (magnitudeFootL > stepLenght || magnitudeFootR > stepLenght)
        {
            if (magnitudeFootL > magnitudeFootR)
            {
                if (footRNewMovingTimer <= 0)
                {
                    footLMovingTimer = footMoveTime;
                    footLNewMovingTimer = footCouldownTime;

                    if (pushed)
                        pushed = false;
                }
            }
            else
            {
                if (footLNewMovingTimer <= 0)
                {
                    footRMovingTimer = footMoveTime;
                    footRNewMovingTimer = footCouldownTime;

                    if (pushed)
                        pushed = false;
                }
            }
        }
    }

    /*void MoveFoots()
    {
        Vector2 footLPos = new Vector2(footL.transform.position.x, footL.transform.position.z);
        Vector2 footRPos = new Vector2(footR.transform.position.x, footR.transform.position.z);

        Vector3 velNorm = rigHandlerR.velocity.normalized;
        Vector3 velocity = new Vector3(velNorm.x, 0, velNorm.z);
        velocity = Quaternion.Euler(0, 90, 0) * velocity;

        Vector3 footTarget = new Vector3(handlerR.transform.position.x, 0, handlerR.transform.position.z);

        Vector3 footRTarget = footTarget + velocity * stepWidth;
        Vector3 footLTarget = footTarget - velocity * stepWidth;

        Vector2 footRTargetV2 = new Vector2(footRTarget.x, footRTarget.z);
        Vector2 footLTargetV2 = new Vector2(footLTarget.x, footLTarget.z);

        //CheckJumpCouldowns

        if (obstacleJumpNowCouldown <= 0)
        {
            if(obstacleJumpTimer > 0)
            {
                obstacleJumpTimer -= Time.deltaTime;
            }
            else
            {
                obstacleJumpNowCouldown = obstacleJumpCouldown;
            }
        }
        else
        {
            obstacleJumpNowCouldown -= Time.deltaTime;
        }

        if (footRMovingTimer > 0)
        {

            footRMovingTimer -= Time.deltaTime;

            if (footRNewMovingTimer > 0)
                footRNewMovingTimer -= Time.deltaTime;

            Vector2 targetDirR = (footRTargetV2 - footRPos).normalized * footsMoveForce * healthK;

            if (pushed && rightFootPooshed)
                targetDirR = pushedDir;

            bool jumpR = false;

            if(obstacleJumpNowCouldown <= 0)
            {
                if (obstacle && IsObstacle(footR, new Vector3(targetDirR.x, 0, targetDirR.y)))
                {
                    jumpR = true;

                    if(obstacleJumpTimer <= 0)
                    {
                        obstacleJumpTimer = obstacleJumpTime;
                    }
                }
            }

            if (jumpR)
            {
                rigFootR.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                //rigUpperLegR.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                rigHead.AddForce(new Vector3(0, forceObstacle, 0) * 50 * healthK * Time.deltaTime);
            }
            else
            {
                rigFootR.AddForce(new Vector3(targetDirR.x, footsUpForce, targetDirR.y) * 100 * Time.deltaTime);
            }
        }

        if (footLMovingTimer > 0)
        {
            footLMovingTimer -= Time.deltaTime;

            if (footLNewMovingTimer > 0)
                footLNewMovingTimer -= Time.deltaTime;

            Vector2 targetDirL = (footLTargetV2 - footLPos).normalized * footsMoveForce * healthK;

            if (pushed && !rightFootPooshed)
                targetDirL = pushedDir;

            bool jumpL = false;

            if (obstacleJumpNowCouldown <= 0)
            {
                if (obstacle && IsObstacle(footL, new Vector3(targetDirL.x, 0, targetDirL.y)))
                {
                    jumpL = true;

                    if (obstacleJumpTimer <= 0)
                    {
                        obstacleJumpTimer = obstacleJumpTime;
                    }
                }
            }

            if (jumpL)
            {
                rigFootL.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                //rigUpperLegL.AddForce(new Vector3(0, forceObstacle, 0) * 100 * healthK * Time.deltaTime);
                rigHead.AddForce(new Vector3(0, forceObstacle, 0) * 50 * healthK * Time.deltaTime);
            }
            else
            {
                rigFootL.AddForce(new Vector3(targetDirL.x, footsUpForce, targetDirL.y) * 100 * Time.deltaTime);
            }
        }

        float magnitudeFootL = (footLTargetV2 - footLPos).magnitude;
        float magnitudeFootR = (footRTargetV2 - footRPos).magnitude;

        if (magnitudeFootL > stepLenght || magnitudeFootR > stepLenght)
        {
            if (magnitudeFootL > magnitudeFootR)
            {
                if (footRNewMovingTimer <= 0)
                {
                    footLMovingTimer = footMoveTime;
                    footLNewMovingTimer = footCouldownTime;

                    if (pushed)
                        pushed = false;
                }
                    
            }
            else
            {
                if (footLNewMovingTimer <= 0)
                {
                    footRMovingTimer = footMoveTime;
                    footRNewMovingTimer = footCouldownTime;

                    if (pushed)
                        pushed = false;
                }
                    
            }
        }


    }*/

    void FindSecondWeapon()
    {
        if(leftSwordTarget == null)
        {
            if(leftSwordFindTimer <= 0)
            {
                leftSwordFindTimer = leftSwordFindCouldown;

                float minDistance = leftSwordFindMagnitude;

                Collider[] hitColliders = Physics.OverlapSphere(handlerL.transform.position, leftSwordFindMagnitude);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.GetComponent<Weapon>())
                    {
                        if (!hitCollider.GetComponent<Weapon>().Twohanded && hitCollider.GetComponent<Weapon>().dropped)
                        {
                            float distance = (hitCollider.transform.position - handlerL.transform.position).magnitude;

                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                leftSwordTarget = hitCollider.gameObject;
                            }
                        }
                    }
                }
            }
            else
            {
                leftSwordFindTimer -= Time.deltaTime;
            }

           
        }
        else
        {
            Vector3 targetDir = leftSwordTarget.transform.position - handlerL.transform.position;

            if (targetDir.magnitude > leftSwordFindMagnitude)
            {
                leftSwordTarget = null;
            }
            else
            {
                if(targetDir.magnitude > equipLeftMagnitude)
                {
                    rigHandlerL.AddForce(targetDir.normalized * 100 * followSwordForce * Time.deltaTime);
                }
                else
                {
                    EquipLeftHand();
                }
            }
        }
    }

    void EquipLeftHand()
    {
        leftSword = leftSwordTarget;
        leftSword.GetComponent<Weapon>().dropped = false;
        leftSword.transform.position = handlerL.transform.position;
        leftSword.transform.rotation = handlerL.transform.rotation;
        leftSword.transform.parent = handlerL.transform;
        leftSwordJoint = handlerL.AddComponent<FixedJoint>();
        leftSwordJoint.connectedBody = leftSword.GetComponent<Rigidbody>();
        leftHandEquipeed = true;

        leftSword.GetComponent<ImpactTrigger>().isEnabled = true;
        leftSword.GetComponent<ImpactTrigger>().hasDoll = true;
        leftSword.GetComponent<ImpactTrigger>().dollId = dollId;
        leftSword.GetComponent<Impact>().dollId = dollId;


        if(sword && sword.GetComponent<Sword>() && sword.GetComponent<Sword>().player)
        {
            leftSword.GetComponent<ImpactTrigger>().minDamage *= 2;
            leftSword.GetComponent<ImpactTrigger>().maxDamage *= 2;
        }
        
        if (leftSword.GetComponent<Weapon>())
        {
            if (leftSword.GetComponent<Weapon>().tip)
            {
                if (leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>()) leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>().dollId = dollId;
                if (leftSword.GetComponent<Weapon>().tip.GetComponent<Impact>()) leftSword.GetComponent<Weapon>().tip.GetComponent<Impact>().dollId = dollId;

                if(sword && sword.GetComponent<Sword>() && sword.GetComponent<Sword>().player)
                {
                    leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>().minDamage *= 2;
                    leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>().maxDamage *= 2;
                }
                
            }
        }

        ChangeSwordLayer(leftSword.transform, 7);
    }

    // Avoid the blade colliding with its new owner's hand while the grip joint is being formed.
    void BeginGrip(GameObject weapon)
    {
        EndGrip();
        foreach(var part in GetComponentsInChildren<Collider>())
        foreach(var blade in weapon.GetComponentsInChildren<Collider>())
        {
            if(part == blade || Physics.GetIgnoreCollision(part, blade)) continue;
            Physics.IgnoreCollision(part, blade, true);
            gripContacts.Add(new[]{part, blade});
        }
    }
    void EndGrip()
    {
        foreach(var pair in gripContacts) if(pair[0] && pair[1]) Physics.IgnoreCollision(pair[0], pair[1], false);
        gripContacts.Clear();
    }
    void OnDestroy(){EndGrip();}
    void FollowSword()
    {
        
        if (!targetSword) { followSword = false; bashedSword = false; return; }
        Vector3 forceVector = targetSword.transform.position - handlerR.transform.position;

        if(forceVector.magnitude > equipMagnitude)
            rigHandlerR.AddForce(forceVector.normalized * 100 * followSwordForce * Time.deltaTime);
        else
        {
            followSword = false;
            targetSword.transform.position = handlerR.transform.position;
            targetSword.transform.rotation = handlerR.transform.rotation;
            targetSword.transform.parent = handlerR.transform;
            targetSword.GetComponent<Sword>().Equip();
            EndGrip();
        }
    }

    void BashedSword()
    {

        if (!targetSword) { followSword = false; bashedSword = false; return; }
        Vector3 forceVector = targetSword.transform.position - handlerR.transform.position;

        if (forceVector.magnitude > equipMagnitude)
        {
            if (forceVector.magnitude > 2)
            {
                forceVector = new Vector3(forceVector.x, 0, forceVector.z).normalized;
            }

            rigHandlerR.AddForce(forceVector.normalized * 100 * followSwordForce * Time.deltaTime);
        }    
        else
        {
            bashedSword = false;
            targetSword.transform.position = handlerR.transform.position;
            targetSword.transform.rotation = handlerR.transform.rotation;
            targetSword.transform.parent = handlerR.transform;
            targetSword.GetComponent<Sword>().GetUpSword();
        }
    }

    public void BashSword(float bashTime)
    {
        targetSword = sword;
        bashedSwordTimer = bashTime;
        bashedSword = true;
    }
}
