using System.Collections;
using UnityEngine;

/// <summary>
/// Camera zoom by touches (for android). Orthographic camera projection only.
/// This script must be on camera.
/// </summary>
public class ZoomCamera : MonoBehaviour
{
	public static ZoomCamera Instance { get; private set; }
	private Camera mainCamera;
	private float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;
	private Vector2 firstTouchPrevPos, secondTouchPrevPos;
	[SerializeField] private float zoomModifierSpeed = 0.1f;
	[SerializeField] private float moveModifierSpeed = 0.1f;
	private CameraFollowing cameraFollowing;
	//private Vector2 currMaxScreenEdge, currMinScreenEdge;
	
	void Start()
	{
		Instance = this;
		mainCamera = GetComponent<Camera>();
		cameraFollowing = GetComponent<CameraFollowing>();
	}

	void FixedUpdate()
	{
		//block script
		if (UIMapMenu.Instance.isZooming)
			return;

		if (Input.touchCount == 2)// && cameraFollowing.enabled == false)
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
				mainCamera.orthographicSize += zoomModifier;
				mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, 1.5f, 10f);
            }
			if (touchesPrevPosDifference < touchesCurPosDifference)
			{

				mainCamera.orthographicSize -= zoomModifier;
				mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, 1.5f, 10f);


				/// This part doen't work correctly. 
				/// It should check if we are off screen edges (after zooming back) and in this case change camera position
				//currMinScreenEdge = ScreenInfo.GetCurrMinScreenEdge();
				//currMaxScreenEdge = ScreenInfo.GetCurrMaxScreenEdge();
				//if (!ScreenInfo.Instance.IsOnTheScreen(currMinScreenEdge) || !ScreenInfo.Instance.IsOnTheScreen(currMaxScreenEdge))
				//{
				//    float newX = gameObject.transform.position.x, newY = gameObject.transform.position.y;
				//    if (currMinScreenEdge.x < ScreenInfo.Instance.minScreenEdge.x)
				//        newX = gameObject.transform.position.x + (ScreenInfo.Instance.minScreenEdge.x - currMinScreenEdge.x);
				//    else if (currMaxScreenEdge.x > ScreenInfo.Instance.maxScreenEdge.x)
				//        newX = gameObject.transform.position.x + (-ScreenInfo.Instance.maxScreenEdge.x + currMaxScreenEdge.x);

				//    if (currMinScreenEdge.y < ScreenInfo.Instance.minScreenEdge.y)
				//        newY = gameObject.transform.position.y + (ScreenInfo.Instance.minScreenEdge.y - currMinScreenEdge.y);
				//    else if (currMaxScreenEdge.y > ScreenInfo.Instance.maxScreenEdge.y)
				//        newY = gameObject.transform.position.y + (-ScreenInfo.Instance.maxScreenEdge.y + currMaxScreenEdge.y);

				//    gameObject.transform.position = new Vector2(newX, newY);
				//}
			}
			//if (UIMapMenu.Instance.isPlanetMode && mainCamera.orthographicSize > UIMapMenu.Instance.cameraSizeOnPlanetMode)
			//	UIMapMenu.Instance.SwitchMode();
			//if (!UIMapMenu.Instance.isPlanetMode && mainCamera.orthographicSize < UIMapMenu.Instance.cameraSizeOnPlanetMode)
			//	UIMapMenu.Instance.SwitchMode();
		}
		if (mainCamera.orthographicSize <= 2f && UIMapMenu.Instance.followPlayerPlanet)
				cameraFollowing.enabled = true;
			if (mainCamera.orthographicSize >= 2f)
				if (cameraFollowing.enabled == true)
					cameraFollowing.enabled = false;
		if (Input.touchCount == 1)// && cameraFollowing.enabled == false)
		{
			if (cameraFollowing.enabled == true)
				cameraFollowing.enabled = false;
			Touch firstTouch = Input.GetTouch(0);
			Vector3 dir = -firstTouch.deltaPosition;
			Vector2 bottomLeft = ScreenInfo.GetCurrMinScreenEdge();
            Vector2 topRight = ScreenInfo.GetCurrMaxScreenEdge();
			bottomLeft.x += dir.x * moveModifierSpeed;
			bottomLeft.y += dir.y * moveModifierSpeed;
			topRight.x += dir.x * moveModifierSpeed;
			topRight.y += dir.y * moveModifierSpeed;
			Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
			Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);
			///Using left, right, top, bottom values as separate variables instead would be better practice
			///We check if the half of our content s
			if (bottomLeft.y < ScreenInfo.Instance.minScreenEdge.y)
				if (topLeft.y < ScreenInfo.Instance.centerPoint.y)
					if (dir.y < 0)
						dir.y = 0;

			if (topLeft.y > ScreenInfo.Instance.maxScreenEdge.y)
				if (bottomLeft.y > ScreenInfo.Instance.centerPoint.y)
					if (dir.y > 0)
						dir.y = 0;

            if (topLeft.x < ScreenInfo.Instance.minScreenEdge.x)
                if (topRight.x < ScreenInfo.Instance.centerPoint.x)
                    if (dir.x < 0)
                        dir.x = 0;

            if (bottomRight.x > ScreenInfo.Instance.maxScreenEdge.x)
				if (bottomLeft.x > ScreenInfo.Instance.centerPoint.x)
					if (dir.x > 0)
						dir.x = 0;
			
			mainCamera.transform.Translate(dir * moveModifierSpeed, Space.World);
        }
	}
}
