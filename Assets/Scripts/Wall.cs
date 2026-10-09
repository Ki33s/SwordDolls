
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Material main;
    public Material trans;
    
    public void Trans(Transform go)
    {
        if(go.GetComponent<Renderer>())
        {
            if(go.tag != "FX")
                go.GetComponent<Renderer>().material = trans;
        }
            

        if(go.childCount > 0)
        {
            for(int i = 0; i<go.childCount; i++)
            {
                Trans(go.GetChild(i));
            }
        }
    }

    public void UnTrans(Transform go)
    {
        if (go.GetComponent<Renderer>())
        {
            if (go.tag != "FX")
                go.GetComponent<Renderer>().material = main;
        }
            
        if (go.childCount > 0)
        {
            for (int i = 0; i < go.childCount; i++)
            {
                UnTrans(go.GetChild(i));
            }
        }
    }
}
