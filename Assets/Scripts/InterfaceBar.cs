
using UnityEngine;
using UnityEngine.UI;

public class InterfaceBar : MonoBehaviour
{
    public Slider slide;

    public void UpdateBar(float val)
    {
        slide.value = val;
    }
}
