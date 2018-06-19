using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MovingElement : MonoBehaviour {

    [SerializeField] private Vector3 positionA;
    [SerializeField] private Vector3 positionB;
    [SerializeField] private UnityEvent callbackMethod;
    private RectTransform rect;
    private bool position;

	void Start () {
        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = positionA;
        position = false;
	}

    void OnEnable()
    {
        MainMenuManager.CharSelectTransition += Move;
    }

    void OnDisable()
    {
        MainMenuManager.CharSelectTransition -= Move;
    }

	void Move()
    {
        StopAllCoroutines();
        if (!position && rect.localPosition != positionA)
            rect.localPosition = positionA;
        else if (position && rect.localPosition != positionB)
            rect.localPosition = positionB;
        StartCoroutine("DoMove");
    }

    IEnumerator DoMove()
    {
        position = !position;
        Vector3 target = position ? positionB : positionA;
        Vector3 diff = target - rect.localPosition;
        while (diff.magnitude >= 5)
        {
            rect.localPosition += diff * 0.1f;
            rect.localPosition += diff.normalized * 2f;
            diff = target - rect.localPosition;
            yield return new WaitForEndOfFrame();
        }
        rect.localPosition = target;
        callbackMethod.Invoke();
    }
}
