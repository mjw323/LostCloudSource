using UnityEngine;
using UnityEditor;

public class MissingComponentFinder : EditorWindow 
{
	[MenuItem("Window/Missing Component Finder")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(MissingComponentFinder));
	}

	public void OnGUI()
	{
		if (GUILayout.Button("Scan Scene"))
			Search();
	}

	private static void Search()
	{
		Debug.Log("[Missing Component Finder] Scanning scene for missing components...");
		missingComponentsTotal = 0;
		GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
		foreach (GameObject gameObject in gameObjects)
		{
			bool isMissingComponents = false;
			int missingComponents = 0;
			Component[] components = gameObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (component == null)
				{
					missingComponents++;
					isMissingComponents = true;
				}
			}
			missingComponentsTotal += missingComponents;
			if (isMissingComponents)
				Debug.Log(missingComponents + " components missing on " +
					gameObject.name + ".", gameObject);
		}
		Debug.Log("[Missing Component Finder] Scan complete. " +
			missingComponentsTotal + " missing components found.");
	}

	private static int missingComponentsTotal;
}