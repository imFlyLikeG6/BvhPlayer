using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @file RectAttachAnchors.cs
 * @brief ugui의 rectTransform을 ngui의 widget 크기에 맞춥니다. 그래프ui에 붙어있습니다.
 */
public class RectAttachAnchors : MonoBehaviour {

    public RectTransform m_RectTransform;
    public UIWidget m_Widget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //m_RectTransform.transform.position = m_Widget.transform.position;
        m_RectTransform.sizeDelta = m_Widget.localSize;
        
        m_RectTransform.localPosition = new Vector2(m_Widget.width / 2, 0);
    }
}
