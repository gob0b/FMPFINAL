using UnityEngine;

public class FullscreenQuad : MonoBehaviour
{
    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();

        Vector3[] vertices = {
            new Vector3(-1, -1, 0), // Bottom-left
            new Vector3(1, -1, 0),  // Bottom-right
            new Vector3(-1, 1, 0),  // Top-left
            new Vector3(1, 1, 0)    // Top-right
        };

        int[] triangles = { 0, 2, 1, 2, 3, 1 }; // Two triangles

        Vector2[] uv = {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();

        GameObject quad = new GameObject("FullscreenQuad", typeof(MeshFilter), typeof(MeshRenderer));
        quad.GetComponent<MeshFilter>().mesh = mesh;
        quad.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/VignetteShader"));
    }
}

