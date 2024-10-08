#region

using UnityEngine;

#endregion

public class FOVMeshCreator : MonoBehaviour
{
    private float _actualAngle;
    public float angle;

    public Material mat;
    private Mesh _myMesh;
    private Vector3[] _normals;
    public float radius;
    private float _segmentAngle;

    public float segments = 10;
    private int[] _triangles;
    private Vector2[] _uvs;

    private Vector3[] _verts;

    private void Start()
    {
        var meshF = gameObject.AddComponent<MeshFilter>();
        var meshR = gameObject.AddComponent<MeshRenderer>();

        meshR.material = mat;

        //go.renderer.material.mainTexture = Resources.Load("glass", typeof(Texture2D));
        //AssetDatabase.CreateAsset(material, "Assets/MyMaterial.mat");

        //MESH
        _myMesh = gameObject.GetComponent<MeshFilter>().mesh;

        //BUILD THE MESH
        //BuildMesh();
    }

    public void BuildMesh()
    {
        // Grab the Mesh off the gameObject
        //myMesh = gameObject.GetComponent<MeshFilter>().mesh;

        //Clear the mesh
        if (_myMesh == null)
            _myMesh = gameObject.AddComponent<MeshFilter>().mesh;
        _myMesh.Clear();

        // Calculate actual pythagorean angle
        _actualAngle = 90.0f - angle;

        // Segment Angle
        _segmentAngle = angle * 2 / segments;

        // Initialise the array lengths
        _verts = new Vector3[(int) segments * 3];
        _normals = new Vector3[(int) segments * 3];
        _triangles = new int[(int) segments * 3];
        _uvs = new Vector2[(int) segments * 3];

        // Initialise the Array to origin Points
        for (var i = 0; i < _verts.Length; i++)
        {
            _verts[i] = new Vector3(0, 0, 0);
            _normals[i] = Vector3.up;
        }

        // Create a dummy angle
        var a = _actualAngle;

        // Create the Vertices
        for (var i = 1; i < _verts.Length; i += 3)
        {
            _verts[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a) * radius, // x
                0, // y
                Mathf.Sin(Mathf.Deg2Rad * a) * radius); // z

            a += _segmentAngle;

            _verts[i + 1] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * a) * radius, // x
                0, // y
                Mathf.Sin(Mathf.Deg2Rad * a) * radius); // z          
        }

        // Create Triangle
        for (var i = 0; i < _triangles.Length; i += 3)
        {
            _triangles[i] = 0;
            _triangles[i + 1] = i + 2;
            _triangles[i + 2] = i + 1;
        }

        // Generate planar UV Coordinates
        for (var i = 0; i < _uvs.Length; i++)
            _uvs[i] = new Vector2(_verts[i].x, _verts[i].z);

        // Put all these back on the mesh
        _myMesh.vertices = _verts;
        _myMesh.normals = _normals;
        _myMesh.triangles = _triangles;
        _myMesh.uv = _uvs;
    }
}