using UnityEngine;
public sealed class AxisChoice
{
 public int Selected { get; private set; }
 public void Focus(int index){Selected=index;}
 bool neutral, horizontalHeld, verticalHeld;
 float repeatAt;
 public int Step(Vector2 axis,int count)
 {
  if(count==0)return -1;
  if(!neutral){if(axis.magnitude<.25f)neutral=true;return -1;}
  if(Mathf.Abs(axis.x)<.25f)horizontalHeld=false;
  if(Mathf.Abs(axis.y)<.25f)verticalHeld=false;
  if(Mathf.Abs(axis.x)>.55f&&(!horizontalHeld||Time.unscaledTime>=repeatAt))
  {
   Selected=(Selected+(axis.x>0?1:count-1))%count;
   repeatAt=Time.unscaledTime+(horizontalHeld?.16f:.4f);horizontalHeld=true;
  }
  if(axis.y>.55f&&!verticalHeld){verticalHeld=true;return Selected;}
  return -1;
 }
}
