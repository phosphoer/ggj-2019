using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TVSlideShow : MonoBehaviour
{
  [SerializeField]
  private Transform slideShowRoot = null;

  [SerializeField]
  private float slideTiming = 3.0f;

  private void Awake()
  {
    foreach (Transform slideShowItem in slideShowRoot)
    {
      slideShowItem.gameObject.SetActive(false);
    }
  }

  public Coroutine StartSlideShow()
  {
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