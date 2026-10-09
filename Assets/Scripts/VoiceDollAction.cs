
using UnityEngine;

public class VoiceDollAction : MonoBehaviour
{
    public AudioClip[] actionSound;
    // Start is called before the first frame update
    void Start()
    {
        
            AudioSource hitSound = gameObject.AddComponent<AudioSource>();
            hitSound.clip = actionSound[Random.Range(0, actionSound.Length)];
            hitSound.Play();
            Destroy(gameObject, 3);
    }
}
