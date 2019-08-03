using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
	Material mat;
	
	public Color onColor;
	public Color offColor;
	bool on;
	float timer;
	public float waitTime;

    void Start()
    {
		mat = this.GetComponent<Renderer>().material;
		on = true;
    }

    // Update is called once per frame
    void Update()
    {
		timer += Time.deltaTime;
		if (timer > waitTime)
		{
			if (on)
			{
				mat.color = onColor;
				on = false;
			}
			else{
				mat.color = offColor;
				on = true;
			}
			timer = 0;
		}
    }
}

