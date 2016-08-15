using UnityEngine;
using UnityEditor;

//TODO: Add undo functionality 
[CustomEditor(typeof(QuadEditor))]
public class TestQuadEditor : Editor
{
    private QuadEditor editor;
    private bool selectTopRight;
    private bool selectTopLeft;
    private bool selectBottomRight;
    private bool selectBottomLeft;
    private bool isMouseDragging;
    private bool setVertices = true;
    private VertNames selectedVertex;
    private string filePath;

    private Vector3
        bottomLeft,
        topRight,
        bottomRight,
        topLeft;

    private Vector3
        defaultBottomLeft,
        defaultTopRight,
        defaultBottomRight,
        defaultTopLeft;

    private enum VertNames { TopLeft, TopRight, BottomLeft, BottomRight, None };

    public override void OnInspectorGUI()
    {
    	
    	EditorGUILayout.Space();
	    // show all the vertice positions
	    EditorGUILayout.Vector3Field("Top Left Vertex", topLeft);
	    EditorGUILayout.Vector3Field("Top Right Vertex", topRight);
	    EditorGUILayout.Vector3Field("Bottom Left Vertex", bottomLeft);
	    EditorGUILayout.Vector3Field("Bottom Right Vertex", bottomRight);

	    EditorGUILayout.Space();

	    EditorGUILayout.Space();

	    if (GUILayout.Button("Reset Vertices"))
	    {
	        topLeft = defaultTopLeft;
		    topRight = defaultTopRight;
		    bottomLeft = defaultBottomLeft;
		    bottomRight = defaultBottomRight;
	    }

	    EditorGUILayout.Space();

	    if (GUILayout.Button("Done"))
	    {
            filePath = EditorUtility.SaveFilePanelInProject("Save Mesh", "New Mesh", "prefab", "Please enter a file name to save the mesh to");
            if (filePath != "")
            {
                AssetDatabase.CreateAsset(editor.mesh, filePath);
            }
	    }
    }

    void OnSceneGUI()
    {
    	editor = target as QuadEditor;
		
		if (setVertices && editor.mesh)
		{
			setVertices = false;

			defaultBottomLeft = editor.mesh.vertices[0];
	        defaultTopRight = editor.mesh.vertices[1];
	        defaultBottomRight = editor.mesh.vertices[2];
	        defaultTopLeft = editor.mesh.vertices[3];

	    	bottomLeft = editor.mesh.vertices[0];
	        topRight = editor.mesh.vertices[1];
	        bottomRight = editor.mesh.vertices[2];
	        topLeft = editor.mesh.vertices[3];
		}

        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controlID);
        }
        else
        {
            Ray screenToWordCoords = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, -Event.current.mousePosition.y + Camera.current.pixelHeight));
            Vector3 mousePos = screenToWordCoords.origin;
            //Vector3 mousePos = Camera.current.ScreenToWorldPoint(Event.current.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, editor.transform.position.z);

            Handles.color = GetHandleColor(VertNames.TopLeft);
            Debug.Log(topLeft+editor.transform.position);
            Handles.SphereCap(controlID, new Vector3(editor.transform.position.x + topLeft.x, editor.transform.position.y + topLeft.y, 0), editor.transform.rotation, 0.05f);
            Handles.color = GetHandleColor(VertNames.TopRight);
            Handles.SphereCap(controlID, new Vector3(editor.transform.position.x + topRight.x, editor.transform.position.y + topRight.y, 0), editor.transform.rotation, 0.05f);
            Handles.color = GetHandleColor(VertNames.BottomRight);
            Handles.SphereCap(controlID, new Vector3(editor.transform.position.x + bottomRight.x, editor.transform.position.y + bottomRight.y, 0), editor.transform.rotation, 0.05f);
            Handles.color = GetHandleColor(VertNames.BottomLeft);
            Handles.SphereCap(controlID, new Vector3(editor.transform.position.x + bottomLeft.x, editor.transform.position.y + bottomLeft.y, 0), editor.transform.rotation, 0.05f);

        	if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                isMouseDragging = true;
                UpdateVertexHandles(mousePos);
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                isMouseDragging = false;
                UpdateVertexHandles(Vector3.zero, true);
            }

            selectedVertex = (selectTopLeft) ? VertNames.TopLeft : (selectTopRight) ? VertNames.TopRight : (selectBottomLeft) ? 
                VertNames.BottomLeft : (selectBottomRight) ? VertNames.BottomRight : VertNames.None;

            MoveVertex(mousePos);
            editor.mesh.vertices = new Vector3[]
            {
                bottomLeft,
                topRight,
                bottomRight,
                topLeft
            };
        }
    }

    void UpdateVertexHandles(Vector3 mousePos, bool mouseUp = false)
    {
        Debug.Log("Update Vertex Handles " + mousePos);

        if (mouseUp)
        {
            selectTopLeft = false;
	        selectTopRight = false;
	        selectBottomLeft = false;
	        selectBottomRight = false;
            
        }
        else
        {
            Debug.Log((mousePos - (topLeft + editor.transform.position)).magnitude);

            selectTopLeft = (Vector3.Distance(new Vector3(editor.transform.position.x + topLeft.x, editor.transform.position.y + topLeft.y, 0), mousePos) < 0.05f) ? true : false;
            selectTopRight = (Vector3.Distance(new Vector3(editor.transform.position.x + topRight.x, editor.transform.position.y + topRight.y, 0), mousePos) < 0.05f) ? true : false;
            selectBottomLeft = (Vector3.Distance(new Vector3(editor.transform.position.x + bottomLeft.x, editor.transform.position.y + bottomLeft.y, 0), mousePos) < 0.05f) ? true : false;
            selectBottomRight = (Vector3.Distance(new Vector3(editor.transform.position.x + bottomRight.x, editor.transform.position.y + bottomRight.y, 0), mousePos) < 0.05f) ? true : false;
        }
    }

    Color GetHandleColor(VertNames vertName)
    {
        Color handleColor = Color.white;

        switch (vertName)
        {
            case VertNames.TopLeft:
                handleColor = (selectTopLeft) ? Color.yellow : Color.green;
                break;
            case VertNames.TopRight:
                handleColor = (selectTopRight) ? Color.yellow : Color.green;
                break;
            case VertNames.BottomLeft:
                handleColor = (selectBottomLeft) ? Color.yellow : Color.green;
                break;
            case VertNames.BottomRight:
                handleColor = (selectBottomRight) ? Color.yellow : Color.green;
                break;
        }

        return handleColor;
    }

    void MoveVertex(Vector3 targetPos)
    {
        if (!isMouseDragging)
        { return; }

        targetPos = editor.transform.position;

        switch (selectedVertex)
        {
            case VertNames.TopLeft:
            	topLeft = targetPos;
                break;
            case VertNames.TopRight:
            	topRight = targetPos;
                break;
            case VertNames.BottomLeft:
            	bottomLeft = targetPos;
                break;
            case VertNames.BottomRight:
            	bottomRight = targetPos;
                break;
        }
    }
}
