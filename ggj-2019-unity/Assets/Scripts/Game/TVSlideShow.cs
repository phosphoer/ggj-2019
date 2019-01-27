using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TVSlideShow : MonoBehaviour
{
  [SerializeField]
  private Transform slideShowRoot = null;

  [SerializeField]
  private float slideTiming = 3.0f;

  [SerializeField]
  private TMP_Text styleNameText = null;

  [SerializeField]
  private TMP_Text color1NameText = null;

  [SerializeField]
  private TMP_Text color2NameText = null;

  private void Awake()
  {
    foreach (Transform slideShowItem in slideShowRoot)
    {
      slideShowItem.gameObject.SetActive(false);
    }
  }

  public Coroutine StartSlideShow()
  {
    DecorEvaluator parentRoom = GetComponentInParent<DecorEvaluator>();
    if (parentRoom != null)
    {
      styleNameText.text = parentRoom.DesiredStyle.Name;
      color1NameText.text = parentRoom.DesiredColors[0].Name;
      color2NameText.text = parentRoom.DesiredColors[1].Name;
      color1NameText.color = parentRoom.DesiredColors[0].Color;
      color2NameText.color = parentRoom.DesiredColors[1].Color;
    }
    else
    {
      Debug.LogError("TV was not under a decor evaluator", this);
    }

    return StartCoroutine(SlideShowRoutine());
  }

  private IEnumerator SlideShowRoutine()
  {
    foreach (Transform slideShowItem in slideShowRoot)
    {
      slideShowItem.gameObject.SetActive(true);

      for (float timer = 0; timer < slideTiming; timer += Time.deltaTime)
      {
        if (PlayerManager.Instance.IsAnyPlayerPressingButton())
        {
          timer = slideTiming;
        }

        yield return null;
      }

      slideShowItem.gameObject.SetActive(false);
    }
  }
}