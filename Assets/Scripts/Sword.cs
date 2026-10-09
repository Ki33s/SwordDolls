using UnityEngine;

public class Sword : MonoBehaviour
{
    public int forceMove = 500;
    public int forceHead = 800;
    public int forceAnvill = 200;
    public float maxSpeed = 5;
    public float forceHold = 500;
    private float swordHoldY = 1.5f;
    public float headUpMagnitude = 0.3f;

    public float leftHandDeltaY = 0f;
    public float distanceLeftHandEquip = 0.08f;

    public float newLifeHeal = 0.2f;

    public float newLifeTime = 1f;
    public float newLifeTimer = 0f;

    public bool twoHanded = false;
    public bool player = false;
    public bool startEquipped = true;
    public bool hasDoll = false;

    public GameObject mainModel;

    public bool stunLearned = false;
    public bool bashLearned = false;
    public bool critLearned = false;

    //Skills max
    public bool slashLearned = false;
    public float slashScale = 1f;
    public float slashDamageK = 0.5f;
    public float slashForceK = 0.5f;

    //skills duplicate
    public bool duplicateLearned = false;
    public float duplicateDamageK = 0.5f;
    public float duplicateScaleK = 0.5f;
    public float duplicateForceK = 0.5f;

    //skills fireball
    public bool fireballLearned = false;
    public float fireballDamageK = 0.5f;
    public float fireballScaleK = 1f;
    public float fireballForceK = 0.5f;
    private GameObject fireball;

    //Slash
    public bool slashShot = false;
    public float slashStartK = 1.2f;
    public float slashCouldown = 5f;
    private float slashTimer = 0;
    public GameObject slashFX;

    //Prick
    public bool prickShot = false;
    public float prickCouldown = 5f;
    private float prickTimer = 0;
    public GameObject prickFX;

    //DuplicateShot
    public bool duplicateShot = false;
    public float duplicateShotCouldown = 5f;
    private float duplicateShotTimer = 0;

    //Skills fireball around
    public bool fireballAround = false;
    public int maxfireballAround = 1;
    public float fireballAroundCouldown = 10f;
    private float fireballAroundTimer = 0f;
    public int fireballAroundCount = 0;

    //fireballShot
    public bool fireballShot = false;
    public float fireballShotCouldown = 5f;
    private float fireballShotTimer = 0;

    //DuplicateClone
    public bool duplicateClone = false;
    public float duplicateCloneCouldown = 10f;
    public float duplicateCloneLifeTime = 5f;
    private float duplicateCloneTime = 0;

    public GameObject tip;
    private Rigidbody rigTip;

    public GameObject weigth;
    public GameObject glowSpotPrefab;
    public GameObject glowSpot;
    private GameObject glowTarget;
    private int glowSpeed = 5;
    private bool movingGlow = false;
    public GameObject newLifeEffect;

    public Vector3 moveVector = Vector3.zero;
    private Rigidbody rig;
    public GameObject head;
    private GameObject handlerR;
    private GameObject handlerL;
    private Rigidbody rigHandlerL;
    private FixedJoint weaponRJoint;
    private FixedJoint weaponLJoint;

    private bool leftHandEquipped = false;

    private GameObject targetHandlerR;
    public bool followBody = false;
    public float healthK = 1f;

    private GameObject nowAnvill;
    public bool evolutionating = false;
    private bool nearAnvill = false;

    private bool levelUp = false;
    public float bodyHealthBonus;
    public void RefreshMovementPower() { withBodyMoveForce = forceMove * (hasDoll && head && head.transform.root.GetComponent<Body>() ? head.transform.root.GetComponent<Body>().swordSpeedModificator : 1); }
    public bool InUpgradeMenu => levelUp || evolutionating;
    public void PrepareAxisEvolution() { nearAnvill = true; }

    private float withBodyMoveForce;
    private float withBodyMaxSpeed;

    //Skills duplicate around
    public GameObject duplicate;
    public GameObject duplicateAroundPrefab;
    public bool duplicateAround = false;
    public int maxduplicateAround = 1;
    public float duplicateAroundCouldown = 10f;
    private float duplicateAroundTimer = 0f;
    public int duplicatesAroundCount = 0;


    //DroppedSword
    private bool swordDropped = false;

    public bool stunned = false;
    public float stunTimer = 0;

    private float anvilTime = 1f;
    private float anvilTimer = 0f;

    private GameObject globalPars;
    private GameObject newLevelUp;

    public float newLevelTimer = 0;

    public GameObject swordBody;
    private GameObject audioListener;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();

        if (tip)
            rigTip = tip.GetComponent<Rigidbody>();
        else
            rigTip = rig;

        withBodyMoveForce = forceMove;
        withBodyMaxSpeed = maxSpeed;

        if (startEquipped)
            Equip();

        glowSpot = glowSpotPrefab ? Instantiate(glowSpotPrefab, transform.position, Quaternion.identity, transform) : new GameObject("Weapon glow anchor");
        if (!glowSpotPrefab) { glowSpot.transform.SetParent(transform, false); glowSpot.transform.localPosition = Vector3.zero; }

        if(hasDoll)
        {
            glowTarget = transform.root.GetComponent<Body>().head;
            movingGlow = true;
        }

        globalPars = GameObject.Find("GlobalParameters");

        swordHoldY = globalPars.GetComponent<GlobalParameters>().swordHoldY;
        fireball = globalPars.GetComponent<GlobalParameters>().fireball;

        if(player)
        {
            audioListener = globalPars.GetComponent<GlobalParameters>().audioListener;
            audioListener.transform.parent = glowSpot.transform;
        }

        //if(swordBody)
            //MoveInBody(transform.root.gameObject);
    }
    public void FirstEffects()
    {
        newLifeTimer = newLifeTime;

        GameObject newLife = Instantiate(newLifeEffect, transform.position, transform.rotation, transform);
        Destroy(newLife, 3);
    }

    public void LevelUp()
    {
        
        if (transform.root.GetComponent<Body>()) transform.root.GetComponent<Body>().stay = true;

        //Destroy(weaponRJoint);

        //if (twoHanded)
            //Destroy(weaponLJoint);

        rig.velocity = Vector3.zero;
        //rig.useGravity = false;
        if(weigth) weigth.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //weigth.GetComponent<Rigidbody>().useGravity = false;

        /*for (int i = 0; i < GetComponents<BoxCollider>().Length; i++)
        {
            if(!GetComponents<BoxCollider>()[i].isTrigger)
            {
                GetComponents<BoxCollider>()[i].enabled = false;
            }
        }

        GetComponent<ImpactTrigger>().isEnabled = false;

        if(tip.GetComponent<ImpactTrigger>())
        {
            for (int i = 0; i < tip.GetComponents<BoxCollider>().Length; i++)
            {
                if (!tip.GetComponents<BoxCollider>()[i].isTrigger)
                {
                    tip.GetComponents<BoxCollider>()[i].enabled = false;
                }
            }

            tip.GetComponent<ImpactTrigger>().isEnabled = false;
        }*/

        levelUp = true;
        nearAnvill = false;
        anvilTimer = anvilTime;

        //glowSpot.transform.parent = null;
        //glowTarget = transform.gameObject;
        //movingGlow = true;

        //newLevelUp = Instantiate(globalPars.GetComponent<GlobalParameters>().levelUpGO, new Vector3(transform.position.x,transform.position.y+1,transform.position.z), Quaternion.identity);
        //newLevelUp.GetComponent<LevelUp>().playerSword = gameObject;

        //Camera.main.GetComponent<CameraMove>().Evolution(newLevelUp.transform.position);

        newLevelUp = Instantiate(globalPars.GetComponent<GlobalParameters>().levelUpGO);
        newLevelUp.transform.SetParent(GameObject.Find("CanvasExp").transform, false);
    }

    public void FinshLevelUp()
    {
        levelUp=false;
        var body=head?head.transform.root.GetComponent<Body>():null;
        if(body)body.stay=false;
        Camera.main.GetComponent<CameraMove>().GetPlayer(gameObject);
        // Preserve hand joints and position. Queued choices open on the next HUD update.
    }
    public void Evolution(GameObject anvill)
    {
        if (transform.root.GetComponent<Body>()) transform.root.GetComponent<Body>().stay = true;

        rig.velocity = Vector3.zero;
        if(weigth) weigth.GetComponent<Rigidbody>().velocity = Vector3.zero;

        nowAnvill = anvill;
        nearAnvill = false;
        evolutionating = true;
        anvilTimer = anvilTime;

        glowSpot.transform.parent = null;
        glowTarget = transform.gameObject;
        movingGlow = true;

        Camera.main.GetComponent<CameraMove>().Evolution(anvill.transform.position);
    }

    public void ChooseEvo(GameObject evo)
    {
        if (evolutionating && nearAnvill)
        {
            Destroy(weaponRJoint);Destroy(weaponLJoint);
            evo.GetComponent<SwordEvo>().anvill.GetComponent<Anvill>().EvoChoosed(evo);
            GameObject newSword = Instantiate(evo.GetComponent<SwordEvo>().newSword, transform.position, transform.rotation);
            Camera.main.GetComponent<CameraMove>().GetPlayer(newSword);
            newSword.GetComponent<SwordHero>().level = GetComponent<SwordHero>().level;
            newSword.GetComponent<SwordHero>().experience = GetComponent<SwordHero>().experience;
            newSword.GetComponent<SwordHero>().levelUpRunes = GetComponent<SwordHero>().levelUpRunes;
            if(globalPars.GetComponent<GlobalParameters>().nowCity.GetComponent<CampaignStage>()) { globalPars.GetComponent<GlobalParameters>().nowCity.GetComponent<City>().newLevelStart=true; newSword.GetComponent<SwordHero>().NewCityLevel(); }

            var carriedBody = head ? head.transform.root.GetComponent<Body>() : null;
            if (carriedBody && (hasDoll || followBody))
            {
                newSword.GetComponent<Sword>().swordBody = carriedBody.gameObject;
                carriedBody.stay = false;
                if (carriedBody.sword == gameObject) carriedBody.sword = null;
                carriedBody.MoveInBody(newSword);
            }
            else
            {

            }

            GetComponent<SwordHero>().MoveSkills(newSword);

            GameObject newLife = Instantiate(newLifeEffect, newSword.transform.position, newSword.transform.rotation, newSword.transform);
            Destroy(newLife, 3);

            Destroy(evo);
            audioListener.transform.parent = null;
            Destroy(glowSpot);
            //GameObject.Find("Dungeon").GetComponent<Dungeon>().GenerateLevel();
            Destroy(gameObject);

        }
    }

    public void MoveInBody(GameObject body)
    {
        targetHandlerR = body.GetComponent<Body>().handlerR;
        glowTarget = body.GetComponent<Body>().head;
        head = body.GetComponent<Body>().head;
        followBody = true;
        glowSpot.transform.parent = null;
        movingGlow = true;

        body.GetComponent<MainHealth>().NewSword();
        var bonuses=body.GetComponent<PlayerBodyBonuses>(); if(!bonuses)bonuses=body.AddComponent<PlayerBodyBonuses>(); bonuses.Apply(bodyHealthBonus);
        var status=body.GetComponent<CombatStatus>();if(status){status.enabled=false;Destroy(status);}
        var health=body.GetComponent<MainHealth>(); health.Heal(1);healthK=1;
        var shield=body.GetComponent<BodyShield>(); if(shield)shield.Refill();

        if (body.GetComponent<Body>().leftHandEquipeed)
        {
            body.GetComponent<Body>().leftSword.GetComponent<ImpactTrigger>().minDamage *= 2;
            body.GetComponent<Body>().leftSword.GetComponent<ImpactTrigger>().maxDamage *= 2;
            body.GetComponent<Body>().leftSword.GetComponent<ImpactTrigger>().dollId = GetComponent<ImpactTrigger>().dollId;
            body.GetComponent<Body>().leftSword.GetComponent<Impact>().dollId = GetComponent<Impact>().dollId;

            if (body.GetComponent<Body>().leftSword.GetComponent<Weapon>())
            {
                if (body.GetComponent<Body>().leftSword.GetComponent<Weapon>().tip)
                {
                    body.GetComponent<Body>().leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>().dollId = GetComponent<ImpactTrigger>().dollId;
                    body.GetComponent<Body>().leftSword.GetComponent<Weapon>().tip.GetComponent<Impact>().dollId = GetComponent<Impact>().dollId;
                    body.GetComponent<Body>().leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>().minDamage *= 2;
                    body.GetComponent<Body>().leftSword.GetComponent<Weapon>().tip.GetComponent<ImpactTrigger>().maxDamage *= 2;
                }
            }
        }

        GetComponent<SwordHero>().UseBodySkills();
    }

    public void Equip()
    {
        head = transform.root.GetComponent<Body>().head;
        handlerR = transform.root.GetComponent<Body>().handlerR;
        handlerL = transform.root.GetComponent<Body>().handlerL;
        transform.root.GetComponent<Body>().sword = gameObject;
        rigHandlerL = handlerL.GetComponent<Rigidbody>();

        transform.position = handlerR.transform.position;

        weaponRJoint = handlerR.AddComponent<FixedJoint>();
        weaponRJoint.connectedBody = rig;
        followBody = false;
        hasDoll = true;
        if (weigth)
            weigth.gameObject.SetActive(false);
        GetComponent<ImpactTrigger>().hasDoll = true;

        withBodyMoveForce = forceMove * transform.root.GetComponent<Body>().swordSpeedModificator;
        withBodyMaxSpeed = maxSpeed * transform.root.GetComponent<Body>().swordSpeedModificator;

        if (tip.GetComponent<ImpactTrigger>())
        {
            tip.GetComponent<ImpactTrigger>().hasDoll = true;
        }
    }

    void JointHands()
    {
        weaponLJoint = handlerL.AddComponent<FixedJoint>();
        weaponLJoint.connectedBody = rig;
    }

    public void DropLeft()
    {
        if (transform.root.GetComponent<Body>().leftHandEquipeed)
            transform.root.GetComponent<Body>().DropLeft();
    }

    public void LooseLife()
    {
        Camera.main.GetComponent<CameraMove>().LooseLife();
    }

    public void BashedSword(float bashTime)
    {
        swordDropped = true;

        rig.velocity /= 2;

        Destroy(weaponRJoint);

        if (twoHanded)
        {
            Destroy(weaponLJoint);
            leftHandEquipped = false;
        }


        GetComponent<ImpactTrigger>().isEnabled = false;
        //GetComponent<Impact>().enabled = false;

        transform.root.GetComponent<Body>().BashSword(bashTime);
    }

    public void GetUpSword()
    {
        if (hasDoll)
        {
            transform.position = handlerR.transform.position;
            weaponRJoint = handlerR.AddComponent<FixedJoint>();
            weaponRJoint.connectedBody = rig;
        }


        GetComponent<ImpactTrigger>().isEnabled = true;
        //GetComponent<Impact>().enabled = true;

        swordDropped = false;
    }

    public void DropSword()
    {
        transform.parent = null;
        Destroy(weaponRJoint);

        if (twoHanded)
            Destroy(weaponLJoint);

        if (!player)
        {
            GetComponent<Weapon>().dropped = true;
            GetComponent<ImpactTrigger>().isEnabled = false;
            //Destroy(GetComponent<ImpactTrigger>());
            //Destroy(GetComponent<Impact>());
            Destroy(GetComponent<SwordAI>());
            if (tip && tip.GetComponent<Impact>() && tip.GetComponent<ImpactTrigger>())
            {
                tip.GetComponent<ImpactTrigger>().isEnabled = false;
                //Destroy(tip.GetComponent<Impact>());
                //Destroy(tip.GetComponent<ImpactTrigger>());
            }
            Destroy(this);
            //GameObject.Find("Dungeon").GetComponent<Dungeon>().EnemyDie();

            rig.velocity /= 2;

            Destroy(glowSpot);
            if (audioListener)
                audioListener.transform.parent = null;
            ChangeSwordVisibleLayer(transform);
            if (mainModel) Destroy(mainModel.GetComponent<Outline>());
        }
        else
        {
            if (Camera.main.GetComponent<CameraMove>().lifes > 0)
            {
                if (swordDropped)
                    GetUpSword();

                newLifeTimer = newLifeTime;
                //Camera.main.GetComponent<CameraMove>().LooseLife();
                hasDoll = false;
                leftHandEquipped = false;

                GameObject newLife = Instantiate(newLifeEffect, glowSpot.transform.position, glowSpot.transform.rotation, glowSpot.transform);
                Destroy(newLife, 3);

                if (weigth)
                    weigth.gameObject.SetActive(true);
                GetComponent<ImpactTrigger>().hasDoll = false;

                glowSpot.transform.parent = null;
                glowTarget = transform.gameObject;
                movingGlow = true;
            }
            else
            {
                Destroy(GetComponent<ImpactTrigger>());
                Destroy(GetComponent<Impact>());
                Destroy(GetComponent<SwordHero>());
                Destroy(this);

                rig.velocity /= 2;
            }
        }

    }

    void ChangeSwordVisibleLayer(Transform go)
    {
        go.gameObject.layer = default;

        for (int i = 0; i < go.childCount; i++)
        {
            ChangeSwordVisibleLayer(go.GetChild(i));
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(CampaignMenu.OpenCount > 0) return;
        if (newLevelTimer > 0)
        {
            HoldUp();
            newLevelTimer -= Time.deltaTime;
        }
        else if (!stunned)
        {
            if (!swordDropped)
            {
                if (!evolutionating && !levelUp)
                {
                    HoldUp();

                    if (!followBody)
                    {
                        if (newLifeTimer > 0)
                        {
                            newLifeTimer -= Time.deltaTime;
                            rig.velocity = Vector3.zero;
                            rigTip.velocity = Vector3.zero;
                            if (weigth)
                                if(weigth) weigth.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        }
                        else
                            Move();
                    }
                    else
                        FollowBody();

                    if (hasDoll)
                    {
                        CheckHead();
                        if (twoHanded & !leftHandEquipped)
                            LeftHandSwordEquip();

                    }


                }
                else if(evolutionating)
                {
                    FollowAnvill();
                }
                //else
               // {
                   // FollowLevelUp();
                //}

                if (movingGlow)
                    MoveGlow();

                CheckMaxSpeed();

                CheckSkills();
            }
        }
        else
        {
            if (stunTimer > 0)
            {
                stunTimer -= Time.deltaTime;
            }
            else
            {
                stunned = false;
                if (transform.root.GetComponent<Body>())
                    transform.root.GetComponent<Body>().stunned = false;
            }
        }

    }

    void LeftHandSwordEquip()
    {
        Vector3 dir = transform.position - (handlerL.transform.position + transform.up * leftHandDeltaY);
        if (dir.magnitude > distanceLeftHandEquip)
        {
            rigHandlerL.AddForce(dir.normalized * withBodyMoveForce * 100 * Time.deltaTime);
        }
        else
        {
            leftHandEquipped = true;
            JointHands();
        }
    }

    void HoldUp()
    {
        if (transform.position.y < swordHoldY)
            rig.AddForce(new Vector3(0, 100, 0) * forceHold * healthK * Time.deltaTime);

        if (!twoHanded && hasDoll)
        {
            if (handlerL.transform.position.y < swordHoldY)
                rigHandlerL.AddForce(new Vector3(0, 100, 0) * forceHold * healthK * Time.deltaTime);
        }
    }
    void CheckMaxSpeed()
    {
        float tempMaxSpeed = maxSpeed;
        if (!hasDoll)
            tempMaxSpeed /= 1.5f;

        float velocityMagnitude = rig.velocity.magnitude;
        if (velocityMagnitude > tempMaxSpeed)
            rig.velocity *= tempMaxSpeed / velocityMagnitude;

        /*if (slashLevel > 0)
        {
            CheckSlash(velocityMagnitude);
        }

        if (duplicateShotLevel > 0)
        {
            CheckDuplicateShot(velocityMagnitude);
        }

        if(prickLevel > 0)
        {
            CheckPrick(velocityMagnitude);
        }*/
    }

    void Move()
    {
        float tempForceMove = withBodyMoveForce;
        var status=head?head.transform.root.GetComponent<CombatStatus>():null;if(status)tempForceMove*=status.MoveMultiplier;
        if (!hasDoll)
            tempForceMove /= 1.5f;

        if (moveVector != Vector3.zero)
            rigTip.AddForce(moveVector * tempForceMove * 100 * Time.deltaTime);
    }

    void FollowBody()
    {
        Vector3 forceVector = targetHandlerR.transform.position - transform.position;
        rig.AddForce(forceVector.normalized * 100 * forceMove * Time.deltaTime);
    }

    void FollowLevelUp()
    {
        Vector3 swordPlacePos = newLevelUp.transform.position+Vector3.up;
        Vector3 forceVector = swordPlacePos - transform.position;
        rig.AddForce(forceVector.normalized * 100 * forceAnvill * Time.deltaTime);

        if (!nearAnvill)
        {
            if (anvilTimer > 0)
                anvilTimer -= Time.deltaTime;
            else
                nearAnvill = true;
        }
        else
        {
            if (moveVector != null)
            {
                Vector3 eForceVector = Vector3.zero;

                    if(Mathf.Abs(moveVector.x) > moveVector.z)
                    {
                        if (moveVector.x < 0)
                        {
                            eForceVector = newLevelUp.GetComponent<LevelUp>().leftRune.transform.position;
                        }
                        else
                        {
                            eForceVector = newLevelUp.GetComponent<LevelUp>().rightRune.transform.position;
                        }
                    }
                    else if(moveVector.z > 0)
                    {
                        eForceVector = newLevelUp.GetComponent<LevelUp>().centerRune.transform.position;
                    }

                

                if(eForceVector != Vector3.zero)
                {
                    eForceVector -= transform.position;
                    rigTip.AddForce(eForceVector.normalized * 130 * forceAnvill * Time.deltaTime);
                }
                    
            }
        }
    }

    void FollowAnvill()
    {
        Vector3 swordPlacePos = nowAnvill.GetComponent<Anvill>().swordPlace.transform.position;
        Vector3 forceVector = swordPlacePos - transform.position;
        rig.AddForce(forceVector.normalized * 100 * forceAnvill * Time.deltaTime);

        if (!nearAnvill)
        {
            if (anvilTimer > 0)
                anvilTimer -= Time.deltaTime;
            else
                nearAnvill = true;
        }
        else
        {
            float leftEMag = (swordPlacePos + moveVector - nowAnvill.GetComponent<Anvill>().leftE.transform.position).magnitude;
            float rightEMag = (swordPlacePos + moveVector - nowAnvill.GetComponent<Anvill>().rightE.transform.position).magnitude;

            if (leftEMag != rightEMag)
            {
                Vector3 eForceVector;

                if (leftEMag > rightEMag)
                {
                    eForceVector = swordPlacePos - nowAnvill.GetComponent<Anvill>().leftE.transform.position;
                }
                else
                {
                    eForceVector = swordPlacePos - nowAnvill.GetComponent<Anvill>().rightE.transform.position;
                }

                rigTip.AddForce(eForceVector.normalized * 120 * forceAnvill * Time.deltaTime);
            }

        }
    }

    void MoveGlow()
    {
        Vector3 movePoint = new Vector3(glowTarget.transform.position.x, glowTarget.transform.position.y + 0.1f, glowTarget.transform.position.z);
        Vector3 moveVector = movePoint - glowSpot.transform.position;

        if (moveVector.magnitude > 0.1f)
        {
            glowSpot.transform.position += moveVector.normalized * glowSpeed * Time.deltaTime;
        }
        else
        {
            glowSpot.transform.position = movePoint;
            glowSpot.transform.parent = glowTarget.transform;
            movingGlow = false;
        }
    }

    void CheckHead()
    {
        if ((head.transform.position - transform.position).magnitude < headUpMagnitude && transform.position.y < head.transform.position.y + 0.3f)
        {
            rigTip.AddForce(new Vector3(0, 100, 0) * forceHead * healthK * Time.deltaTime);
        }
    }


    //Skills

    //DuplicateShot
    void DuplicateShotTimer()
    {
            if (duplicateShotTimer <= 0)
            {
                duplicateShotTimer = duplicateShotCouldown;
                DuplicateShot();   
            }
            else
            {
                duplicateShotTimer -= Time.deltaTime;
            }
    }

    void DuplicateShot()
    {
        //duplicateShotTimer = duplicateShotCouldown;

        GameObject nowDuplicateShot = Instantiate(duplicate, transform.position, Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z));
        
        nowDuplicateShot.transform.localScale = new Vector3(duplicateScaleK, duplicateScaleK, duplicateScaleK);
        nowDuplicateShot.GetComponent<BulletTrigger>().forceBack = Mathf.RoundToInt(GetComponent<Impact>().forceBack * duplicateForceK);
        nowDuplicateShot.GetComponent<BulletTrigger>().dollId = GetComponent<Impact>().dollId;
        nowDuplicateShot.GetComponent<BulletTrigger>().minDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().minDamage * duplicateDamageK);
        nowDuplicateShot.GetComponent<BulletTrigger>().maxDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().maxDamage * duplicateDamageK);
        nowDuplicateShot.GetComponent<BulletTrigger>().impactType = GetComponent<Impact>().impactType;

        if (nowDuplicateShot.GetComponent<Weapon>())
        {
            if (nowDuplicateShot.GetComponent<Weapon>().tip)
            {
                nowDuplicateShot.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().minDamage = Mathf.RoundToInt(tip.GetComponent<ImpactTrigger>().minDamage * duplicateDamageK);
                nowDuplicateShot.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().maxDamage = Mathf.RoundToInt(tip.GetComponent<ImpactTrigger>().maxDamage * duplicateDamageK);
                nowDuplicateShot.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().forceBack = Mathf.RoundToInt(tip.GetComponent<Impact>().forceBack * duplicateForceK);
                nowDuplicateShot.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().dollId = GetComponent<Impact>().dollId;
                nowDuplicateShot.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().impactType = GetComponent<Impact>().impactType;
            }
        }

        nowDuplicateShot.AddComponent<Bullet>();
    }

    void FireballShotTimer()
    {
        if (fireballShotTimer <= 0)
        {
            fireballShotTimer = fireballShotCouldown;
            FireballShot();
        }
        else
        {
            fireballShotTimer -= Time.deltaTime;
        }
    }

    void FireballShot()
    {
        //duplicateShotTimer = duplicateShotCouldown;

        GameObject nowFireballShot = Instantiate(fireball, transform.position, Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z));

        nowFireballShot.transform.localScale = new Vector3(fireballScaleK, fireballScaleK, fireballScaleK);
        //nowFireballShot.GetComponent<BulletTrigger>().forceBack = Mathf.RoundToInt(GetComponent<Impact>().forceBack * fireballForceK);
        nowFireballShot.GetComponent<BulletTrigger>().dollId = GetComponent<Impact>().dollId;

        int minDamage;
        int maxDamage;
        int forceBack;

        if (tip.GetComponent<ImpactTrigger>())
        {
            minDamage = Mathf.RoundToInt((tip.GetComponent<ImpactTrigger>().minDamage + GetComponent<ImpactTrigger>().minDamage) / 2 * fireballDamageK);
            maxDamage = Mathf.RoundToInt((tip.GetComponent<ImpactTrigger>().maxDamage + GetComponent<ImpactTrigger>().maxDamage) / 2 * fireballDamageK);
            forceBack = Mathf.RoundToInt((tip.GetComponent<Impact>().forceBack + GetComponent<Impact>().forceBack) / 2 * fireballForceK);
        }
        else
        {
            minDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().minDamage * fireballDamageK);
            maxDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().maxDamage * fireballDamageK);
            forceBack = Mathf.RoundToInt(GetComponent<Impact>().forceBack * fireballForceK);
        }

        nowFireballShot.GetComponent<BulletTrigger>().minDamage = minDamage;
        nowFireballShot.GetComponent<BulletTrigger>().maxDamage = maxDamage;
        nowFireballShot.GetComponent<BulletTrigger>().forceBack = forceBack;
        //nowFireballShot.GetComponent<BulletTrigger>().impactType = GetComponent<Impact>().impactType;

        nowFireballShot.AddComponent<Bullet>();
    }

    //Prick
    void PrickShotTimer()
    {
        if (prickTimer <= 0)
        {
            prickTimer = prickCouldown;
            Slash(prickFX);
        }
        else
        {
            prickTimer -= Time.deltaTime;
        }
    }

    //DuplicateClone
    void DuplicateCloneTimer()
    {
        if (duplicateCloneTime <= 0)
        {
            duplicateCloneTime = duplicateCloneCouldown;
            DuplicateClone();
        }
        else
        {
            duplicateCloneTime -= Time.deltaTime;
        }
    }

    public void DuplicateClone()
    {
        GameObject nowDuplicateClone = Instantiate(duplicate, transform.position, transform.rotation);
        
        nowDuplicateClone.GetComponent<Rigidbody>().isKinematic = true;
        nowDuplicateClone.AddComponent<DuplicateClone>();
        nowDuplicateClone.GetComponent<DuplicateClone>().targetSword = gameObject;

        nowDuplicateClone.transform.localScale = new Vector3(duplicateScaleK, duplicateScaleK, duplicateScaleK);
        nowDuplicateClone.GetComponent<BulletTrigger>().forceBack = Mathf.RoundToInt(GetComponent<Impact>().forceBack * duplicateForceK);
        nowDuplicateClone.GetComponent<BulletTrigger>().dollId = GetComponent<Impact>().dollId;
        nowDuplicateClone.GetComponent<BulletTrigger>().minDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().minDamage * duplicateDamageK);
        nowDuplicateClone.GetComponent<BulletTrigger>().maxDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().maxDamage * duplicateDamageK);
        nowDuplicateClone.GetComponent<BulletTrigger>().impactType = GetComponent<Impact>().impactType;

        nowDuplicateClone.GetComponent<BulletTrigger>().lifeTime = duplicateCloneLifeTime;

        if (nowDuplicateClone.GetComponent<Weapon>())
        {
            if (nowDuplicateClone.GetComponent<Weapon>().tip)
            {
                nowDuplicateClone.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().minDamage = Mathf.RoundToInt(tip.GetComponent<ImpactTrigger>().minDamage * duplicateDamageK);
                nowDuplicateClone.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().maxDamage = Mathf.RoundToInt(tip.GetComponent<ImpactTrigger>().maxDamage * duplicateDamageK);
                nowDuplicateClone.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().forceBack = Mathf.RoundToInt(tip.GetComponent<Impact>().forceBack * duplicateForceK);
                nowDuplicateClone.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().dollId = GetComponent<Impact>().dollId;
                nowDuplicateClone.GetComponent<Weapon>().tip.GetComponent<BulletTrigger>().impactType = GetComponent<Impact>().impactType;
            }
        }
    }

    //Slash
    void SlashShotTimer()
    {
        if (slashTimer <= 0)
        {
            slashTimer = slashCouldown;
            Slash(slashFX);
        }
        else
        {
            slashTimer -= Time.deltaTime;
        }
    }

    public void Slash(GameObject slash)
    {
        //slashTimer = slashCouldown;
        GameObject newPrick = Instantiate(slash, tip.transform.position, tip.transform.rotation);
        newPrick.transform.localScale = new Vector3(slashScale, slashScale, slashScale);
        newPrick.transform.GetChild(0).transform.localScale = new Vector3(slashScale, slashScale, slashScale);
        Destroy(newPrick, 3);

        int minDamage;
        int maxDamage;
        int forceBack;

        if (tip.GetComponent<ImpactTrigger>())
        {
            minDamage = Mathf.RoundToInt((tip.GetComponent<ImpactTrigger>().minDamage + GetComponent<ImpactTrigger>().minDamage)/2 * slashDamageK);
            maxDamage = Mathf.RoundToInt((tip.GetComponent<ImpactTrigger>().maxDamage + GetComponent<ImpactTrigger>().maxDamage) / 2 * slashDamageK);
            forceBack = Mathf.RoundToInt((tip.GetComponent<Impact>().forceBack + GetComponent<Impact>().forceBack)/2 * slashForceK);
        }
        else
        {
            minDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().minDamage * slashDamageK);
            maxDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().maxDamage * slashDamageK);
            forceBack = Mathf.RoundToInt(GetComponent<Impact>().forceBack * slashForceK);
        }

        newPrick.GetComponent<SkillTrigger>().dollId = GetComponent<ImpactTrigger>().dollId;
        newPrick.GetComponent<SkillTrigger>().damageScore = GetComponent<ImpactTrigger>().damageScore;
        newPrick.GetComponent<SkillTrigger>().minDamage = minDamage;
        newPrick.GetComponent<SkillTrigger>().maxDamage = maxDamage;
        newPrick.GetComponent<SkillTrigger>().forceBack = forceBack;
    }

    void CheckDuplicateAround()
    {
         if (duplicateAroundTimer <= 0)
         {
             if (duplicatesAroundCount < maxduplicateAround)
             {
                duplicatesAroundCount++;
                duplicateAroundTimer = duplicateAroundCouldown;
                DuplicateAround();
              }
         }
         else
         {
             duplicateAroundTimer -= Time.deltaTime;
         } 
    }

    void DuplicateAround()
    {
        GameObject nowFireball = Instantiate(duplicateAroundPrefab, transform.position, Quaternion.identity);
        nowFireball.GetComponent<FireballAround>().fireball = duplicate;
        nowFireball.GetComponent<FireballAround>().fireballType = 0;
        nowFireball.GetComponent<FireballAround>().rotateAroundAdd = true;
        nowFireball.GetComponent<FireballAround>().swordSkill = true;
        nowFireball.GetComponent<FireballAround>().followTarget = gameObject;
        nowFireball.GetComponent<FireballAround>().duplicateScaleK = duplicateScaleK;
        nowFireball.GetComponent<FireballAround>().forceBack = Mathf.RoundToInt(GetComponent<Impact>().forceBack * duplicateForceK);
        nowFireball.GetComponent<FireballAround>().dollId = GetComponent<Impact>().dollId;
        nowFireball.GetComponent<FireballAround>().minDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().minDamage * duplicateDamageK);
        nowFireball.GetComponent<FireballAround>().maxDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().maxDamage * duplicateDamageK);
        nowFireball.GetComponent<FireballAround>().impactType = GetComponent<Impact>().impactType;

        if (duplicate.GetComponent<Weapon>())
        {
            if (duplicate.GetComponent<Weapon>().tip)
            {
                nowFireball.GetComponent<FireballAround>().tipminDamage = Mathf.RoundToInt(tip.GetComponent<ImpactTrigger>().minDamage * duplicateDamageK);
                nowFireball.GetComponent<FireballAround>().tipmaxDamage = Mathf.RoundToInt(tip.GetComponent<ImpactTrigger>().maxDamage * duplicateDamageK);
                nowFireball.GetComponent<FireballAround>().tipforceBack = Mathf.RoundToInt(tip.GetComponent<Impact>().forceBack * duplicateForceK);
            }
        }
    }

    void CheckFireballAround()
    {
        if (fireballAroundTimer <= 0)
        {
            if (fireballAroundCount < maxfireballAround)
            {
                fireballAroundCount++;
                fireballAroundTimer = fireballAroundCouldown;
                FireballAround();
            }
        }
        else
        {
            fireballAroundTimer -= Time.deltaTime;
        }
    }

    void FireballAround()
    {
        GameObject nowFireball = Instantiate(duplicateAroundPrefab, transform.position, Quaternion.identity);
        nowFireball.GetComponent<FireballAround>().fireball = fireball;
        nowFireball.GetComponent<FireballAround>().rotateAroundAdd = false;
        nowFireball.GetComponent<FireballAround>().fireballType = 1;
        nowFireball.GetComponent<FireballAround>().swordSkill = true;
        nowFireball.GetComponent<FireballAround>().followTarget = gameObject;
        nowFireball.GetComponent<FireballAround>().duplicateScaleK = fireballScaleK;
        nowFireball.GetComponent<FireballAround>().dollId = GetComponent<Impact>().dollId;

        int minDamage;
        int maxDamage;
        int forceBack;

        if (tip.GetComponent<ImpactTrigger>())
        {
            minDamage = Mathf.RoundToInt((tip.GetComponent<ImpactTrigger>().minDamage + GetComponent<ImpactTrigger>().minDamage) / 2 * fireballDamageK);
            maxDamage = Mathf.RoundToInt((tip.GetComponent<ImpactTrigger>().maxDamage + GetComponent<ImpactTrigger>().maxDamage) / 2 * fireballDamageK);
            forceBack = Mathf.RoundToInt((tip.GetComponent<Impact>().forceBack + GetComponent<Impact>().forceBack) / 2 * fireballForceK);
        }
        else
        {
            minDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().minDamage * fireballDamageK);
            maxDamage = Mathf.RoundToInt(GetComponent<ImpactTrigger>().maxDamage * fireballDamageK);
            forceBack = Mathf.RoundToInt(GetComponent<Impact>().forceBack * fireballForceK);
        }

        nowFireball.GetComponent<FireballAround>().fireball.GetComponent<BulletTrigger>().minDamage = minDamage;
        nowFireball.GetComponent<FireballAround>().fireball.GetComponent<BulletTrigger>().maxDamage = maxDamage;
        nowFireball.GetComponent<FireballAround>().fireball.GetComponent<BulletTrigger>().forceBack = forceBack;
    }

    void CheckSkills()
    {
        if (duplicateAround)
            CheckDuplicateAround();

        if (duplicateShot)
            DuplicateShotTimer();

        if (slashShot)
            SlashShotTimer();

        if (prickShot)
            PrickShotTimer();

        if (duplicateClone)
            DuplicateCloneTimer();

        if (fireballShot)
            FireballShotTimer();

        if (fireballAround)
            CheckFireballAround();
    }
}
