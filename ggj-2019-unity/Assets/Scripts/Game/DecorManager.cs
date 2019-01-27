using UnityEngine;
using System.Collections.Generic;

public class DecorManager : MonoBehaviour
{
  [SerializeField]
  private DecorList decorList = null;

  private List<DecorStyle> availableStyles = new List<DecorStyle>();
  private List<DecorColor> availableColors = new List<DecorColor>();

  private void Start()
  {
    // Fill pools of items
    availableStyles.AddRange(decorList.DecorStyles);
    availableColors.AddRange(decorList.DecorColors);

    // Assign a style and color to each of the rooms
    foreach (DecorEvaluator decorEvaluator in DecorEvaluator.Instances)
    {
      decorEvaluator.DesiredStyle = GetStyleFromPool();
      decorEvaluator.DesiredColors.Add(GetColorFromPool());
    }

    // Add a color from another room to each room, to encourage competitivity
    for (int i = 0; i < DecorEvaluator.Instances.Count; ++i)
    {
      int anotherRoomIndex = (i + 1) % DecorEvaluator.Instances.Count;
      DecorEvaluator thisRoom = DecorEvaluator.Instances[i];
      DecorEvaluator anotherRoom = DecorEvaluator.Instances[anotherRoomIndex];
      thisRoom.DesiredColors.Add(anotherRoom.DesiredColors[0]);
    }
  }

  public DecorStyle GetStyleFromPool()
  {
    int index = Random.Range(0, availableStyles.Count);
    DecorStyle chosenStyle = availableStyles[index];
    availableStyles.RemoveAt(index);
    return chosenStyle;
  }

  public DecorColor GetColorFromPool()
  {
    int index = Random.Range(0, availableColors.Count);
    DecorColor chosenColor = availableColors[index];
    availableColors.RemoveAt(index);
    return chosenColor;
  }
}