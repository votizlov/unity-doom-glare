using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GlarePlane : MonoBehaviour
{
    public Color quadColor = new Color(1f, 1f, 1f, 0.95f);
    public Color edgeColor = new Color(0f, 0f, 0f, 0f);
    public Material material;
    public bool wireframe = false;

    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    private int[] indices;
    private Vector2[] uv;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        CreateGeometry();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    void Update()
    {
        if (mainCamera == null) return;

        Extrude(mainCamera);

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void CreateGeometry()
    {
        vertices = new Vector3[16];
        colors = new Color[16];
        uv = new Vector2[16];
        indices = new int[]
        {
            0,1,2, 0,2,3,                                                   // Quad
            0,5,7, 0,7,1, 1,8,10, 1,10,2, 2,11,13, 2,13,3, 3,14,4, 3,4,0,   // Flaps
            0,4,6, 0,6,5, 1,7,9, 1,9,8, 2,10,12, 2,12,11, 3,13,15, 3,15,14  // Connections
        };

        float[] positions = new float[]
        {
            -1f, -1f, 0f,
            1f, -1f, 0f,
            1f, 1f, 0f,
            -1f, 1f, 0f,

            -1f, -1f, 0f,
            1f, -1f, 0f,
            1f, 1f, 0f,
            -1f, 1f, 0f,

            -1f, -1f, 0f,
            1f, -1f, 0f,
            1f, 1f, 0f,
            -1f, 1f, 0f,

            -1f, -1f, 0f,
            1f, -1f, 0f,
            1f, 1f, 0f,
            -1f, 1f, 0f,
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
            colors[i] = Color.white;
            uv[i] = new Vector2(1f, 0f);
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.colors = colors;
        mesh.triangles = indices;
    }

    void Extrude(Camera camera, float pushDistance = 0.5f)
        {
            Vector3 cameraPosition = camera.transform.position;
            Vector3 directionToCenter = (transform.position - cameraPosition).normalized;
            Vector3 quadNormal = transform.forward; // Use the object's forward direction
    
            float dot = Vector3.Dot(directionToCenter, quadNormal);
    
            float alpha = Mathf.Clamp01(Mathf.InverseLerp(0.001f, 0.1f, Mathf.Abs(dot)));
    
            for (int i = 0; i < 4; i++)
            {
                colors[i] = new Color(quadColor.r, quadColor.g, quadColor.b, quadColor.a * alpha);
            }
    
            for (int i = 4; i < colors.Length; i++)
            {
                colors[i] = edgeColor;
            }
    
            Vector3[] eyeToVerticesWorldSpace = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                Vector3 worldVertex = transform.TransformPoint(vertices[i]);
                eyeToVerticesWorldSpace[i] = (worldVertex - cameraPosition).normalized;
            }
    
            // Extrude quad vertices
            float sign = Mathf.Sign(dot);
            Vector3[] pushDirectionsWorldSpace = new Vector3[3];
            for (int i = 0; i < 4; i++)
            {
                pushDirectionsWorldSpace[0] = Vector3.Cross(eyeToVerticesWorldSpace[i], eyeToVerticesWorldSpace[(i + 3) % 4]).normalized * sign;
                pushDirectionsWorldSpace[1] = Vector3.Cross(eyeToVerticesWorldSpace[(i + 1) % 4], eyeToVerticesWorldSpace[i]).normalized * sign;
                pushDirectionsWorldSpace[2] = (pushDirectionsWorldSpace[0] + pushDirectionsWorldSpace[1]).normalized;
    
                for (int j = 0; j < 3; j++)
                {
                    Vector3 offset = pushDirectionsWorldSpace[j] * pushDistance;
    
                    Vector3 worldVertex = transform.TransformPoint(vertices[i]);
    
                    Vector3 worldVertexExtruded = worldVertex + offset;
    
                    vertices[4 + j + 3 * i] = transform.InverseTransformPoint(worldVertexExtruded);
                }
            }
        }
}
