using UnityEngine;
using System.Collections.Generic;

public class DecorItem : MonoBehaviour
{
  public static IReadOnlyList<DecorItem> Instances => instances;

  public DecorColor Color
  {
    get { return color; }
    set
    {
      color = value;
      UpdateMaterial();
    }
  }

  public DecorStyle Style => style;

  [SerializeField]
  private DecorStyle style = null;

  [SerializeField]
  private DecorColor color = null;

  [SerializeField]
  private new Renderer renderer = null;

  private Material materialInstance;

  private static List<DecorItem> instances = new List<DecorItem>();

  private void Awake()
  {
    UpdateMaterial();
  }

  private void Start()
  {
    instances.Add(this);
  }

  private void OnDestroy()
  {
    instances.Remove(this);
  }

  private void UpdateMaterial()
  {
    if (materialInstance == null)
    {
      materialInstance = Instantiate(renderer.sharedMaterial);
      renderer.sharedMaterial = materialInstance;
    }

    materialInstance.SetColor("_ReplaceWithColor", color.Color);
  }
}