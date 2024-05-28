/* 
 * В цьому класі знаходяться корисні методи
 * які необхідні для роботи з мешами
 */

using UnityEngine;

public static class MeshUtils
{
    private static Quaternion[] _cachedQuaternionEulerArr;

    // Цей метод кешує повороти
    private static void CacheQuaternionEuler()
    {
        if (_cachedQuaternionEulerArr != null)
        {
            return;
        }

        _cachedQuaternionEulerArr = new Quaternion[360];
        for (int i = 0; i < 360; i++)
        {
            _cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
        }
    }

    private static Quaternion GetQuaternionEuler(float rotFloat)
    {
        int rot = Mathf.RoundToInt(rotFloat) % 360;

        if (rot < 0)
        {
            rot += 360;
        }

        if (_cachedQuaternionEulerArr == null)
        {
            CacheQuaternionEuler();
        }

        return _cachedQuaternionEulerArr[rot];
    }
    
    // Цей метод створює пустий масив з мешами
    public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs,
        out int[] triangles)
    {
        vertices = new Vector3[4 * quadCount];
        uvs = new Vector2[4 * quadCount];
        triangles = new int[6 * quadCount];
    }

    // Цей метод додає меш до масиву мешів
    public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos,
        float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
    {
        baseSize *= 0.5f;
        SetMesh(vertices, uvs, triangles, baseSize, index, rot, pos, uv00, uv11);
    }
    
    // Цей метод потрібен для налаштування мешу
    private static void SetMesh(Vector3[] vertices, Vector2[] uvs, int[] triangles, Vector3 baseSize, int index,
        float rot,
        Vector3 pos, Vector2 uv00, Vector2 uv11)
    {
        int vIndex = index * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        bool skewed = baseSize.x != baseSize.y;
        if (skewed)
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
        }
        else
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
            vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
            vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
            vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
        }

        uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
        uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
        uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
        uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

        int tIndex = index * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex3;
        triangles[tIndex + 2] = vIndex1;

        triangles[tIndex + 3] = vIndex1;
        triangles[tIndex + 4] = vIndex3;
        triangles[tIndex + 5] = vIndex2;
    }
}
