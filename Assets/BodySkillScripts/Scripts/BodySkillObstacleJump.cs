
using UnityEngine;

public class BodySkillObstacleJump : MonoBehaviour
{
    public int headForcePlus = 50;
    public int obstacleForcePlus = 50;
    public float jumpTimePlus = 0.05f;
    public float jumpCouldownPlus = -0.1f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject body = GetComponent<BodyAllSkill>().body;
        body.GetComponent<Body>().headForce += headForcePlus;
        body.GetComponent<Body>().forceObstacle += obstacleForcePlus;
        body.GetComponent<Body>().obstacleJumpTime += jumpTimePlus;
        body.GetComponent<Body>().obstacleJumpCouldown += jumpCouldownPlus;
        Destroy(gameObject);
    }
}
