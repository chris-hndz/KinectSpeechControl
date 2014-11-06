using UnityEngine;
using System.Collections;

public class GameControlScript : MonoBehaviour 
{
	public GameObject cratePrefab;
	public Rect guiWindowRect = new Rect(10, 40, 200, 300);
	public GUISkin guiSkin;
	
	
	void Start () 
	{
		Quaternion quatRot90 = Quaternion.Euler(new Vector3(0, 90, 0));
		
		for(int i = -50; i <= 50; i++)
		{
			GameObject.Instantiate(cratePrefab, new Vector3(i, 0.32f, 50), Quaternion.identity);
			GameObject.Instantiate(cratePrefab, new Vector3(i, 0.32f, -50), Quaternion.identity);
			GameObject.Instantiate(cratePrefab, new Vector3(50, 0.32f, i), quatRot90);
			GameObject.Instantiate(cratePrefab, new Vector3(-50, 0.32f, i), quatRot90);
		}
	}

	private void ShowGuiWindow(int windowID) 
	{
		GUILayout.BeginVertical();

		GUILayout.Label("Palabras que reconoce:");
		GUILayout.Label("<b>* FORWARD / GO AHEAD</b> : caminar");
		GUILayout.Label("<b>* BACK / GO BACK</b> : retroceder");
		GUILayout.Label("<b>* TURN LEFT</b> : ir a la izquierda");
		GUILayout.Label("<b>* TURN RIGHT</b> : ir a la derecha");
		GUILayout.Label("<b>* RUN</b> : correr");
		GUILayout.Label("<b>* JUMP</b> : saltar");
		GUILayout.Label("<b>* STOP</b> : detenerse");
		GUILayout.Label("<b>* HELLO</b> : saludar");
		//GUILayout.Label("<i>Creado por Cristian Hernandez</i>\ne-mail: cris.hndz@gmail.com");
		
		GUILayout.EndVertical();
		
		// Make the window draggable.
		GUI.DragWindow();
	}
	
	void OnGUI()
	{
		GUI.skin = guiSkin;
		guiWindowRect = GUI.Window(0, guiWindowRect, ShowGuiWindow, "Comandos de Audio");
	}
	
}
