using UnityEngine;
using System.Collections;

public class CommentaryManager : MonoBehaviour
{
  public static CommentaryManager Instance { get; private set; }

  [SerializeField]
  private SoundBank genericSoundByte = null;

  [SerializeField]
  private RangedFloat genericCommentaryTiming = new RangedFloat(10, 20);

  private Coroutine commentaryRoutine;

  private void Awake()
  {
    Instance = this;
  }

  public void StartCommentary()
  {
    commentaryRoutine = StartCoroutine(CommentaryRoutine());
  }

  public void StopCommentary()
  {
    StopCoroutine(commentaryRoutine);
  }

  private IEnumerator CommentaryRoutine()
  {
    while (enabled)
    {
      yield return new WaitForSeconds(genericCommentaryTiming.RandomValue);

      if (genericSoundByte != null)
      {
        AudioManager.Instance.PlaySound(genericSoundByte);
      }
    }
  }
}