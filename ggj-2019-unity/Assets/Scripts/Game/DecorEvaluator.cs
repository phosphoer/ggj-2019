using UnityEngine;
using System.Collections.Generic;

public class DecorEvaluator : MonoBehaviour
{
  public static IReadOnlyList<DecorEvaluator> Instances => instances;

  public DecorStyle DesiredStyle { get; set; }
  public List<DecorColor> DesiredColors { get; set; } = new List<DecorColor>();

  [SerializeField]
  private Transform boundsCornerA = null;

  [SerializeField]
  private Transform boundsCornerB = null;

  private int evaluateIndex;

  private static List<DecorEvaluator> instances = new List<DecorEvaluator>();

  private void Awake()
  {
    instances.Add(this);
  }

  private void OnDestroy()
  {
    instances.Remove(this);
  }

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