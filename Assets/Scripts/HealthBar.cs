
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Image UiBar;
    public TextMeshProUGUI level;
    public float changeTime = 0.5f;
    private float timer = 0f;
    private float nowVal = 1f;
    private float deltaVal = 0f;
    // Start is called before the first frame update
    
    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            nowVal += deltaVal;

            UiBar.fillAmount = nowVal;
        }
    }

    public void UpdateBar(float val)
    {
        timer = changeTime;
        deltaVal = (val - nowVal) / changeTime * Time.deltaTime;
    }
}
