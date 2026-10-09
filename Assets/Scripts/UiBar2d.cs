
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiBar2d : MonoBehaviour
{
    public Image UiBar;
    public TextMeshProUGUI levelText;
    public float changeTime = 0.5f;
    private float timer = 0f;
    private float nowVal = 0f;
    private float deltaVal = 0f;
    private int nowLevel = 1;
    // Start is called before the first frame update
    public void ChangeBar(float val, int level)
    {
        timer = changeTime;

        if(level == nowLevel)
        {
            deltaVal = (val - nowVal) / changeTime * Time.deltaTime;
        }
        else
        {
            deltaVal = (level - nowLevel + val - nowVal) / changeTime * Time.deltaTime;
        }

        nowVal += deltaVal;
    }

    private void FixedUpdate()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            nowVal += deltaVal;

            if(nowVal >= 1)
            {
                nowVal -= 1;
                nowLevel++;
                levelText.text = nowLevel.ToString();
            }

            UiBar.fillAmount = nowVal;
        }
    }
}
