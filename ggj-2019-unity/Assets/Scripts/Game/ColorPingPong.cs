using UnityEngine;

public class ColorPingPong : MonoBehaviour
{
	Material mat;
	public Color startColor;
	public Color endColor;
	public float ppTime;
	

    void Awake()
	{
		mat = this.GetComponent<Renderer>().material;
	}
	
	void Update()
    {
		mat.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time, ppTime));
    }
}
