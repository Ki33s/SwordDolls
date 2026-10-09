using UnityEngine;
public static class SwordInput
{
 public static bool TestEnabled;
 public static Vector2 TestAxes;
 public static Vector2 Read()
 {
  if(TestEnabled)return TestAxes;
  Vector2 keys=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
  Vector2 keyboard=new Vector2((Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)?1:0)-(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)?1:0),(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow)?1:0)-(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)?1:0));
  if(keyboard.sqrMagnitude>0)keys=keyboard;
  var joystick=Object.FindFirstObjectByType<VariableJoystick>();
  return joystick&&joystick.Direction.sqrMagnitude>.01f?joystick.Direction:keys;
 }
}
