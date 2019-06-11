using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LightController : MonoBehaviour {
	public Light light;
	public float strength;
	public Slider slider;
	public void editStrength(){
		light.intensity = slider.value;
	}
}
