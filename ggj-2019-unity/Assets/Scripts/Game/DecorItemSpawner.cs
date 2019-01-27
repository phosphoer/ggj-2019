using UnityEngine;

public class DecorItemSpawner : MonoBehaviour
{
  public void SpawnDecorItem(DecorItem itemPrefab, DecorColor color)
  {
    DecorItem decorItem = Instantiate(itemPrefab, null);
    decorItem.Color = color;
    decorItem.transform.SetPositionAndRotation(transform.position, transform.rotation);
    decorItem.transform.localScale = Vector3.one;
  }
}