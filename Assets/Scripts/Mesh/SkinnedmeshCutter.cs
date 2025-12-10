using System.Collections.Generic;
using UnityEngine;

public class SkinnedmeshCutter : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    // 키워드로 부위 분류
    string[] headKeywords = { "head", "neck", "headEnd" };
    string[] spineKeywords = { "spine" };
    string[] rightElbowKeywords = { "" };
    string[] rightArmKeywords = { "head", "neck", "headEnd" };
    string[] leftElbowKeywords = { "" };
    string[] leftArmKeywords = { "head", "neck", "headEnd" };
    string[] rightFootKeywords = { "" };
    string[] rightKneeKeywords = { "" };
    string[] rightHipsKeywords = { "" };
    string[] leftFootKeywords = { "" };
    string[] leftKneeKeywords = { "" };
    string[] leftHipsKeywords = { "" };    
    string[] pelvisKeywords = { "spine" };

    [ContextMenu("Bake and Cut Mesh Into Parts")]
    void BakeAndCut()
    {
        Mesh bakedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakedMesh);

        Mesh originalMesh = skinnedMeshRenderer.sharedMesh;
        BoneWeight[] boneWeights = originalMesh.boneWeights;
        Transform[] bones = skinnedMeshRenderer.bones;

        Vector3[] vertices = bakedMesh.vertices;
        Vector3[] normals = bakedMesh.normals;
        Vector2[] uvs = bakedMesh.uv;
        int[] triangles = bakedMesh.triangles;

        Dictionary<string, List<int>> partVertices = new Dictionary<string, List<int>>()
        {
            { "Head", new List<int>() },
            { "MiddleSpine", new List<int>() },
            { "RightElbow", new List<int>() },
            { "RightArm", new List<int>() },
            { "LeftElbow", new List<int>() },
            { "LeftArm", new List<int>() },
            { "RightFoot", new List<int>() },
            { "RightKnee", new List<int>() },
            { "RightHips", new List<int>() },
            { "LeftFoot", new List<int>() },
            { "LeftKnee", new List<int>() },
            { "Pelvis", new List<int>() }
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            BoneWeight bw = boneWeights[i];
            int dominantBone = GetDominantBoneIndex(bw);
            string boneName = bones[dominantBone].name;

            if (ContainsAny(boneName, headKeywords))
                partVertices["Head"].Add(i);
            else if (ContainsAny(boneName, spineKeywords))
                partVertices["MiddleSpine"].Add(i);
            else if (ContainsAny(boneName, rightElbowKeywords))
                partVertices["RightElbow"].Add(i);
            else if (ContainsAny(boneName, rightArmKeywords))
                partVertices["RightArm"].Add(i);
            else if (ContainsAny(boneName, leftElbowKeywords))
                partVertices["LeftElbow"].Add(i);
            else if (ContainsAny(boneName, leftArmKeywords))
                partVertices["LeftArm"].Add(i);
            else if (ContainsAny(boneName, rightFootKeywords))
                partVertices["RightFoot"].Add(i);
            else if (ContainsAny(boneName, rightKneeKeywords))
                partVertices["RightKnee"].Add(i);
            else if (ContainsAny(boneName, rightHipsKeywords))
                partVertices["RightHips"].Add(i);
            else if (ContainsAny(boneName, leftFootKeywords))
                partVertices["LeftFoot"].Add(i);
            else if (ContainsAny(boneName, leftKneeKeywords))
                partVertices["LeftKnee"].Add(i);
            else if (ContainsAny(boneName, leftHipsKeywords))
                partVertices["LeftHips"].Add(i);
            else
                partVertices["Pelvis"].Add(i);
        }

        // 오브젝트 생성
        GameObject parentObj = new GameObject("NewSeparatedCharacter");
        parentObj.transform.position = transform.position;
        parentObj.transform.rotation = transform.rotation;
        parentObj.transform.localScale = transform.localScale;


        foreach (var kv in partVertices)
        {
            string partName = kv.Key;
            List<int> partVerts = kv.Value;

            if (partVerts.Count == 0)
            {
                Debug.Log($"정점없 {partName}");
                continue;
            }


            Dictionary<int, int> oldToNewIndex = new Dictionary<int, int>();
            List<Vector3> newVertices = new List<Vector3>();
            List<Vector3> newNormals = new List<Vector3>();
            List<Vector2> newUVs = new List<Vector2>();

            for (int i = 0; i < partVerts.Count; i++)
            {
                int oldIndex = partVerts[i];
                oldToNewIndex[oldIndex] = i;
                newVertices.Add(vertices[oldIndex]);
                newNormals.Add(normals[oldIndex]);
                newUVs.Add(uvs[oldIndex]);
            }

            List<int> newTriangles = new List<int>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i0 = triangles[i];
                int i1 = triangles[i + 1];
                int i2 = triangles[i + 2];

                if (oldToNewIndex.ContainsKey(i0) && oldToNewIndex.ContainsKey(i1) && oldToNewIndex.ContainsKey(i2))
                {
                    newTriangles.Add(oldToNewIndex[i0]);
                    newTriangles.Add(oldToNewIndex[i1]);
                    newTriangles.Add(oldToNewIndex[i2]);
                }
            }

            if (newTriangles.Count == 0)
            {
                Debug.Log($"삼각형안됨 {partName}");
                continue;
            }

            Mesh partMesh = new Mesh();
            partMesh.vertices = newVertices.ToArray();
            partMesh.normals = newNormals.ToArray();
            partMesh.uv = newUVs.ToArray();
            partMesh.triangles = newTriangles.ToArray();
            partMesh.RecalculateBounds();

            GameObject partObj = new GameObject(partName);
            partObj.transform.parent = parentObj.transform;
            partObj.transform.localPosition = Vector3.zero;
            partObj.transform.localRotation = Quaternion.identity;
            partObj.transform.localScale = Vector3.one;

            MeshFilter mf = partObj.AddComponent<MeshFilter>();
            mf.mesh = partMesh;

            MeshRenderer mr = partObj.AddComponent<MeshRenderer>();
            mr.material = skinnedMeshRenderer.sharedMaterial;

            Debug.Log($"Created part: {partName} with {newVertices.Count} vertices");
        }

        Debug.Log("만듦@!!");
    }

    int GetDominantBoneIndex(BoneWeight bw)
    {
        float maxWeight = bw.weight0;
        int index = bw.boneIndex0;

        if (bw.weight1 > maxWeight) { maxWeight = bw.weight1; index = bw.boneIndex1; }
        if (bw.weight2 > maxWeight) { maxWeight = bw.weight2; index = bw.boneIndex2; }
        if (bw.weight3 > maxWeight) { maxWeight = bw.weight3; index = bw.boneIndex3; }

        return index;
    }

    bool ContainsAny(string boneName, string[] keywords)
    {
        string cleanedName = CleanBoneName(boneName);

        foreach (var k in keywords)
            if (cleanedName.Contains(k.ToLower()))
                return true;

        return false;
    }
    string CleanBoneName(string name)
    {
        name = name.ToLower();

        if (name.StartsWith("def-"))
            name = name.Substring(4);

        name = name.Replace(".l", "").Replace(".r", "");
        name = name.Replace("_l", "").Replace("_r", "");

        int dotIndex = name.IndexOf('.');
        if (dotIndex >= 0)
            name = name.Substring(0, dotIndex);

        return name;
    }

    [ContextMenu("Print All Bone Names")]
    void PrintAllBoneNames()
    {
        Transform[] bones = skinnedMeshRenderer.bones;
        for (int i = 0; i < bones.Length; i++)
        {
            Debug.Log($"Bone {i}: {bones[i].name}");
        }
    }
}