using UnityEngine;
using System.Collections;

public class CommentaryManager : MonoBehaviour
{
  public static CommentaryManager Instance { get; private set; }

  [SerializeField]
  private SoundBank introSoundByte = null;

  [SerializeField]
  private SoundBank reactionSoundByte = null;

  [SerializeField]
  private SoundBank timeWarningSoundByte = null;

  [SerializeField]
  private SoundBank timesUpSoundByte = null;

  private Coroutine commentaryRoutine;
  private float lastReactionTime;

  private void Awake()
  {
    Instance = this;
  }

  public void CommentaryIntro()
  {
    AudioManager.Instance.PlaySound(introSoundByte);
  }

  public void CommentaryReaction()
  {
    if (Random.value < 0.1 && Time.time > lastReactionTime + 10)
    {
      lastReactionTime = Time.time;
      AudioManager.Instance.PlaySound(reactionSoundByte);
    }
  }

  public void CommentaryTimeWarning()
  {
    AudioManager.Instance.PlaySound(timeWarningSoundByte);
  }

  public void CommentaryTimesUp()
  {
    AudioManager.Instance.PlaySound(timesUpSoundByte);
  }
}