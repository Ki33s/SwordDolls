using System;
using System.Collections;
using System.IO;
using UnityEngine;
public sealed class RecoveryProbe : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-recoveryProbe") >= 0)
            new GameObject("Recovery probe").AddComponent<RecoveryProbe>();
    }
    IEnumerator Start()
    {
        UnityEngine.Random.InitState(42);
        yield return new WaitForSecondsRealtime(3);
        var hero = FindFirstObjectByType<SwordHero>();
        Debug.Log("RECOVERY_RUNTIME hero=" + (hero ? hero.name : "missing") + " bodies=" + FindObjectsByType<Body>(FindObjectsSortMode.None).Length);
        if (!hero) { Application.Quit(1); yield break; }
        Vector3 start = hero.transform.position;
        hero.enabled = false;
        var sword = hero.GetComponent<Sword>();
        for (int i = 0; i < 180; i++) { sword.moveVector = Vector3.forward; yield return new WaitForFixedUpdate(); }
        sword.moveVector = Vector3.zero;
        Debug.Log("RECOVERY_MOVEMENT distance=" + Vector3.Distance(start, hero.transform.position) + " hasDoll=" + sword.hasDoll);
        yield return new WaitForSecondsRealtime(1);
        var camera = Camera.main;
        if (camera)
        {
            var rt = new RenderTexture(1280, 720, 24);
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0,0,1280,720),0,0); image.Apply();
            File.WriteAllBytes(Path.Combine(Application.dataPath, "../../recovery-preview.png"), image.EncodeToPNG());
            camera.targetTexture = null; RenderTexture.active = null;
            Destroy(rt); Destroy(image);
        }
        Application.Quit();
    }
}
