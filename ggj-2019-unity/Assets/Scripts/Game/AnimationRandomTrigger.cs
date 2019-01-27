using UnityEngine;
using System.Collections;

public class AnimationRandomTrigger : MonoBehaviour
{
  [SerializeField]
  private Animator animator = null;

  [SerializeField]
  private string[] animationTriggers = null;

  [SerializeField]
  private RangedFloat triggerTimeRange = new RangedFloat(3, 6);

  private void OnEnable()
  {
    StartCoroutine(AnimationRoutine());
  }

  private IEnumerator AnimationRoutine()
  {
    while (enabled)
    {
      animator.SetTrigger(animationTriggers[Random.Range(0, animationTriggers.Length)]);
      yield return new WaitForSeconds(triggerTimeRange.RandomValue);
    }
  }
}