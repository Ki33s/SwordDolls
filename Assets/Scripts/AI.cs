
using UnityEngine;

public class AI : MonoBehaviour
{
    public float moveVerticalsMinTime = 0.5f;
    public float moveVerticalsMaxTime = 2f;
    public float attackVerticalsMinTime = 0.1f;
    public float attackVerticalsMaxTime = 0.3f;
    public float runVerticalsMinTime = 0.5f;
    public float runVerticalsMaxTime = 2f;

    public float moveChance = 0.5f;
    public float attackPlayerChance = 0.5f;
    public float runChance = 0.5f;
    public float playerSeeDistance = 10f;
    public float wallSeeDistance = 0.5f;
    public float startTime = 1f;

    public float moveDelta = 2f;

    public float forceMin = 0.5f;
    public float forceMax = 1f;

    public float voiceRunProb = 0.2f;
    public float voiceAttackProb = 0.1f;
    public GameObject voiceRun;
    public GameObject voiceAttack;
}
