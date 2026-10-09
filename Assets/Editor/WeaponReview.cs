using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class WeaponReview
{
 public static void Run()
 {
  Directory.CreateDirectory("Review");
  var report = new StringBuilder("# Оружие оригинального SwordHeroDolls\n\nИсточник: игровые префабы; просмотр в редакторе, без изменения баланса.\n\n");
  foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Player"}))
  {
   var path=AssetDatabase.GUIDToAssetPath(guid); var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
   var hero=prefab.GetComponent<SwordHero>(); if(!hero) continue;
   var sword=prefab.GetComponent<Sword>(); var hit=prefab.GetComponent<ImpactTrigger>(); if(!sword || !hit) continue;
   report.AppendLine("- **"+prefab.name+"**: урон "+hit.minDamage+"–"+hit.maxDamage+", скорость "+sword.maxSpeed+", двуручное: "+sword.twoHanded+". Следующие варианты: "+string.Join(", ",hero.evoSwords.Where(x=>x).Select(x=>x.GetComponent<SwordEvo>() && x.GetComponent<SwordEvo>().newSword ? x.GetComponent<SwordEvo>().newSword.name : x.name + " (missing SwordEvo/newSword)"))+".");
  }
  File.WriteAllText("Review/weapons.md",report.ToString());
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var light=new GameObject("Review light").AddComponent<Light>(); light.type=LightType.Directional; light.intensity=1.5f; light.transform.rotation=Quaternion.Euler(40,-30,0);
  RenderSettings.ambientLight=new Color(.7f,.7f,.7f);
  var camera=new GameObject("Review camera").AddComponent<Camera>(); camera.orthographic=true;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.12f,.15f,.19f);
  var sheet=new Texture2D(1080,720,TextureFormat.RGB24,false);
  string[] paths={"Dagger01(E0).prefab","E4/Sword01(E4) Variant.prefab","E4/SwordTH02.prefab","E4/Axe02(E4).prefab","E4/Spear03(E4).prefab","E4/Dagger07L(E4).prefab"};
  for(int i=0;i<paths.Length;i++)
  {
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/Player/"+paths[i]); var instance=UnityEngine.Object.Instantiate(prefab);
   foreach(var t in instance.GetComponentsInChildren<Transform>(true)) t.gameObject.layer=0;
   foreach(var p in instance.GetComponentsInChildren<ParticleSystemRenderer>(true))p.enabled=false;
   var renderers=instance.GetComponentsInChildren<Renderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy&&!(r is ParticleSystemRenderer)).ToArray();
   var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
   camera.transform.position=bounds.center+new Vector3(2,1.1f,-3).normalized*6;camera.transform.LookAt(bounds.center);camera.orthographicSize=Mathf.Max(bounds.size.magnitude*.6f,.3f);
   var rt=new RenderTexture(360,360,24);camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;
   var frame=new Texture2D(360,360,TextureFormat.RGB24,false);frame.ReadPixels(new Rect(0,0,360,360),0,0);frame.Apply();
   sheet.SetPixels((i%3)*360,(1-i/3)*360,360,360,frame.GetPixels());
   camera.targetTexture=null;RenderTexture.active=null;UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(frame);UnityEngine.Object.DestroyImmediate(instance);
  }
  sheet.Apply();File.WriteAllBytes("Review/weapon-models.png",sheet.EncodeToPNG());UnityEngine.Object.DestroyImmediate(sheet);
  Debug.Log("WEAPON_REVIEW_PASS");
 }
}
