using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomArea : MonoBehaviour
{
	private float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;
	private Vector2 firstTouchPrevPos, secondTouchPrevPos;
	[SerializeField] private float zoomModifierSpeed = 0.1f;
	[SerializeField] private float moveModifierSpeed = 0.1f;
	[SerializeField] private Canvas canvas;
	private RectTransform rectTransform;
	private Vector2 currMaxScreenEdge, currMinScreenEdge;
	private Vector2 currScale;
    private void Start()
    {
		rectTransform = GetComponent<RectTransform>();
		currScale = gameObject.transform.localScale;

		///*Left*/
		//Debug.Log(rectTransform.offsetMin.x);
		///*Bottom*/
		//Debug.Log(rectTransform.offsetMin.y);
		//Debug.Log(canvas.transform.TransformPoint(rectTransform.offsetMin));
		///*Right*/
		//Debug.Log(rectTransform.offsetMax.x);
		///*Top*/
		//Debug.Log(rectTransform.offsetMax.y);
		//Debug.Log(RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.offsetMax));
	}

    void FixedUpdate()
	{
		if (Input.touchCount == 2)
		{
			Touch firstTouch = Input.GetTouch(0);
			Touch secondTouch = Input.GetTouch(1);

			firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
			secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

			touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
			touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

			zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;

			if (touchesPrevPosDifference > touchesCurPosDifference)
			{
				currScale.x += zoomModifier;
				currScale.y += zoomModifier;				
			}

			if (touchesPrevPosDifference < touchesCurPosDifference)
			{
				currScale.x -= zoomModifier;
				currScale.y -= zoomModifier;
				//сдвиг

			}
			currScale.x = Mathf.Clamp(currScale.x, 1f, 5f);
			currScale.y = Mathf.Clamp(currScale.y, 1f, 5f);
			gameObject.transform.localScale = currScale;
		}

		if (Input.touchCount == 1)
		{
			Touch firstTouch = Input.GetTouch(0);
			Vector3 dir = -firstTouch.deltaPosition;
			bool move = true;
			Vector2 minAreaEdge = canvas.transform.TransformPoint(rectTransform.offsetMin);
			Debug.Log(minAreaEdge);
			Vector2 maxAreaEdge = canvas.transform.TransformPoint(rectTransform.offsetMax);
			Debug.Log(maxAreaEdge);
			minAreaEdge.x += dir.x * moveModifierSpeed;
			minAreaEdge.y += dir.y * moveModifierSpeed;
			maxAreaEdge.x += dir.x * moveModifierSpeed;
			maxAreaEdge.y += dir.y * moveModifierSpeed;
			Debug.Log(minAreaEdge);
			Debug.Log(maxAreaEdge);
			if (minAreaEdge.x < ScreenInfo.Instance.minScreenEdge.x)
				move = false;
			else if (maxAreaEdge.x > ScreenInfo.Instance.maxScreenEdge.x)
				move = false;
			if (minAreaEdge.y < ScreenInfo.Instance.minScreenEdge.y)
				move = false;
			else if (maxAreaEdge.y > ScreenInfo.Instance.maxScreenEdge.y)
				move = false;

			if (move)
				gameObject.transform.Translate(dir * moveModifierSpeed, Space.World);
		}
	}
}
