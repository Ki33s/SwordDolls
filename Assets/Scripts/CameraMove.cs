
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    public int lifes = 3;
    public float camDamping = 2;
    public float anvilCameraKoef = 0.75f;
    public float anvilDeltaZ = -1f;
    public int anvilRotateX = 10;
    private Vector3 cameraDeltaVector;
    private GameObject playerSword;

    public GameObject expBar;
    public GameObject levelText;

    public float levelUpDeltaZ = -3f;

    public List<ParticleSystem> lifesGo = new List<ParticleSystem>();

    public Vector3 anvillPos;
    public bool evo = false;
    private bool lifesSpawned = false;

    private float startRotateX;

    // Start is called before the first frame update
    public void GetPlayer(GameObject sword)
    {
        playerSword = sword;
        evo = false;

        if(!lifesSpawned)
        {
            lifesSpawned = true;
            cameraDeltaVector = transform.position - sword.transform.position;
            startRotateX = transform.eulerAngles.x;
            SpawnLifes();
        }
    }

    public void SpawnLifes()
    {
        GameObject lifeInterface = GetComponent<Interface>().lifeInterface;
        ParticleSystem life = GetComponent<Interface>().life;
        float lifeDeltaX = GetComponent<Interface>().lifeDeltaX;


        for (int i = 0; i < lifes; i++)
        {
            ParticleSystem newLife = Instantiate(life, new Vector3(lifeInterface.transform.position.x + lifeDeltaX * i, lifeInterface.transform.position.y, lifeInterface.transform.position.z), Quaternion.identity, lifeInterface.transform);
            lifesGo.Add(newLife);
        }
    }

    public void LooseLife()
    {
        lifes--;
        lifesGo[lifes].loop = false;
        Destroy(lifesGo[lifes], 3);
        lifesGo.Remove(lifesGo[lifes]);
    }

    public void AddLife()
    {
        lifes++;
        GameObject lifeInterface = GetComponent<Interface>().lifeInterface;
        ParticleSystem life = GetComponent<Interface>().life;
        float lifeDeltaX = GetComponent<Interface>().lifeDeltaX;

            ParticleSystem newLife = Instantiate(life, new Vector3(lifeInterface.transform.position.x + lifeDeltaX * (lifes-1), lifeInterface.transform.position.y, lifeInterface.transform.position.z), Quaternion.identity, lifeInterface.transform);
            lifesGo.Add(newLife);
    }

    public void Evolution(Vector3 newAnvillPos)
    {
        anvillPos = newAnvillPos;
        evo = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerSword)
        {
            CameraFollow();
        }
    }

    void CameraFollow()
    {
        if (!evo)
        {
            GameObject target = playerSword;

            if (target.GetComponent<Sword>().hasDoll)
                target = target.transform.root.GetComponent<Body>().head;

            transform.position = Vector3.Lerp(transform.position, target.transform.position + cameraDeltaVector, camDamping * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(startRotateX, 0, 0), camDamping * Time.deltaTime);
        } 
        else
        {
            Vector3 targetPos = anvillPos + cameraDeltaVector * anvilCameraKoef;
            targetPos = new Vector3(targetPos.x, targetPos.y, targetPos.z + anvilDeltaZ);
            transform.position = Vector3.Lerp(transform.position, targetPos, camDamping * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(anvilRotateX, 0, 0), camDamping * Time.deltaTime);
        }
            
    }
}
