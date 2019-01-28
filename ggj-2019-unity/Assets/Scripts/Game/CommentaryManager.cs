using UnityEngine;
using System.Collections;

public class CommentaryManager : MonoBehaviour
{
  public static CommentaryManager Instance { get; private set; }

  [SerializeField]
  private SoundBank genericSoundByte = null;

  [SerializeField]
  private SoundBank slapSoundByte = null;

  [SerializeField]
  private SoundBank goodDecorSoundByte = null;

  [SerializeField]
  private SoundBank badDecorSoundByte = null;

  [SerializeField]
  private SoundBank timeWarningSoundByte = null;

  [SerializeField]
  private SoundBank timesUpSoundByte = null;

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

  public void CommentarySlap()
  {
    MaybeDoCommentary(slapSoundByte);
  }

  public void CommentaryGoodDecor()
  {
    MaybeDoCommentary(goodDecorSoundByte);
  }

  public void CommentaryBadDecor()
  {
    MaybeDoCommentary(badDecorSoundByte);
  }

  public void CommentaryTimeWarning()
  {
    MaybeDoCommentary(timeWarningSoundByte, 1);
  }

  public void CommentaryTimesUp()
  {
    MaybeDoCommentary(timesUpSoundByte, 1);
  }

  private void MaybeDoCommentary(SoundBank soundByte, float chance = 0.1f)
  {
    if (soundByte != null && Random.value < chance)
    {
      AudioManager.Instance.PlaySound(soundByte);
    }
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