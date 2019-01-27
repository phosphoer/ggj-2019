using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Decor List", fileName = "DecorList")]
public class DecorList : ScriptableObject
{
  public DecorStyle[] DecorStyles;
  public DecorColor[] DecorColors;
  public DecorItem[] DecorItems;
}