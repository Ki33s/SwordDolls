
using UnityEngine;

public class ImpactSoundEffect : MonoBehaviour
{
    public AudioClip[] impactBloodAudioclips;
    public AudioClip[] impactMetallAudioclips;
    public AudioClip[] impactStoneAudioclips;
    public AudioClip[] impactWoodAudioclips;
    public int impactType = 0;
    // Start is called before the first frame update
    void Start()
    {
        AudioSource hitSound = gameObject.AddComponent<AudioSource>();

        switch (impactType)
        {
            case 0:
                
                hitSound.clip = impactBloodAudioclips[Random.Range(0, impactBloodAudioclips.Length)];
                break;

            case 1:
                hitSound.clip = impactMetallAudioclips[Random.Range(0, impactMetallAudioclips.Length)];
                break;

            case 2:
                hitSound.clip = impactStoneAudioclips[Random.Range(0, impactStoneAudioclips.Length)];
                break;

            case 3:
                hitSound.clip = impactWoodAudioclips[Random.Range(0, impactWoodAudioclips.Length)];
                break;
        }

        hitSound.Play();
        Destroy(gameObject, 3);
    }

}
