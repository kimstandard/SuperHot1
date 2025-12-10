using UnityEngine;

public class MeshMaker : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float explosionForce = 300f;
    public float explosionRadius = 5f;
    public float upwardsModifier = 0.5f;
    public bool useConvex = true;

    void Start()
    {
        SplitMeshIntoPyramids();
    }

    void SplitMeshIntoPyramids()
    {
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        // 중심 계산 (조각 중심점)
        Vector3 center = Vector3.zero;
        foreach (var v in vertices) center += v;
        center /= vertices.Length;

        Vector2 centerUV = Vector2.zero;
        foreach (var uv in uvs) centerUV += uv;
        centerUV /= uvs.Length;

        // 기존 메터리얼
        Material mat = GetComponent<MeshRenderer>()?.sharedMaterial;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            // 삼각형 인덱스
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];


            // 사면체 꼭짓점
            Vector3[] pyramidVerts = new Vector3[]
            {
                vertices[i0],
                vertices[i1],
                vertices[i2],
                center
            };

            Vector2[] pyramidUVs = new Vector2[]
            {
                uvs[i0],
                uvs[i1],
                uvs[i2],
                centerUV
            };

            int[] pyramidTris = new int[]
            {
                0, 1, 2,
                0, 3, 1,
                1, 3, 2,
                2, 3, 0
            };

            // 메쉬 생성
            Mesh pyramidMesh = new Mesh();
            pyramidMesh.vertices = pyramidVerts;
            pyramidMesh.uv = pyramidUVs;
            pyramidMesh.triangles = pyramidTris;
            pyramidMesh.RecalculateNormals();

            // 게임오브젝트 생성
            GameObject pyramidObj = new GameObject("Pyramid_" + (i / 3));
            pyramidObj.transform.position = transform.position;
            pyramidObj.transform.rotation = transform.rotation;
            pyramidObj.transform.localScale = transform.localScale;

            // 메쉬필터 + 렌더러
            MeshFilter mf = pyramidObj.AddComponent<MeshFilter>();
            mf.mesh = pyramidMesh;

            MeshRenderer mr = pyramidObj.AddComponent<MeshRenderer>();
            mr.material = mat;

            //  Collider 추가
            MeshCollider collider = pyramidObj.AddComponent<MeshCollider>();
            collider.sharedMesh = pyramidMesh;
            collider.convex = useConvex;   // Convex = true 로 설정하면 Rigidbody와 호환됨 (Unity 5에서도 지원)

            //  Rigidbody 추가
            Rigidbody rb = pyramidObj.AddComponent<Rigidbody>();
            rb.mass = 1f;

            //  폭발 힘
            Vector3 explosionPos = transform.position;
            rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }
    }
}
