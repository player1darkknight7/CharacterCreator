using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class DummyBlendshapeCharacter : MonoBehaviour
{
    [Header("Material (Optional)")]
    public Material material;

    [Header("Blendshape Names (must match UI binder names)")]
    public string bsBust = "Bust";
    public string bsWaist = "Waist";
    public string bsHips = "Hips";
    public string bsNoseWidth = "NoseWidth";

    private SkinnedMeshRenderer smr;
    private Mesh mesh;

    void Awake()
    {
        smr = GetComponent<SkinnedMeshRenderer>();

        // 1) Create a bone so SkinnedMeshRenderer can render
        Transform bone = new GameObject("RootBone").transform;
        bone.SetParent(transform, false);
        bone.localPosition = Vector3.zero;
        bone.localRotation = Quaternion.identity;
        bone.localScale = Vector3.one;

        // 2) Create segmented mesh (has vertices across height so bands work)
        mesh = CreateSegmentedBodyMesh();

        // 3) Assign bone weights / bindposes
        AssignSingleBone(mesh);

        // 4) Add blendshapes (bust/waist/hips/nosewidth)
        AddBlendshapes(mesh);

        // 5) Assign to SkinnedMeshRenderer
        smr.sharedMesh = mesh;
        smr.rootBone = bone;
        smr.bones = new Transform[] { bone };
        smr.updateWhenOffscreen = true;

        // 6) Material
        if (material != null)
            smr.sharedMaterial = material;
        else
            smr.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }

    Mesh CreateSegmentedBodyMesh()
    {
        // Width/Depth of body
        float w = 0.30f;
        float d = 0.175f;

        // More height levels so bust/waist/hips have vertices to affect
        float[] yLevels = { 0f, 0.45f, 0.9f, 1.35f, 1.8f };

        // 4 corners per level
        Vector3[] verts = new Vector3[yLevels.Length * 4];
        for (int i = 0; i < yLevels.Length; i++)
        {
            float y = yLevels[i];
            int o = i * 4;
            verts[o + 0] = new Vector3(-w, y, -d);
            verts[o + 1] = new Vector3(w, y, -d);
            verts[o + 2] = new Vector3(w, y, d);
            verts[o + 3] = new Vector3(-w, y, d);
        }

        // Triangles
        var tris = new List<int>();

        // Bottom cap (level 0)
        tris.Add(0); tris.Add(2); tris.Add(1);
        tris.Add(0); tris.Add(3); tris.Add(2);

        // Top cap (last level)
        int top = (yLevels.Length - 1) * 4;
        tris.Add(top + 0); tris.Add(top + 1); tris.Add(top + 2);
        tris.Add(top + 0); tris.Add(top + 2); tris.Add(top + 3);

        // Sides between each band
        for (int i = 0; i < yLevels.Length - 1; i++)
        {
            int a = i * 4;
            int b = (i + 1) * 4;

            // Front
            tris.Add(a + 0); tris.Add(b + 0); tris.Add(b + 1);
            tris.Add(a + 0); tris.Add(b + 1); tris.Add(a + 1);

            // Right
            tris.Add(a + 1); tris.Add(b + 1); tris.Add(b + 2);
            tris.Add(a + 1); tris.Add(b + 2); tris.Add(a + 2);

            // Back
            tris.Add(a + 2); tris.Add(b + 2); tris.Add(b + 3);
            tris.Add(a + 2); tris.Add(b + 3); tris.Add(a + 3);

            // Left
            tris.Add(a + 3); tris.Add(b + 3); tris.Add(b + 0);
            tris.Add(a + 3); tris.Add(b + 0); tris.Add(a + 0);
        }

        Mesh m = new Mesh();
        m.name = "DummyBodyMesh_Segmented";
        m.vertices = verts;
        m.triangles = tris.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    void AssignSingleBone(Mesh m)
    {
        // One bone influences all vertices 100%
        var bw = new BoneWeight[m.vertexCount];
        for (int i = 0; i < bw.Length; i++)
        {
            bw[i].boneIndex0 = 0;
            bw[i].weight0 = 1f;
        }
        m.boneWeights = bw;

        // One bindpose (identity works for our dummy)
        m.bindposes = new Matrix4x4[] { Matrix4x4.identity };
    }

    void AddBlendshapes(Mesh m)
    {
        Vector3[] baseV = m.vertices;

        // These ranges now match the new yLevels:
        // yLevels = 0, 0.45, 0.9, 1.35, 1.8

        // Bust hits band around 1.35
        AddRangeWidthShape(m, baseV, bsBust, yMin: 1.20f, yMax: 1.50f, maxDelta: 0.18f);

        // Waist hits band around 0.9
        AddRangeWidthShape(m, baseV, bsWaist, yMin: 0.75f, yMax: 1.05f, maxDelta: -0.18f);

        // Hips hits band around 0.45
        AddRangeWidthShape(m, baseV, bsHips, yMin: 0.30f, yMax: 0.60f, maxDelta: 0.18f);

        // NoseWidth hits top band around 1.8
        AddRangeDepthShape(m, baseV, bsNoseWidth, yMin: 1.65f, yMax: 1.80f, maxDelta: 0.12f);
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
                float dir = Mathf.Sign(baseV[i].x); // left/right
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
