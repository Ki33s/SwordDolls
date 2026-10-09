
using UnityEngine;

public class SwordAI : MonoBehaviour
{
    private float moveVerticalsMinTime = 0.1f;
    private float moveVerticalsMaxTime = 0.3f;
    private float attackVerticalsMinTime = 0.1f;
    private float attackVerticalsMaxTime = 0.3f;
    private float runVerticalsMinTime = 0.5f;
    private float runVerticalsMaxTime = 2f;
    private float moveChance = 0.5f;
    private float attackPlayerChance = 0.4f;
    private float runChance = 0.8f;
    private float playerSeeDistance = 10f;
    private float wallSeeDistance = 0.5f;
    private float moveVerticalsTimer = 0f;
    private float forceMin = 0.5f;
    private float forceMax = 1f;

    private float moveDelta = 1f;
    public GameObject playerSword;

    private float voiceRunProb = 0.2f;
    private float voiceAttackProb = 0.1f;
    private GameObject voiceRun;
    private GameObject voiceAttack;

    private int actionType = 0;
    private Vector2 moveDeltaVector;

    private Vector3 randMoveVector;
    private float idleMoveTimer = 0f;
    private float startTimer = 0f;
    private Vector3 startDir;
    void Start()
    {
        playerSword = GameObject.FindGameObjectWithTag("Player");

        moveVerticalsMinTime = transform.root.GetComponent<AI>().moveVerticalsMinTime;
        moveVerticalsMaxTime = transform.root.GetComponent<AI>().moveVerticalsMaxTime;
        attackVerticalsMinTime = transform.root.GetComponent<AI>().attackVerticalsMinTime;
        attackVerticalsMaxTime = transform.root.GetComponent<AI>().attackVerticalsMaxTime;
        runVerticalsMinTime = transform.root.GetComponent<AI>().runVerticalsMinTime;
        runVerticalsMaxTime = transform.root.GetComponent<AI>().runVerticalsMaxTime;

        moveChance = transform.root.GetComponent<AI>().moveChance;
        attackPlayerChance = transform.root.GetComponent<AI>().attackPlayerChance;
        runChance = transform.root.GetComponent<AI>().runChance;

        startTimer = transform.root.GetComponent<AI>().startTime;

        moveDelta = transform.root.GetComponent<AI>().moveDelta;

        forceMin = transform.root.GetComponent<AI>().forceMin;
        forceMax = transform.root.GetComponent<AI>().forceMax;

        voiceRunProb = transform.root.GetComponent<AI>().voiceRunProb;
        voiceAttackProb = transform.root.GetComponent<AI>().voiceAttackProb;
        voiceRun = transform.root.GetComponent<AI>().voiceRun;
        voiceAttack = transform.root.GetComponent<AI>().voiceAttack;

        startDir = transform.root.transform.forward;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!playerSword)playerSword = GameObject.FindGameObjectWithTag("Player");
        if(!playerSword)return;
        if(startTimer < 0)
            Controls();
        else
        {
            startTimer -= Time.deltaTime;

            GetComponent<Sword>().moveVector = new Vector3(startDir.x,0, startDir.z).normalized * Random.Range(forceMin, forceMax);
        }
    }

    void CheckVoiceAttack()
    {
        if (Random.Range(0f, 1f) < voiceAttackProb)
        {
            Instantiate(voiceAttack, transform.position, Quaternion.identity);
        }
    }

    void CheckVoiceRun()
    {
        if (Random.Range(0f, 1f) < voiceRunProb)
        {
            Instantiate(voiceRun, transform.position, Quaternion.identity);
        }
    }

    void Controls()
    {
        Vector3 moveVector = Vector3.zero;
        Vector2 playerDir;

        if(playerSword.GetComponent<Sword>().hasDoll)
        {
            playerDir = (new Vector2(playerSword.transform.root.GetComponent<Body>().head.transform.position.x, playerSword.transform.root.GetComponent<Body>().head.transform.position.z) - new Vector2(transform.position.x, transform.position.z)).normalized;
        }
        else
        {
            playerDir = (new Vector2(playerSword.transform.position.x, playerSword.transform.position.z) - new Vector2(transform.position.x, transform.position.z)).normalized;
        }

        if (playerDir.magnitude < playerSeeDistance)
        {
            if(moveVerticalsTimer > 0)
            {
                moveVerticalsTimer -= Time.deltaTime;
            }
            else
            {
                if (playerSword.GetComponent<Sword>().hasDoll)
                {
                    if (Random.Range(0f, 1f) < attackPlayerChance)
                    {
                        moveVerticalsTimer = Random.Range(attackVerticalsMinTime, attackVerticalsMaxTime);
                        actionType = 0;
                        CheckVoiceAttack();
                    }
                    else if (Random.Range(0f, 1f) < moveChance)
                    {
                        moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
                        actionType = 2;
                        moveDeltaVector = new Vector2(Random.Range(-moveDelta, moveDelta), Random.Range(-moveDelta, moveDelta));
                    }
                    else if (Random.Range(0f, 1f) < runChance)
                    {
                        moveVerticalsTimer = Random.Range(runVerticalsMinTime, runVerticalsMaxTime);
                        actionType = 1;
                        CheckVoiceRun();
                    }
                }
                else
                {
                    if (Random.Range(0f, 1f) < runChance)
                    {
                        moveVerticalsTimer = Random.Range(runVerticalsMinTime, runVerticalsMaxTime);
                        actionType = 1;
                        CheckVoiceRun();
                    }
                    else if (Random.Range(0f, 1f) < moveChance)
                    {
                        moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
                        actionType = 2;
                        moveDeltaVector = new Vector2(Random.Range(-moveDelta, moveDelta), Random.Range(-moveDelta, moveDelta));
                    }
                    else if (Random.Range(0f, 1f) < attackPlayerChance)
                    {
                        moveVerticalsTimer = Random.Range(attackVerticalsMinTime, attackVerticalsMaxTime);
                        actionType = 0;
                        CheckVoiceAttack();
                    }
                }

                if (idleMoveTimer > 0)
                    idleMoveTimer = 0;
            }
        }
        else //Dont see enemy
        {
            if(idleMoveTimer > 0)
            {
                idleMoveTimer -= Time.deltaTime;
            }
            else
            {
                idleMoveTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
                actionType = 3;
                randMoveVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            }
        }

        switch (actionType)
        {
            case 0:
                moveVector = new Vector3(playerDir.x, 0, playerDir.y);
                break;

            case 1:
                moveVector = new Vector3(-playerDir.x, 0, -playerDir.y);
                break;

            case 2:
                playerDir = (new Vector2(playerSword.transform.position.x + moveDeltaVector.x, playerSword.transform.position.z + moveDeltaVector.y) - new Vector2(transform.position.x, transform.position.z)).normalized;
                moveVector = new Vector3(playerDir.x, 0, playerDir.y);
                break;

            case 3:
                moveVector = randMoveVector;
                break;
        }

       
        float randDelta = 0.5f;
        moveVector = new Vector3(moveVector.x + Random.Range(-randDelta,randDelta), 0, moveVector.z + Random.Range(-randDelta, randDelta)).normalized;
        moveVector = CheckWall(moveVector);
        GetComponent<Sword>().moveVector = moveVector * Random.Range(forceMin, forceMax);
    }

    /* void Controls()
    {
        if (moveVerticalsTimer < 0)
        {
            Vector3 moveVector;
            Vector2 playerDir = new Vector2(playerSword.transform.position.x, playerSword.transform.position.z) - new Vector2(transform.position.x, transform.position.z);

            if (playerDir.magnitude < playerSeeDistance)
            {
                if (playerSword.GetComponent<Sword>().hasDoll)
                {
                    if (Random.Range(0f, 1f) < attackPlayerChance)
                    {
                        playerDir = new Vector2(playerSword.transform.root.GetComponent<Body>().head.transform.position.x, playerSword.transform.root.GetComponent<Body>().head.transform.position.z) - new Vector2(transform.position.x, transform.position.z);
                        //Vector2 playerDir = new Vector2(playerSword.transform.position.x, playerSword.transform.position.z) - new Vector2(transform.position.x, transform.position.z);
                        moveVerticalsTimer = Random.Range(attackVerticalsMinTime, attackVerticalsMaxTime);

                        CheckVoiceAttack();
                    }
                    else if (Random.Range(0f, 1f) < moveChance)
                    {
                        playerDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                        moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
                    }
                    else if (Random.Range(0f, 1f) < runChance)
                    {
                        playerDir *= -1;
                        moveVerticalsTimer = Random.Range(runVerticalsMinTime, runVerticalsMaxTime);
                        CheckVoiceRun();
                    }
                    else
                    {
                        playerDir = Vector2.zero;
                        moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
                    }
                }
                else
                {
                    if (Random.Range(0f, 1f) < runChance)
                    {
                        playerDir *= -1;
                        moveVerticalsTimer = Random.Range(runVerticalsMinTime, runVerticalsMaxTime);

                        CheckVoiceRun();
                    }
                    else if (Random.Range(0f, 1f) < attackPlayerChance)
                    {
                        moveVerticalsTimer = Random.Range(attackVerticalsMinTime, attackVerticalsMaxTime);

                        CheckVoiceAttack();
                    }
                    else if (Random.Range(0f, 1f) < moveChance)
                    {
                        playerDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                        moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
                    }
                    else
                    {
                        playerDir = Vector2.zero;
                        moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
                    }
                }
            }
            else if(Random.Range(0f,1f) < moveChance)
            {
                playerDir = Vector2.zero;
                moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
            }
            else
            {
                playerDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                moveVerticalsTimer = Random.Range(moveVerticalsMinTime, moveVerticalsMaxTime);
            }

            moveVector = new Vector3(playerDir.x, 0, playerDir.y).normalized;
            moveVector = CheckWall(moveVector);
            GetComponent<Sword>().moveVector = moveVector*Random.Range(forceMin,forceMax);
        }
        else
        {
            moveVerticalsTimer -= Time.deltaTime;
        }
    }*/

    Vector3 CheckWall(Vector3 dir)
    {
        Vector3 newDir = dir;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, wallSeeDistance))
        {
            if (hit.transform.tag == "Wall")
                newDir *= -1;
        }

        return newDir;
    }
}
