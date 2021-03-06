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

  [SerializeField]
  private DecorList decorList = null;

  private int evaluateIndex;
  private int runningScore;
  private Bounds bounds;

  private static List<DecorEvaluator> instances = new List<DecorEvaluator>();

  public static void EvaluateDroppedItem(DecorItem decorItem)
  {
    foreach (DecorEvaluator decorEvaluator in Instances)
    {
      if (decorEvaluator.ContainsItem(decorItem))
      {
        int score = decorEvaluator.CalculateItemScore(decorItem);
        if (score > 0)
        {
          AudioManager.Instance.PlaySound(decorEvaluator.decorList.CorrectSound);
          GameObject fxPrefab = score == 1 ? decorEvaluator.decorList.FxCorrect1Prefab : decorEvaluator.decorList.FxCorrect2Prefab;
          GameObject fx = Instantiate(fxPrefab, decorItem.transform.position, Quaternion.identity);
          fx.transform.forward = Vector3.up;
        }
        else
        {
          GameObject fxPrefab = decorEvaluator.decorList.FxIncorrectPrefab;
          GameObject fx = Instantiate(fxPrefab, decorItem.transform.position, Quaternion.identity);
          AudioManager.Instance.PlaySound(decorEvaluator.decorList.IncorrectSound);
          fx.transform.forward = Vector3.up;
        }

        CommentaryManager.Instance.CommentaryReaction();
      }
    }
  }

  private void Awake()
  {
    instances.Add(this);

    bounds = new Bounds(boundsCornerA.position, Vector3.zero);
    bounds.Encapsulate(boundsCornerB.position);
  }

  private void OnDestroy()
  {
    instances.Remove(this);
  }

  private void Update()
  {
    if (evaluateIndex < DecorItem.Instances.Count)
    {
      DecorItem decorItem = DecorItem.Instances[evaluateIndex];
      if (ContainsItem(decorItem))
      {
        int itemScore = CalculateItemScore(decorItem);
        runningScore += itemScore;
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

  public bool ContainsItem(DecorItem decorItem)
  {
    return bounds.Contains(decorItem.transform.position);
  }

  public int CalculateTotalScore()
  {
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

  public int CalculateItemScore(DecorItem decorItem)
  {
    bool hasStyle = decorItem.Style == DesiredStyle;
    bool hasColor = DesiredColors.Contains(decorItem.Color);

    // Calculate a score based on how the style matches desired 
    if (hasStyle && hasColor)
      return 3;
    else if (hasStyle != hasColor)
      return 1;

    return -1;
  }
}