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
      yield return new WaitForSeconds(slideTiming);
      slideShowItem.gameObject.SetActive(false);
    }
  }
}