using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	private void OnEnable()
	{
		Cursor.visible = true;
		Time.timeScale = 0;
	}

	private void OnDisable()
	{
		Cursor.visible = false;
		Time.timeScale = 1;
	}
}
