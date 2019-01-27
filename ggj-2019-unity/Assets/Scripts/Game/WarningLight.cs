using UnityEngine;
using System.Collections.Generic;

public class WarningLight : MonoBehaviour
{
  public static IReadOnlyList<WarningLight> Instances { get; private set; }

  private static List<WarningLight> instances = new List<WarningLight>();

  public static void TurnOn()
  {
    foreach (WarningLight light in instances)
    {
      light.gameObject.SetActive(true);
    }
  }

  public static void TurnOff()
  {
    foreach (WarningLight light in instances)
    {
      light.gameObject.SetActive(false);
    }
  }

  private void Awake()
  {
    instances.Add(this);
    gameObject.SetActive(false);
  }

  private void OnDestroy()
  {
    instances.Remove(this);
  }
}