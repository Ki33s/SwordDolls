using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public float timeOpen = 1f;
    public int angleOpen = 120;
    public bool opened = false;
    public GameObject rightDoor;
    public GameObject leftDoor;
    
    private float timerOpen;
    // Start is called before the first frame update
    void Start()
    {
        timerOpen = timeOpen;
    }

    // Update is called once per frame
    void Update()
    {
        if (opened)
            Opening();
    }

    void Opening()
    {
        if (timerOpen > 0)
        {
            timerOpen -= Time.deltaTime;

            rightDoor.transform.eulerAngles = new Vector3(rightDoor.transform.eulerAngles.x, rightDoor.transform.eulerAngles.y + angleOpen / timeOpen * Time.deltaTime, rightDoor.transform.eulerAngles.z);
            
            if(leftDoor)
                leftDoor.transform.eulerAngles = new Vector3(leftDoor.transform.eulerAngles.x, leftDoor.transform.eulerAngles.y - angleOpen / timeOpen * Time.deltaTime, leftDoor.transform.eulerAngles.z);
        }
        else
        {
            opened = false;
            timerOpen = timeOpen;
            angleOpen *= -1;
        }
    }
}
