using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PatrolPath))]
public class PatrolPathEditor : Editor {

    private bool addNodeMode = false;

    public override void OnInspectorGUI()
    {
        PatrolPath myTarget = target as PatrolPath;
        if(GUILayout.Button("Rebuild Path"))
        {
            myTarget.RebuildPath();
        }
        addNodeMode = EditorGUILayout.Toggle("Add Nodes Mode", addNodeMode);
        this.DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        Event current = Event.current;
        int controlID = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);

        switch (current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (addNodeMode && current.button == 0)
                {
                    GUIUtility.hotControl = controlID;
                    PatrolPath myTarget = target as PatrolPath;
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(worldRay, out hitInfo))
                    {
                        Vector3 loc = new Vector3(hitInfo.point.x, Mathf.Round(hitInfo.point.y), hitInfo.point.z);
                        PatrolNode node = Instantiate(myTarget.nodeType, loc, Quaternion.identity) as PatrolNode;
                        node.name = "Patrol Node (" + myTarget.Path.Count + ")";
                        node.transform.parent = myTarget.transform;
                    }
                    current.Use();
                }
                break;

            case EventType.MouseUp:
                if (addNodeMode && current.button == 0)
                {
                    GUIUtility.hotControl = 0;
                    current.Use();
                }
                break;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

}
