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

    // Spawn decor items in each room 
    foreach (DecorEvaluator decorEvaluator in DecorEvaluator.Instances)
    {
      List<DecorItem> possibleItems = GetDifferentStyleItemsForRoom(decorEvaluator);
      List<DecorColor> possibleColors = GetDifferentColorsForRoom(decorEvaluator);
      DecorItemSpawner[] spawners = decorEvaluator.GetComponentsInChildren<DecorItemSpawner>();
      foreach (DecorItemSpawner spawner in spawners)
      {
        if (possibleItems.Count == 0)
        {
          possibleItems = GetDifferentStyleItemsForRoom(decorEvaluator);
        }

        DecorItem itemPrefab = possibleItems[Random.Range(0, possibleItems.Count)];
        DecorColor itemColor = possibleColors[Random.Range(0, possibleColors.Count)];
        possibleItems.Remove(itemPrefab);
        spawner.SpawnDecorItem(itemPrefab, itemColor);
      }
    }
  }

  private List<DecorItem> GetDifferentStyleItemsForRoom(DecorEvaluator decorEvaluator)
  {
    List<DecorItem> decorItems = new List<DecorItem>(decorList.DecorItems);
    for (int i = decorItems.Count - 1; i >= 0; --i)
    {
      DecorItem decorItem = decorItems[i];
      if (decorItem.Style == decorEvaluator.DesiredStyle)
      {
        decorItems.RemoveAt(i);
      }
    }

    return decorItems;
  }

  private List<DecorColor> GetDifferentColorsForRoom(DecorEvaluator decorEvaluator)
  {
    List<DecorColor> possibleColors = new List<DecorColor>(decorList.DecorColors);
    foreach (DecorColor decorColor in decorList.DecorColors)
    {
      if (decorEvaluator.DesiredColors.Contains(decorColor))
      {
        possibleColors.Remove(decorColor);
      }
    }

    return possibleColors;
  }

  private DecorStyle GetStyleFromPool()
  {
    int index = Random.Range(0, availableStyles.Count);
    DecorStyle chosenStyle = availableStyles[index];
    availableStyles.RemoveAt(index);
    return chosenStyle;
  }

  private DecorColor GetColorFromPool()
  {
    int index = Random.Range(0, availableColors.Count);
    DecorColor chosenColor = availableColors[index];
    availableColors.RemoveAt(index);
    return chosenColor;
  }
}