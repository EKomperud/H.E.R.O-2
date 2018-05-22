using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour {

    [SerializeField] private Slider sliderUI;
    [SerializeField] private Text textUI;
    [SerializeField] private string textPrefix;

	void Start ()
    {
        sliderUI.onValueChanged.AddListener(UpdateText);
        textUI.text = textPrefix + sliderUI.value;
	}

    void UpdateText(float value)
    {
        textUI.text = textPrefix + value;
    }
}
