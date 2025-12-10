using System.Collections.Generic;
using UnityEngine;

public class MeshSplitter : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int numberOfPieces = 5;

    void Start()
    {
        SplitMeshByBoundingBox();
    }

    void SplitMeshByBoundingBox()
    {
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;
        Material mat = GetComponent<MeshRenderer>().sharedMaterial;

        // 바운딩박스 계산
        Bounds bounds = mesh.bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        Vector3 size = bounds.size;

        // 가장 긴 축 결정
        int axis = 0;
        if (size.y >= size.x && size.y >= size.z) axis = 1;
        else if (size.z >= size.x && size.z >= size.y) axis = 2;

        float start = min[axis];
        float end = max[axis];
        float step = (end - start) / numberOfPieces;

        // 삼각형 분류
        List<int>[] groups = new List<int>[numberOfPieces];
        for (int i = 0; i < numberOfPieces; i++)
            groups[i] = new List<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];
            Vector3 center = (v0 + v1 + v2) / 3f;

            float coord = center[axis];
            int groupIndex = Mathf.Clamp((int)((coord - start) / step), 0, numberOfPieces - 1);
            groups[groupIndex].Add(triangles[i]);
            groups[groupIndex].Add(triangles[i + 1]);
            groups[groupIndex].Add(triangles[i + 2]);
        }

        // 각 조각 생성
        for (int pieceIndex = 0; pieceIndex < numberOfPieces; pieceIndex++)
        {
            List<int> pieceTriangles = groups[pieceIndex];
            if (pieceTriangles.Count == 0) continue;

            // 정점만 추출
            Dictionary<int, int> vertMap = new Dictionary<int, int>();
            List<Vector3> newVertices = new List<Vector3>();
            List<Vector2> newUVs = new List<Vector2>();
            int[] remappedTriangles = new int[pieceTriangles.Count];

            for (int i = 0; i < pieceTriangles.Count; i++)
            {
                int originalIndex = pieceTriangles[i];
                if (!vertMap.ContainsKey(originalIndex))
                {
                    vertMap[originalIndex] = newVertices.Count;
                    newVertices.Add(vertices[originalIndex]);
                    if (uvs.Length > 0) newUVs.Add(uvs[originalIndex]);
                }
                remappedTriangles[i] = vertMap[originalIndex];
            }

            // 메쉬 생성
            Mesh pieceMesh = new Mesh();
            pieceMesh.vertices = newVertices.ToArray();
            pieceMesh.triangles = remappedTriangles;
            if (uvs.Length > 0)
                pieceMesh.uv = newUVs.ToArray();

            pieceMesh.RecalculateNormals();
            pieceMesh.RecalculateBounds();

            //  단면 채우기 추가
            AddCapsToMesh(pieceMesh, axis, start + step * pieceIndex, start + step * (pieceIndex + 1));

            // 오브젝트 생성
            GameObject pieceObj = new GameObject("Piece_" + pieceIndex);
            pieceObj.transform.position = transform.position;
            pieceObj.transform.rotation = transform.rotation;
            pieceObj.transform.localScale = transform.localScale;

            MeshFilter mf = pieceObj.AddComponent<MeshFilter>();
            mf.mesh = pieceMesh;

            MeshRenderer mr = pieceObj.AddComponent<MeshRenderer>();
            mr.material = mat;
        }
    }

    void AddCapsToMesh(Mesh mesh, int axis, float frontSlice, float backSlice)
    {
        List<Vector3> verts = new List<Vector3>(mesh.vertices);
        List<int> tris = new List<int>(mesh.triangles);

        // 앞면과 뒷면 캡 생성
        AddCapFace(verts, tris, axis, frontSlice, true);
        AddCapFace(verts, tris, axis, backSlice, false);

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void AddCapFace(List<Vector3> verts, List<int> tris, int axis, float sliceCoord, bool isFront)
    {
        // 슬라이스 면 위 정점만 선택
        List<int> capIndices = new List<int>();
        List<Vector2> projectedPoints = new List<Vector2>();

        for (int i = 0; i < verts.Count; i++)
        {
            if (Mathf.Approximately(verts[i][axis], sliceCoord))
            {
                capIndices.Add(i);
                projectedPoints.Add(ProjectTo2D(verts[i], axis));
            }
        }

        if (capIndices.Count < 3) return;

        // 중심점 생성
        Vector2 center2D = Vector2.zero;
        foreach (var p in projectedPoints) center2D += p;
        center2D /= projectedPoints.Count;

        Vector3 center3D = Vector3.zero;
        for (int d = 0; d < 3; d++)
        {
            if (d == axis) center3D[d] = sliceCoord;
            else
            {
                float sum = 0f;
                foreach (var v in verts)
                    if (Mathf.Approximately(v[axis], sliceCoord))
                        sum += v[d];
                center3D[d] = sum / capIndices.Count;
            }
        }

        int centerIndex = verts.Count;
        verts.Add(center3D);

        // fan triangulation
        for (int i = 0; i < capIndices.Count; i++)
        {
            int i0 = capIndices[i];
            int i1 = capIndices[(i + 1) % capIndices.Count];

            if (isFront)
            {
                tris.Add(centerIndex);
                tris.Add(i0);
                tris.Add(i1);
            }
            else
            {
                tris.Add(centerIndex);
                tris.Add(i1);
                tris.Add(i0);
            }
        }
    }

    Vector2 ProjectTo2D(Vector3 v, int axis)
    {
        if (axis == 0) return new Vector2(v.y, v.z);
        if (axis == 1) return new Vector2(v.x, v.z);
        return new Vector2(v.x, v.y);
    }
}
