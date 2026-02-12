using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class DummyBlendshapeCharacter : MonoBehaviour
{
    public Material material;

    public string bsBust = "Bust";
    public string bsWaist = "Waist";
    public string bsHips = "Hips";
    public string bsNoseWidth = "NoseWidth";

    private SkinnedMeshRenderer smr;
    private Mesh mesh;

    void Awake()
    {
        smr = GetComponent<SkinnedMeshRenderer>();

        // 1) Create a bone so SkinnedMeshRenderer can actually render
        Transform bone = new GameObject("RootBone").transform;
        bone.SetParent(transform, false);
        bone.localPosition = Vector3.zero;
        bone.localRotation = Quaternion.identity;
        bone.localScale = Vector3.one;

        // 2) Create mesh and bone weights
        mesh = CreateCubeBodyMesh();
        AssignSingleBone(mesh);

        // 3) Add our test blendshapes
        AddBlendshapes(mesh);

        // 4) Assign to SkinnedMeshRenderer
        smr.sharedMesh = mesh;
        smr.rootBone = bone;
        smr.bones = new Transform[] { bone };
        smr.updateWhenOffscreen = true;

        // Material
        if (material != null)
            smr.sharedMaterial = material;
        else
            smr.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }

    Mesh CreateCubeBodyMesh()
    {
        Vector3[] v = new Vector3[8]
        {
            new Vector3(-0.3f, 0.0f, -0.175f),
            new Vector3( 0.3f, 0.0f, -0.175f),
            new Vector3( 0.3f, 1.8f, -0.175f),
            new Vector3(-0.3f, 1.8f, -0.175f),

            new Vector3(-0.3f, 0.0f,  0.175f),
            new Vector3( 0.3f, 0.0f,  0.175f),
            new Vector3( 0.3f, 1.8f,  0.175f),
            new Vector3(-0.3f, 1.8f,  0.175f),
        };

        int[] t = new int[]
        {
            // Front (-Z)
            0,2,1, 0,3,2,
            // Back (+Z)
            4,5,6, 4,6,7,
            // Left (-X)
            0,7,3, 0,4,7,
            // Right (+X)
            1,2,6, 1,6,5,
            // Top (+Y)
            3,7,6, 3,6,2,
            // Bottom (0Y)
            0,1,5, 0,5,4
        };

        Mesh m = new Mesh();
        m.name = "DummyBodyMesh";
        m.vertices = v;
        m.triangles = t;
        m.RecalculateNormals();
        m.RecalculateBounds();

        return m;
    }

    void AssignSingleBone(Mesh m)
    {
        // One bone (index 0) influences every vertex 100%
        var bw = new BoneWeight[m.vertexCount];
        for (int i = 0; i < bw.Length; i++)
        {
            bw[i].boneIndex0 = 0;
            bw[i].weight0 = 1f;
        }
        m.boneWeights = bw;

        // One bindpose for that bone (identity at origin is fine)
        m.bindposes = new Matrix4x4[] { Matrix4x4.identity };
    }

    void AddBlendshapes(Mesh m)
    {
        Vector3[] baseV = m.vertices;

        AddRangeWidthShape(m, baseV, bsBust, yMin: 1.1f, yMax: 1.5f, maxDelta: 0.18f);
        AddRangeWidthShape(m, baseV, bsWaist, yMin: 0.75f, yMax: 1.05f, maxDelta: -0.18f);
        AddRangeWidthShape(m, baseV, bsHips, yMin: 0.25f, yMax: 0.7f, maxDelta: 0.18f);
        AddRangeDepthShape(m, baseV, bsNoseWidth, yMin: 1.65f, yMax: 1.8f, maxDelta: 0.12f);
    }

    void AddRangeWidthShape(Mesh m, Vector3[] baseV, string shapeName, float yMin, float yMax, float maxDelta)
    {
        Vector3[] dv = new Vector3[baseV.Length];
        Vector3[] dn = new Vector3[baseV.Length];
        Vector3[] dt = new Vector3[baseV.Length];

        for (int i = 0; i < baseV.Length; i++)
        {
            float y = baseV[i].y;
            if (y >= yMin && y <= yMax)
            {
                float dir = Mathf.Sign(baseV[i].x);
                dv[i] = new Vector3(dir * maxDelta, 0f, 0f);
            }
        }

        m.AddBlendShapeFrame(shapeName, 100f, dv, dn, dt);
    }

    void AddRangeDepthShape(Mesh m, Vector3[] baseV, string shapeName, float yMin, float yMax, float maxDelta)
    {
        Vector3[] dv = new Vector3[baseV.Length];
        Vector3[] dn = new Vector3[baseV.Length];
        Vector3[] dt = new Vector3[baseV.Length];

        for (int i = 0; i < baseV.Length; i++)
        {
            float y = baseV[i].y;
            if (y >= yMin && y <= yMax)
            {
                float dir = Mathf.Sign(baseV[i].z);
                dv[i] = new Vector3(0f, 0f, dir * maxDelta);
            }
        }

        m.AddBlendShapeFrame(shapeName, 100f, dv, dn, dt);
    }
}
