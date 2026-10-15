using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public sealed class CampaignHUD : MonoBehaviour
{
 public static CampaignHUD Active {get;private set;}
 public Font font;
 public GameObject joystickPrefab;
 public bool Victory {get;private set;}
 public bool TestMode;
 AxisChoice endChoice=new AxisChoice();
 void Awake(){Active=this;CampaignMenu.Reset();}
 void Start()
 {
  var canvas=new GameObject("Управление стиком",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
  canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;canvas.GetComponent<Canvas>().sortingOrder=120;
  var scale=canvas.GetComponent<CanvasScaler>();scale.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scale.referenceResolution=new Vector2(1280,720);
  foreach(var old in FindObjectsByType<VariableJoystick>(FindObjectsSortMode.None))old.gameObject.SetActive(false);
  if(joystickPrefab && Application.isMobilePlatform)
  {

   var joystick=Instantiate(joystickPrefab,canvas.transform);joystick.SetActive(true);
   var rect=joystick.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=Vector2.zero;rect.pivot=Vector2.one*.5f;rect.anchoredPosition=new Vector2(125,125);rect.sizeDelta=new Vector2(170,170);
  }
 }
 void Update()
 {
  if(Victory){if(endChoice.Step(SwordInput.Read(),1)>=0)Restart();return;}
  var player=GameObject.FindGameObjectWithTag("Player");if(!player)return;var hero=player.GetComponent<SwordHero>();var sword=player.GetComponent<Sword>();
  if(hero&&sword&&!sword.InUpgradeMenu&&CampaignMenu.OpenCount==0&&hero.levelUpRunes>0)hero.CheckLevelUp();
 }
 public void Win(){Victory=true;Time.timeScale=0;Debug.Log("CAMPAIGN_VICTORY five rounds");}
 public void Restart(){CampaignMenu.Reset();SceneManager.LoadScene(SceneManager.GetActiveScene().name);}
 void OnGUI()
 {
  if(font)GUI.skin.font=font;
  GUI.Label(new Rect(Screen.width-310,Screen.height-65,300,25),SwordInput.TestEnabled?"АВТОПРОВЕРКА":"SwordDolls — версия "+Application.version,new GUIStyle(GUI.skin.label){fontSize=14,alignment=TextAnchor.MiddleRight});
  if(Victory){if(CampaignMenu.Draw("ПОБЕДА — ПЯТЬ ЭТАПОВ ПРОЙДЕНЫ",new[]{"Новый забег"},new[]{"Начать с простого клинка и собрать другое сочетание улучшений."},0)==0)Restart();return;}
  if(CampaignMenu.OpenCount>0)return;
  var global=GameObject.Find("GlobalParameters");if(!global)return;var city=global.GetComponent<GlobalParameters>().nowCity;if(!city)return;var stage=city.GetComponent<CampaignStage>();if(!stage)return;
  var style=new GUIStyle(GUI.skin.box){fontSize=20,alignment=TextAnchor.MiddleLeft};
  GUI.Box(new Rect(15,15,500,55),"  Этап "+stage.number+" / 5 · "+stage.title,style);
  GUI.Box(new Rect(15,125,500,40),"  Противников осталось: "+city.GetComponent<City>().enemies.Count+" / "+stage.PlannedCount,style);
  var hero=FindFirstObjectByType<SwordHero>();if(hero)GUI.Box(new Rect(15,75,500,45),"  Герой "+hero.level+" · Опыт "+hero.experience+" / "+hero.level*10,style);
  var playerObject=GameObject.FindGameObjectWithTag("Player");var combat=playerObject?playerObject.GetComponent<HeroCombat>():null;var bodyHealth=combat?combat.BodyHealth:null;if(bodyHealth){var shield=bodyHealth.GetComponent<BodyShield>();GUI.Box(new Rect(15,170,500,40),"  HP "+Mathf.CeilToInt(bodyHealth.health)+" / "+bodyHealth.maxHealth+"  Щит "+(shield?Mathf.CeilToInt(shield.Current):0),style);}
  if(stage.Boss&&stage.Boss.GetComponent<MainHealth>())
  {
   var health=stage.Boss.GetComponent<MainHealth>();GUI.Box(new Rect(Screen.width*.3f,15,Screen.width*.4f,55),stage.Boss.displayName+" · "+Mathf.CeilToInt(health.health)+" / "+health.maxHealth+" · Фаза "+stage.Boss.Phase,style);
  }
  GUI.Label(new Rect(20,Screen.height-35,900,30),"WASD / стик — оружие · Карточки за опыт · Кузница после этапов 2 и 4",new GUIStyle(GUI.skin.label){fontSize=18});
 }
}
