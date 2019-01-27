using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TVSlideShow : MonoBehaviour
{
  public static IReadOnlyList<TVSlideShow> Instances => instances;

  [SerializeField]
  private Transform slideShowIntro = null;

  [SerializeField]
  private Transform slideShowOutro = null;

  [SerializeField]
  private float slideTiming = 3.0f;

  [SerializeField]
  private TMP_Text styleNameText = null;

  [SerializeField]
  private TMP_Text color1NameText = null;

  [SerializeField]
  private TMP_Text color2NameText = null;

  [SerializeField]
  private TMP_Text scoreText = null;

  [SerializeField]
  private GameObject styleSlide = null;

  private static List<TVSlideShow> instances = new List<TVSlideShow>();

  private void Awake()
  {
    instances.Add(this);

    foreach (Transform slideShowItem in slideShowIntro)
    {
      slideShowItem.gameObject.SetActive(false);
    }

    foreach (Transform slideShowItem in slideShowOutro)
    {
      slideShowItem.gameObject.SetActive(false);
    }

    gameObject.SetActive(false);
  }

  public Coroutine StartIntroSlides()
  {
    gameObject.SetActive(true);

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

    return StartCoroutine(SlideShowRoutine(slideShowIntro));
  }

  public Coroutine StartOutroSlides()
  {
    styleSlide.SetActive(false);
    DecorEvaluator parentRoom = GetComponentInParent<DecorEvaluator>();
    if (parentRoom != null)
    {
      scoreText.text = parentRoom.CalculateTotalScore().ToString();
    }
    else
    {
      Debug.LogError("TV was not under a decor evaluator", this);
    }

    return StartCoroutine(SlideShowRoutine(slideShowOutro));
  }

  private IEnumerator SlideShowRoutine(Transform slidesRoot)
  {
    foreach (Transform slideShowItem in slidesRoot)
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

    if (slidesRoot == slideShowIntro)
    {
      styleSlide.SetActive(true);
    }
  }
}