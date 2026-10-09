using UnityEngine;
public static class CampaignMenu
{
 public static int OpenCount { get; private set; }
 public static void Open(){OpenCount++;Time.timeScale=0;}
 public static void Close(){OpenCount=Mathf.Max(0,OpenCount-1);if(OpenCount==0&&!(CampaignHUD.Active&&CampaignHUD.Active.Victory))Time.timeScale=1;}
 public static void Reset(){OpenCount=0;Time.timeScale=1;}
 public static int Draw(string heading,string[] titles,string[] descriptions,int selected,System.Action<int> focus=null,bool canChoose=true)
 {
  var previous=GUI.matrix;GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/1280f,Screen.height/720f,1));
  if(CampaignHUD.Active&&CampaignHUD.Active.font)GUI.skin.font=CampaignHUD.Active.font;
  GUI.color=new Color(.04f,.06f,.09f,.97f);GUI.DrawTexture(new Rect(0,0,1280,720),Texture2D.whiteTexture);GUI.color=Color.white;
  var header=new GUIStyle(GUI.skin.label){fontSize=32,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(80,60,1120,60),heading,header);
  GUI.Label(new Rect(100,130,1080,40),"A/D или стик ←/→ — выбор · W или стик ↑ — подтвердить · Мышь — наведение и клик",new GUIStyle(GUI.skin.label){fontSize=19,alignment=TextAnchor.MiddleCenter});
  int chosen=-1;float width=330,gap=25;float x=(1280-titles.Length*width-(titles.Length-1)*gap)/2;
  for(int i=0;i<titles.Length;i++)
  {
   var rect=new Rect(x+i*(width+gap),215,width,350);
   Vector2 pointer=new Vector2(Input.mousePosition.x*1280f/Screen.width,(Screen.height-Input.mousePosition.y)*720f/Screen.height);
   if(focus!=null && (Mathf.Abs(Input.GetAxisRaw("Mouse X"))>.01f || Mathf.Abs(Input.GetAxisRaw("Mouse Y"))>.01f) && rect.Contains(pointer)){focus(i);selected=i;}
   GUI.color=i==selected?new Color(.93f,.68f,.25f):new Color(.22f,.28f,.36f);GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=Color.white;
   var inner=new Rect(rect.x+5,rect.y+5,rect.width-10,rect.height-10);GUI.color=new Color(.08f,.11f,.16f);GUI.DrawTexture(inner,Texture2D.whiteTexture);GUI.color=Color.white;
   GUI.Label(new Rect(rect.x+20,rect.y+30,width-40,100),titles[i],new GUIStyle(GUI.skin.label){fontSize=27,wordWrap=true,alignment=TextAnchor.MiddleCenter});
   GUI.Label(new Rect(rect.x+25,rect.y+145,width-50,145),descriptions[i],new GUIStyle(GUI.skin.label){fontSize=21,wordWrap=true,alignment=TextAnchor.MiddleCenter});
   GUI.enabled=canChoose; if(GUI.Button(new Rect(rect.x+25,rect.y+290,width-50,42),i==selected?"ВЫБРАТЬ ↑":"Выбрать"))chosen=i;GUI.enabled=true;
  }
  GUI.Label(new Rect(120,605,1040,50),canChoose?"Отпусти ось перед подтверждением следующего выбора":"Выбор доступен через секунду — отпусти управление",new GUIStyle(GUI.skin.label){fontSize=18,alignment=TextAnchor.MiddleCenter});
  GUI.matrix=previous;return chosen;
 }
}
