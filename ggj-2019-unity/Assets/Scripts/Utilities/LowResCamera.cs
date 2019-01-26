using UnityEngine;

public class LowResCamera : MonoBehaviour
{
  [SerializeField]
  private Camera inputCamera = null;

  [SerializeField]
  private CameraTextureBlit outputCamera = null;

  [SerializeField]
  private float scaleFactor = 0.25f;

  private RenderTexture renderTexture;
  private int lastCameraWidth;
  private int lastCameraHeight;

  private void Awake()
  {
    SetUpRenderTexture();
  }

  private void OnDestroy()
  {
    ReleaseTexture();
  }

  private void Update()
  {
    if (lastCameraWidth != outputCamera.Camera.pixelWidth || lastCameraHeight != outputCamera.Camera.pixelHeight)
    {
      SetUpRenderTexture();
    }
  }

  private void SetUpRenderTexture()
  {
    ReleaseTexture();

    lastCameraWidth = outputCamera.Camera.pixelWidth;
    lastCameraHeight = outputCamera.Camera.pixelHeight;
    int width = Mathf.CeilToInt(outputCamera.Camera.pixelWidth * scaleFactor);
    int height = Mathf.CeilToInt(outputCamera.Camera.pixelHeight * scaleFactor);
    renderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
    renderTexture.filterMode = FilterMode.Point;

    inputCamera.targetTexture = renderTexture;
    outputCamera.InputTexture = renderTexture;
  }

  private void ReleaseTexture()
  {
    if (renderTexture != null)
    {
      renderTexture.Release();
      Destroy(renderTexture);
      renderTexture = null;
    }
  }
}