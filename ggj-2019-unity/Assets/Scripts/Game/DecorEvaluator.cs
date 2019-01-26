using UnityEngine;

public class DecorEvaluator : MonoBehaviour
{
  [SerializeField]
  private Transform boundsCornerA = null;

  [SerializeField]
  private Transform boundsCornerB = null;

  private int evaluateIndex;

  private void Update()
  {
    if (evaluateIndex < DecorItem.Instances.Count)
    {
      Bounds bounds = new Bounds(boundsCornerA.position, Vector3.zero);
      bounds.Encapsulate(boundsCornerB.position);

      DecorItem decorItem = DecorItem.Instances[evaluateIndex];
      if (bounds.Contains(decorItem.transform.position))
      {
      }
    }

    if (++evaluateIndex >= DecorItem.Instances.Count)
    {
      evaluateIndex = 0;
    }
  }

  private void OnDrawGizmos()
  {
    if (boundsCornerA != null && boundsCornerB != null)
    {
      Bounds bounds = new Bounds(boundsCornerA.position, Vector3.zero);
      bounds.Encapsulate(boundsCornerB.position);

      Gizmos.color = Color.white;
      Gizmos.DrawSphere(boundsCornerA.position, 0.5f);
      Gizmos.DrawSphere(boundsCornerB.position, 0.5f);
      Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
  }
}