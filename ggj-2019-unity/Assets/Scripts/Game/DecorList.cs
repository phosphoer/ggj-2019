using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Decor List", fileName = "DecorList")]
public class DecorList : ScriptableObject
{
  public DecorStyle[] DecorStyles;
  public DecorColor[] DecorColors;
  public DecorItem[] DecorItems;
  public SoundBank CorrectSound;
  public SoundBank IncorrectSound;
  public GameObject FxCorrect1Prefab;
  public GameObject FxCorrect2Prefab;
  public GameObject FxIncorrectPrefab;
}