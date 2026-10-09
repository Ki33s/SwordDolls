using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class EnemyReview
{
 public static void ReviewAndBuild(){Run();RecoverySetup.Build();}
 public static void Run()
 {
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var light=new GameObject("Review light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.5f;light.transform.rotation=Quaternion.Euler(40,-30,0);RenderSettings.ambientLight=Color.gray;
  var camera=new GameObject("Review camera").AddComponent<Camera>();camera.orthographic=true;camera.backgroundColor=new Color(.12f,.15f,.19f);camera.clearFlags=CameraClearFlags.SolidColor;
  const int width=350,height=380;var sheet=new Texture2D(width*7,height*3,TextureFormat.RGB24,false);
  for(int i=0;i<21;i++)
  {
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(i<15?"Assets/Campaign/Enemies/Roster/Type"+i+".prefab":"Assets/Campaign/Expanded/Enemy"+(i-15)+".prefab");var actor=UnityEngine.Object.Instantiate(prefab);
   var spawner=actor.GetComponentInChildren<SwordSpawner>();var weapon=UnityEngine.Object.Instantiate(spawner.mainWeapons[0],spawner.transform);weapon.transform.localPosition=Vector3.zero;weapon.transform.localRotation=Quaternion.identity;
   if(spawner.weaponLSpawnProb==1)UnityEngine.Object.Instantiate(spawner.leftWeapons[0],actor.GetComponent<Body>().handlerL.transform);
   foreach(var t in actor.GetComponentsInChildren<Transform>(true))t.gameObject.layer=0;
   foreach(var p in actor.GetComponentsInChildren<ParticleSystemRenderer>(true))p.enabled=false;
   var renderers=actor.GetComponentsInChildren<Renderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy&&!(r is ParticleSystemRenderer)).ToArray();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
   camera.transform.position=bounds.center+new Vector3(2,1,5).normalized*8;camera.transform.LookAt(bounds.center);camera.orthographicSize=Mathf.Max(bounds.size.y*.65f,bounds.size.x*.6f);
   var rt=new RenderTexture(width,height,24);camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;var frame=new Texture2D(width,height,TextureFormat.RGB24,false);frame.ReadPixels(new Rect(0,0,width,height),0,0);frame.Apply();sheet.SetPixels((i%7)*width,(2-i/7)*height,width,height,frame.GetPixels());
   camera.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(frame);UnityEngine.Object.DestroyImmediate(actor);
  }
  sheet.Apply();Directory.CreateDirectory("Review");File.WriteAllBytes("Review/enemy-roster.png",sheet.EncodeToPNG());UnityEngine.Object.DestroyImmediate(sheet);Debug.Log("ENEMY_REVIEW_PASS twenty-one silhouettes and weapons");
 }
}
