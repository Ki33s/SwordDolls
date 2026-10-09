
using UnityEngine;

public class RemoveFromCamera : MonoBehaviour
{
    public GameObject rendererModel;
    public void Trans()
    {
        if(rendererModel)
            rendererModel.GetComponent<MeshRenderer>().enabled = false;
        else
            GetComponent<MeshRenderer>().enabled = false;
    }

    public void UnTrans()
    {
        if (rendererModel)
            rendererModel.GetComponent<MeshRenderer>().enabled = true;
        else
            GetComponent<MeshRenderer>().enabled = true;
    }
}
