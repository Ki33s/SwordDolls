
using UnityEngine;

public class SkillNewLife : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.GetComponent<CameraMove>().AddLife();
    }
}
