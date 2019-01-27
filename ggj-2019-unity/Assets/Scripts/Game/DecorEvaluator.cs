using UnityEngine;
using System.Collections.Generic;

public class DecorEvaluator : MonoBehaviour
{
  public static IReadOnlyList<DecorEvaluator> Instances => instances;

  public DecorStyle DesiredStyle { get; set; }
  public List<DecorColor> DesiredColors { get; set; } = new List<DecorColor>();
  public int RunningScore => runningScore;

  [SerializeField]
  private Transform boundsCornerA = null;

  [SerializeField]
  private Transform boundsCornerB = null;

  private int evaluateIndex;
  private int runningScore;

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
        runningScore += CalculateItemScore(decorItem);
      }
    }

    if (++evaluateIndex >= DecorItem.Instances.Count)
    {
      evaluateIndex = 0;
      runningScore = 0;
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

  public int CalculateTotalScore()
  {
    Bounds bounds = new Bounds(boundsCornerA.position, Vector3.zero);
    bounds.Encapsulate(boundsCornerB.position);

    int totalScore = 0;
    foreach (DecorItem decorItem in DecorItem.Instances)
    {
      if (bounds.Contains(decorItem.transform.position))
      {
        totalScore += CalculateItemScore(decorItem);
      }
    }

    return totalScore;
  }

  private int CalculateItemScore(DecorItem decorItem)
  {
    bool hasStyle = decorItem.Style == DesiredStyle;
    bool hasColor = DesiredColors.Contains(decorItem.Color);

    // Calculate a score based on how the style matches desired 
    if (hasStyle && hasColor)
      return 3;
    else if (hasStyle && !hasColor)
      return 2;
    else if (!hasStyle && hasColor)
      return 1;

    return -1;
  }
}