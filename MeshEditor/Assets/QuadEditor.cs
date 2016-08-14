using UnityEngine;

[ExecuteInEditMode]
public class QuadEditor : MonoBehaviour
{
    [HideInInspector] public Mesh mesh;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();  
        Mesh meshCopy = Mesh.Instantiate(mf.sharedMesh) as Mesh;
        mesh = mf.mesh = meshCopy;
    }
}
