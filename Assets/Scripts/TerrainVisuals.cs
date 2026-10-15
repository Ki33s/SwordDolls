using UnityEngine;

// The uneven ground and grass are artwork; City supplies one flat support collider.
public sealed class TerrainVisuals : MonoBehaviour
{
    public static void RemoveCollision(GameObject visual)
    {
        foreach (var collider in visual.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = false;
            Destroy(collider);
        }
    }
}
