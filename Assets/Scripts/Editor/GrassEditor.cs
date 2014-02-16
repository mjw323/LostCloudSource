using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Diagnostics;
using System.Threading;

[CustomEditor(typeof(Grass)),CanEditMultipleObjects]
public class GrassEditor : Editor {
    SerializedProperty material;
    Stopwatch timer;

	void OnEnable() {
        if(timer == null)
            timer = new Stopwatch();

        material = serializedObject.FindProperty("grassMaterial");
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(material, new GUIContent("Grass Material"));

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if(GUILayout.Button("Generate Grass"))
        {            
            if(material.objectReferenceValue != null)
            {
                timer.Reset();
                UnityEngine.Debug.Log("Generating Grass...");
                timer.Start();
                OnGenerateGrass();                
                timer.Stop();
                UnityEngine.Debug.Log("Grass Generated. (" + timer.ElapsedMilliseconds + "ms)");
            }
            else
            {
                UnityEngine.Debug.LogError("ERROR: Grass material not assigned.");
            }
        }

        if (GUILayout.Button("Delete Grass"))
        {
            if (Selection.activeGameObject.transform.childCount > 0)
            {
                OnDeleteGrass();
            }
            else
            {
                UnityEngine.Debug.LogError("ERROR: Unable to locate grass objects.");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    void OnGenerateGrass() {
        if(Selection.activeGameObject.transform.parent != null ) {
            int bladeCount = 25;

            RaycastHit hit;
            GameObject grassChild;
            GameObject grassRoot = Selection.activeGameObject;
            Bounds bounds = grassRoot.transform.parent.gameObject.renderer.bounds;
            
            Vector3 offset = Vector3.zero, height = Vector3.zero;

            float x = bounds.min.x, 
                  z = bounds.min.z,
                  deltaX = (bounds.max.x - bounds.min.x) / bladeCount,
                  deltaZ = (bounds.max.z - bounds.min.z) / bladeCount;

            for (int j = 0; j < bladeCount; j++)
            {
                offset.z = z;

                for (int i = 0; i < bladeCount; i++)
                {
                    offset.x = x;
                    offset.y = grassRoot.transform.position.y;

                    // Randomize to avoid grid-like visuals
                    offset.x += Random.Range(-12.5f, 12.5f);
                    offset.z += Random.Range(-12.5f, 12.5f);

                    /*if (Physics.Raycast(offset, Vector3.down, out hit, 200.0f))
                        UnityEngine.Debug.DrawLine(offset, offset + Vector3.down * 20.0f, Color.green, 60.0f);
                    else
                        UnityEngine.Debug.DrawLine(offset, offset + Vector3.down * 20.0f, Color.red, 60.0f);*/

                    if (Physics.Raycast(offset, Vector3.down, out hit, 200.0f))
                    {
                        offset.y = 0.0f;

                        grassChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        grassChild.name = "GrassChild_" + (x * z) + "_a";
                        grassChild.transform.parent = grassRoot.transform;
                        grassChild.transform.localScale = Vector3.one * 4.0f;
                        height.y = hit.point.y + (grassChild.renderer.bounds.max.y - grassChild.renderer.bounds.min.y) * 0.5f; // Height offset from midpoint
                        grassChild.transform.position = offset + height;
                        grassChild.renderer.castShadows = false;
                        grassChild.renderer.receiveShadows = false;
                        grassChild.renderer.material = (Material)material.objectReferenceValue;

                        grassChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        grassChild.name = "GrassChild_" + (x * z) + "_b";
                        grassChild.transform.parent = grassRoot.transform;
                        grassChild.transform.localScale = Vector3.one * 4.0f;
                        grassChild.transform.position = offset + height;
                        grassChild.transform.Rotate(Vector3.up, 45.0f);
                        grassChild.renderer.castShadows = false;
                        grassChild.renderer.receiveShadows = false;
                        grassChild.renderer.material = (Material)material.objectReferenceValue;

                        grassChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        grassChild.name = "GrassChild_" + (x * z) + "_c";
                        grassChild.transform.parent = grassRoot.transform;
                        grassChild.transform.localScale = Vector3.one * 4.0f;
                        grassChild.transform.position = offset + height;
                        grassChild.transform.Rotate(Vector3.up, -45.0f);
                        grassChild.renderer.castShadows = false;
                        grassChild.renderer.receiveShadows = false;
                        grassChild.renderer.material = (Material)material.objectReferenceValue;                        
                    }
                    x += deltaX;
                }

                z += deltaZ;
                x = bounds.min.x;
            }            
        }
    }

    void OnDeleteGrass() {
        // TODO: I AM BROKEN :'(
        foreach (Transform g in Selection.activeGameObject.transform)
        {
            DestroyImmediate(g.gameObject);
        }
    }
}
