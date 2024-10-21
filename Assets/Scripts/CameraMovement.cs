using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
	public Transform playerTransform;
    public Transform leftLimit, rightLimit, topLimit, bottomLimit;
    public bool isFollowHorizontal, isFollowVertical;
	public float distance = 10f;

	private Camera mainCamera;
	private Vector3 posToMove;
	private void Awake()
	{
		mainCamera = GetComponent<Camera>();
		posToMove = transform.position - distance * Vector3.forward;
	}

	private void Update()
	{
		if (isFollowHorizontal == true)
		{
			float posToCheckLeft = playerTransform.position.x - (mainCamera.orthographicSize * Screen.width / Screen.height);
			float posToCheckRight = playerTransform.position.x + (mainCamera.orthographicSize * Screen.width / Screen.height);

			if (posToCheckLeft > leftLimit.position.x && posToCheckRight < rightLimit.position.x)
			{
				posToMove.x = playerTransform.position.x;
			}
		}
		if (isFollowVertical == true)
		{
			float posToCheckTop = playerTransform.position.y + mainCamera.orthographicSize;
			float posToCheckBottom = playerTransform.position.y - mainCamera.orthographicSize;

			if (posToCheckTop < topLimit.position.y && posToCheckBottom > bottomLimit.position.y)
			{
				posToMove.y = playerTransform.position.y;
			}
		}
		transform.position = posToMove;
	}
}
