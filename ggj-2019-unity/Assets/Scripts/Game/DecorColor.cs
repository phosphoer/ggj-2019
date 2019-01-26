using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Decor Color", fileName = "new-decor-color")]
public class DecorColor : ScriptableObject
{
  public string Name = "A Cool Color";
  public Color Color = Color.white;
}