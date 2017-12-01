// https://github.com/markv12
using System.Linq;
using UnityEngine;

public class AverageMeshNormals : MonoBehaviour
{
    public MeshFilter[] MeshSources;
    private static readonly Vector3 _zeroVec = Vector3.zero;

    private void Awake()
    {
        foreach (MeshFilter meshSource in MeshSources)
        {
            Vector3[] verts = meshSource.mesh.vertices;
            Vector3[] normals = meshSource.mesh.normals;
            VertInfo[] vertInfo = new VertInfo[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                vertInfo[i] = new VertInfo()
                {
                    vert = verts[i],
                    origIndex = i,
                    normal = normals[i]
                };
            }
            var groups = vertInfo.GroupBy(x => x.vert);
            VertInfo[] processedVertInfo = new VertInfo[vertInfo.Length];
            int index = 0;
            foreach (IGrouping<Vector3, VertInfo> group in groups)
            {
                Vector3 avgNormal = _zeroVec;
                foreach (VertInfo item in group)
                {
                    avgNormal += item.normal;
                }
                avgNormal /= groups.Count();
                foreach (VertInfo item in group)
                {
                    processedVertInfo[index] = new VertInfo()
                    {
                        vert = item.vert,
                        origIndex = item.origIndex,
                        normal = item.normal,
                        averagedNormal = avgNormal
                    };
                    index++;
                }
            }
            Color[] colors = new Color[verts.Length];
            for (int i = 0; i < processedVertInfo.Length; i++)
            {
                VertInfo info = processedVertInfo[i];

                int origIndex = info.origIndex;
                Vector3 normal = info.averagedNormal;
                Color normColor = new Color(normal.x, normal.y, normal.z, 1);
                colors[origIndex] = normColor;
            }
            meshSource.mesh.colors = colors;
        }
    }

    private struct VertInfo
    {
        public Vector3 vert;
        public int origIndex;
        public Vector3 normal;
        public Vector3 averagedNormal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; MeshSources[0].sharedMesh.colors.Length > 0 && i < MeshSources[0].sharedMesh.vertices.Length; i++)
        {
            Color c = MeshSources[0].sharedMesh.colors[i];
            Vector3 worldDirection = new Vector3(c.r, c.g, c.b);
            worldDirection = transform.TransformDirection(worldDirection);
            Vector3 worldPoint = transform.TransformPoint(MeshSources[0].sharedMesh.vertices[i]);
            Gizmos.DrawLine(worldPoint, worldPoint + worldDirection);
        }
    }
}