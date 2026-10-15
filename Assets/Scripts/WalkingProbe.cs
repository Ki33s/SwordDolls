using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Opt-in, repeatable play session using the same controls and possession as the game.
public sealed class WalkingProbe : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-walkingProbe") >= 0)
            new GameObject("Walking review").AddComponent<WalkingProbe>();
    }
    [Serializable] sealed class Segment
    {
        public string name;
        public int samples, steps;
        public float averageSpeed, footLag, maxFootLag, lowHeadFraction, footClearance, groundContact;
    }
    [Serializable] sealed class Report
    {
        public string mode, pelvis;
        public float totalMass, legLength;
        public List<Segment> segments = new List<Segment>();
    }
    string errors;
    float started;
    void OnEnable() { Application.logMessageReceived += Log; started = Time.realtimeSinceStartup; }
    void OnDisable() { Application.logMessageReceived -= Log; SwordInput.TestEnabled = false; }
    void Log(string text, string stack, LogType type) { if (type == LogType.Exception || type == LogType.Error) errors = text + " " + stack; }
    IEnumerator Start()
    {
        var sequence = Run();
        while (true)
        {
            object next = null; bool running = false; string failure = null;
            try
            {
                if (errors != null) throw new Exception(errors);
                if (Time.realtimeSinceStartup - started > 60) throw new Exception("Walking review timeout");
                running = sequence.MoveNext(); if (running) next = sequence.Current;
            }
            catch (Exception error) { failure = error.ToString(); }
            if (failure != null) { Debug.Log("WALKING_FAIL " + failure); Application.Quit(1); yield break; }
            if (!running) break;
            yield return next;
        }
        Debug.Log("WALKING_PASS"); Application.Quit();
    }
    IEnumerator Run()
    {
        bool legacy = Array.IndexOf(Environment.GetCommandLineArgs(), "-legacyWalking") >= 0;
        SwordInput.TestEnabled = true; SwordInput.TestAxes = Vector2.zero; UnityEngine.Random.InitState(42);
        yield return new WaitForSecondsRealtime(1);
        var hero = GameObject.FindGameObjectWithTag("Player");
        if (!hero) throw new Exception("Player missing");
        foreach (var actor in FindObjectsByType<Body>(FindObjectsSortMode.None)) Destroy(actor.gameObject);
        foreach (var weapon in FindObjectsByType<Sword>(FindObjectsSortMode.None)) if (weapon.gameObject != hero) Destroy(weapon.gameObject);
        var global = GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();
        global.nowCity.SetActive(false); CampaignHUD.Active.enabled = false;
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube); floor.name = "Walking review floor";
        floor.transform.position = Vector3.down * .5f; floor.transform.localScale = new Vector3(200, 1, 200);
        floor.GetComponent<Renderer>().sharedMaterial = global.nowCity.GetComponent<City>().groundMaterials[0];
                string bodyType = "fighter";
        var arguments = Environment.GetCommandLineArgs(); int bodyArgument = Array.IndexOf(arguments, "-walkingBody");
        if (bodyArgument >= 0 && bodyArgument + 1 < arguments.Length) bodyType = arguments[bodyArgument + 1];
        var prefab = Resources.Load<GameObject>("CampaignTestEnemy");
        var roster = global.city[3].GetComponent<CampaignStage>();
        if (bodyType == "heavy") prefab = Array.Find(roster.enemyPool, p => p.GetComponent<CampaignEnemy>().style == CampaignEnemy.Style.Heavy);
        if (bodyType == "boss") prefab = roster.bossPrefab;
        if (!prefab) throw new Exception("Walking body prefab missing: " + bodyType);
        var actorGo = Instantiate(prefab, new Vector3(0,.15f,0), Quaternion.identity);
                var body = actorGo.GetComponent<Body>(); body.physicalWalking = !legacy;
        string profile = "default"; int profileArgument = Array.IndexOf(arguments, "-walkingProfile");
        if (profileArgument >= 0 && profileArgument + 1 < arguments.Length) profile = arguments[profileArgument + 1];
        if (profile == "fast") { body.walkStepTime = .22f; body.runStepTime = .12f; body.footSpring = 430; body.footDamping = 30; body.maxFootAcceleration = 550; }
        if (profile == "steady") { body.walkStepTime = .27f; body.runStepTime = .16f; }
        yield return null; yield return null;
        actorGo.GetComponent<MainHealth>().health = 5;
        var part = Array.Find(actorGo.GetComponentsInChildren<Health>(), h => h.GetComponent<Collider>());
        hero.GetComponent<ImpactTrigger>().SendMessage("OnTriggerEnter", part.GetComponent<Collider>());
        actorGo.GetComponent<MainHealth>().maxHealth = 100000; actorGo.GetComponent<MainHealth>().health = 100000;
        float deadline = Time.time + 7;
        while (!hero.GetComponent<Sword>().hasDoll) { if (Time.time > deadline) throw new Exception("Body possession did not finish: hand=" + body.handlerR.transform.position + " sword=" + hero.transform.position); yield return new WaitForFixedUpdate(); }
        var pelvis = body.upperLegL.GetComponent<Joint>().connectedBody;
        var camera = new GameObject("Walking review camera").AddComponent<Camera>();
        camera.orthographic = true; camera.orthographicSize = 1.6f; camera.nearClipPlane = .05f; camera.farClipPlane = 100;
        var texture = new RenderTexture(960, 720, 24); camera.targetTexture = texture;
        var photo = new Texture2D(960, 720, TextureFormat.RGB24, false);
        string mode = (legacy ? "legacy" : "tuned") + (bodyType == "fighter" ? "" : "-" + bodyType) + (profile == "default" ? "" : "-" + profile);
        string folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../Artifacts/Walking/" + mode));
        Directory.CreateDirectory(folder);
        var report = new Report { mode = mode, pelvis = pelvis.name,
            legLength = Vector3.Distance(body.upperLegL.transform.position, body.lowerLegL.transform.position) + Vector3.Distance(body.lowerLegL.transform.position, body.footL.transform.position) };
        foreach (var rb in actorGo.GetComponentsInChildren<Rigidbody>()) report.totalMass += rb.mass;
        Debug.Log("WALKING_RIG " + JsonUtility.ToJson(report));
        Debug.Log("WALKING_PROFILE spring=" + body.footSpring + " damping=" + body.footDamping + " walkTime=" + body.walkStepTime + " runTime=" + body.runStepTime);
        Debug.Log("WALKING_FEET footMass=" + body.footL.GetComponent<Rigidbody>().mass + " thighMass=" + body.upperLegL.GetComponent<Rigidbody>().mass + " shinMass=" + body.lowerLegL.GetComponent<Rigidbody>().mass + " trigger=" + body.footL.GetComponent<Collider>().isTrigger + " ignoresFloor=" + Physics.GetIgnoreLayerCollision(body.footL.layer, floor.layer));
        string[] names = { "walk", "run", "right_turn", "diagonal", "reverse", "stop" };
        Vector2[] controls = { Vector2.up * .55f, Vector2.up, Vector2.right, new Vector2(-.7f,.7f), Vector2.down, Vector2.zero };
        int frame = 0;
        for (int section = 0; section < names.Length; section++)
        {
            var stats = new Segment { name = names[section] }; int steps = body.WalkingSteps;
            SwordInput.TestAxes = controls[section];
            for (int tick = 0; tick < 100; tick++)
            {
                yield return new WaitForFixedUpdate();
                float speed = Vector3.ProjectOnPlane(pelvis.linearVelocity, Vector3.up).magnitude;
                Vector3 direction = Vector3.ProjectOnPlane(pelvis.linearVelocity, Vector3.up).normalized;
                float lag = Mathf.Max(Vector3.Dot(pelvis.position - body.footL.transform.position, direction), Vector3.Dot(pelvis.position - body.footR.transform.position, direction));
                if (Mathf.Min(body.footL.GetComponent<Collider>().bounds.min.y, body.footR.GetComponent<Collider>().bounds.min.y) < .04f) stats.groundContact++;
                stats.samples++; stats.averageSpeed += speed; stats.footLag += lag; stats.maxFootLag = Mathf.Max(stats.maxFootLag, lag);
                if (body.head.transform.position.y < report.legLength * 1.2f) stats.lowHeadFraction++;
                stats.footClearance += Mathf.Max(body.footL.transform.position.y, body.footR.transform.position.y);
                if (tick % 20 == 0)
                {
                    var center = pelvis.position + Vector3.up * .15f;
                    camera.transform.position = center + new Vector3(4, 1.8f, -5);
                    camera.transform.LookAt(center); camera.Render(); RenderTexture.active = texture;
                    photo.ReadPixels(new Rect(0,0,960,720),0,0); photo.Apply(); RenderTexture.active = null;
                    File.WriteAllBytes(Path.Combine(folder, names[section] + "-" + (frame++).ToString("D3") + ".jpg"), photo.EncodeToJPG(80));
                }
            }
            stats.groundContact /= stats.samples; stats.averageSpeed /= stats.samples; stats.footLag /= stats.samples; stats.lowHeadFraction /= stats.samples; stats.footClearance /= stats.samples;
            stats.steps = body.WalkingSteps - steps; report.segments.Add(stats); Debug.Log("WALKING_SEGMENT " + JsonUtility.ToJson(stats));
        }
        SwordInput.TestAxes = Vector2.zero;
        File.WriteAllText(Path.Combine(folder,"metrics.json"), JsonUtility.ToJson(report,true));
        Destroy(photo); Destroy(texture); Destroy(camera.gameObject);
    }
}
