using System;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeDriver : MonoBehaviour
{
    public SkinnedMeshRenderer target;

    private Dictionary<string, int> _indexByName;

    void Awake() => BuildCache();

    public void BuildCache()
    {
        _indexByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        if (target == null || target.sharedMesh == null) return;

        var mesh = target.sharedMesh;
        int count = mesh.blendShapeCount;

        for (int i = 0; i < count; i++)
        {
            string name = mesh.GetBlendShapeName(i);
            if (!_indexByName.ContainsKey(name))
                _indexByName.Add(name, i);
        }
    }

    public void Set(string blendShapeName, float weight01_100)
    {
        if (target == null || target.sharedMesh == null) return;
        if (_indexByName == null || _indexByName.Count == 0) BuildCache();
        if (string.IsNullOrWhiteSpace(blendShapeName)) return;

        if (_indexByName.TryGetValue(blendShapeName, out int idx))
            target.SetBlendShapeWeight(idx, Mathf.Clamp(weight01_100, 0f, 100f));
    }

    public float Get(string blendShapeName)
    {
        if (target == null || target.sharedMesh == null) return 0f;
        if (_indexByName == null || _indexByName.Count == 0) BuildCache();

        return _indexByName.TryGetValue(blendShapeName, out int idx)
            ? target.GetBlendShapeWeight(idx)
            : 0f;
    }
}
