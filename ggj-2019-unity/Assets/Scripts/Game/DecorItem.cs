using UnityEngine;

public class DecorItem : MonoBehaviour
{
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

  private void Awake()
  {
    UpdateMaterial();
  }

  private void UpdateMaterial()
  {
    if (materialInstance == null)
    {
      materialInstance = Instantiate(renderer.sharedMaterial);
      renderer.sharedMaterial = materialInstance;
    }

    materialInstance.SetColor("_ReplacementColor", color.Color);
  }
}