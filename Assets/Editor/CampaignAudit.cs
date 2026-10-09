using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class CampaignAudit
{
 public static void Run()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
  var global=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();
  Debug.Log("AUDIT_GLOBAL city="+string.Join("|",global.city.Select(x=>x?AssetDatabase.GetAssetPath(x):"null"))+" runes="+global.runes.Count+" levelUp="+AssetDatabase.GetAssetPath(global.levelUpGO));
  foreach(var city in Object.FindObjectsByType<City>(FindObjectsSortMode.None))Debug.Log("AUDIT_CITY "+city.name+" level="+city.level+" enemies="+string.Join("|",city.enemies.Select(x=>AssetDatabase.GetAssetPath(x)))+" houses="+city.houses.Length);
  foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Dolls"}))
  {
   var path=AssetDatabase.GUIDToAssetPath(guid);var go=AssetDatabase.LoadAssetAtPath<GameObject>(path);var body=go.GetComponent<Body>();var health=go.GetComponent<MainHealth>();if(!body||!health)continue;
   var spawn=go.GetComponentInChildren<SwordSpawner>(true);
   Debug.Log("AUDIT_ENEMY "+path+" health="+health.maxHealth+" power="+(go.GetComponent<EnemyPower>()?go.GetComponent<EnemyPower>().power:0)+" weapons="+(spawn?string.Join("|",spawn.mainWeapons.Select(x=>AssetDatabase.GetAssetPath(x))):"none"));
  }
  foreach(var rune in global.runes)Debug.Log("AUDIT_RUNE "+AssetDatabase.GetAssetPath(rune));
 }
}
