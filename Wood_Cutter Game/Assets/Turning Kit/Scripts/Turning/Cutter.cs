using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cutter : MonoBehaviour
{
    public GameObject chiselPrefab;                     // 3d model of a chisel handle
    public Material MaterialChisel;                     // Chisel 3d model cutting material
    public List<Vector2> contour = new List<Vector2>(); // contour of a chisel point
    public Vector2 Size = new Vector2(0.5f, 2);         // chisel size

    float maxY;
    float minAngle = 130;                               // 3d model smoothing angle
    List<Vector2> c_tmp = new List<Vector2>();          // temporary variable for forming a chisel
    List<Vector2> all_contour = new List<Vector2>();    // the whole contour of the chisels
    List<Vector2> contourMin3D = new List<Vector2>();   // contour with a shift inward
    List<float> angle3D = new List<float>();            // angles for each point in the contour

    Mesh mesh = null;                                   // 3D model
    List<Vector3> vertices = new List<Vector3>();       // points 3d models
    List<int> triangles = new List<int>();              // triangulation


    private void Awake()
    {
        GetAllContour();
        Instantiate(chiselPrefab, transform.position, Quaternion.Euler(0, 0, 180), transform);
        Build3D();

        maxY = float.MinValue;
        foreach (Vector2 v2 in all_contour)
        {
            if (v2.y > maxY)
                maxY = v2.y;
        }
        maxY = maxY - transform.position.y;
    }

    // lower part of the contour of the chisel
    public List<Vector2> GetBottomCutter()
    {
        c_tmp.Clear();

        c_tmp.Add(new Vector2(Size.x, Size.y));
        c_tmp.Add(new Vector2(Size.x, 0));
        c_tmp.Add(new Vector2(-Size.x, 0));
        c_tmp.Add(new Vector2(-Size.x, Size.y));

        return c_tmp;
    }

    // returns the full contour of the chisel
    public List<Vector2> GetAllContour()
    {
        all_contour.Clear();

        GetBottomCutter();

        Vector2 nowPos = transform.position;

        all_contour.Add(nowPos + c_tmp[2]);
        all_contour.Add(nowPos + c_tmp[3]);

        foreach (Vector2 v2 in contour)
        {
            all_contour.Add(nowPos + v2);
        }

        all_contour.Add(nowPos + c_tmp[0]);
        all_contour.Add(nowPos + c_tmp[1]);

        if (!Expantions.HasСlockwise(all_contour))
            all_contour.Reverse();

        return all_contour;
    }

    // forms a 3d model of a chisel
    void Build3D()
    {
        if (mesh == null)
        {
            mesh = gameObject.AddComponent<MeshFilter>().mesh;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = MaterialChisel;
        }

        contourMin3D.Clear();
        angle3D.Clear();

        Vector2 prev;
        Vector2 next;
        float a;
        Vector2 new_offset;
        Vector3 cross;
        float delta_h = Size.x / 2;
        Vector3 offset = new Vector3(0, 0, 0.05f);
        Vector3 offsetX = new Vector3(delta_h, 0, 0);
        Vector3 offsetRight = (Quaternion.Euler(0, 0, -45) * Vector3.down).normalized * delta_h;
        Vector3 offsetLeft = (Quaternion.Euler(0, 0, 45) * Vector3.down).normalized * delta_h;

        #region Контр сверху стамески 
        for (int i = 0; i < contour.Count; i++)
        {
            if (i == 0)
                prev = c_tmp[3];
            else
                prev = contour[i - 1];

            if (i == contour.Count - 1)
                next = c_tmp[0];
            else
                next = contour[i + 1];

            a = Vector2.Angle(prev - contour[i], next - contour[i]) / 2;
            angle3D.Add(a * 2);
            cross = Vector3.Cross(prev - contour[i], next - contour[i]);
            if (cross.z <= 0)
                a = 360 - a;
            if ((prev.y + next.y) / 2f > contour[i].y)
                a = a - 180;
            new_offset = Quaternion.Euler(0, 0, a) * ((prev - contour[i]).normalized * delta_h);

            contourMin3D.Add(contour[i] + new_offset);
        }
        #endregion

        vertices.Clear();
        triangles.Clear();
        int n = 0;

        #region Низ стамески

        vertices.Add((Vector3)c_tmp[0] + offset);
        vertices.Add((Vector3)c_tmp[1] + offset);
        vertices.Add((Vector3)c_tmp[2] + offset);
        vertices.Add((Vector3)c_tmp[3] + offset);

        MakeMesh(0, 1, 2, 3);

        for (int i = 0; i < contour.Count; i++)
        {
            if (!(i == 0 || i == contour.Count - 1))
                vertices.Add(new Vector3(contour[i].x, c_tmp[3].y, 0) + offset);
            vertices.Add((Vector3)contour[i] + offset);

            if (contour.Count == 1)
                MakeMesh(0, 3, vertices.Count - 1);
            else if (i == contour.Count - 1 && i - 1 == 0)
                MakeMesh(0, 3, vertices.Count - 2, vertices.Count - 1);
            else if (i == contour.Count - 1)
                MakeMesh(i * 2 + 1, i * 2 + 2, i * 2 + 3, 0);
            else if (i > 0)
                MakeMesh(i * 2 + 1, i * 2 + 2, i * 2 + 4, i * 2 + 3);
        }

        #endregion 

        #region Боковая часть стамески

        n = vertices.Count;

        vertices.Add((Vector3)c_tmp[1] + offset);
        vertices.Add((Vector3)c_tmp[2] + offset);
        vertices.Add((Vector3)c_tmp[1] - offset - offsetX);
        vertices.Add((Vector3)c_tmp[2] - offset + offsetX);

        MakeMesh(n + 1, n + 0, n + 2, n + 3);

        n = vertices.Count;

        vertices.Add((Vector3)c_tmp[0] + offset);
        vertices.Add((Vector3)c_tmp[1] + offset);
        vertices.Add((Vector3)c_tmp[2] + offset);
        vertices.Add((Vector3)c_tmp[3] + offset);

        vertices.Add((Vector3)c_tmp[0] - offset + offsetRight);
        vertices.Add((Vector3)c_tmp[1] - offset - offsetX);
        vertices.Add((Vector3)c_tmp[2] - offset + offsetX);
        vertices.Add((Vector3)c_tmp[3] - offset + offsetLeft);

        MakeMesh(n + 4, n + 5, n + 1, n + 0);
        MakeMesh(n + 3, n + 2, n + 6, n + 7);

        if (contourMin3D.Count == 0)
        {
            n = vertices.Count;

            vertices.Add((Vector3)c_tmp[0] + offset);
            vertices.Add((Vector3)c_tmp[3] + offset);
            vertices.Add((Vector3)c_tmp[0] - offset + offsetRight);
            vertices.Add((Vector3)c_tmp[3] - offset + offsetLeft);

            MakeMesh(n + 0, n + 1, n + 3, n + 2);
        }
        else
        {
            a = Mathf.Abs(Vector2.Angle(contour.First() - c_tmp[3], c_tmp[2] - c_tmp[3]));
            if (a <= minAngle)
            {
                vertices.Add((Vector3)c_tmp[3] + offset);
                vertices.Add((Vector3)c_tmp[3] - offset + offsetLeft);
            }
            vertices.Add((Vector3)contour.First() + offset);
            vertices.Add((Vector3)contourMin3D.First() - offset);
            if (a <= minAngle)
                MakeMesh(vertices.Count - 4, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2);
            else
                MakeMesh(n + 3, n + 7, vertices.Count - 1, vertices.Count - 2);

            for (int i = 1; i < contourMin3D.Count; i++)
            {
                if (Mathf.Abs(angle3D[i - 1]) <= minAngle)
                {
                    vertices.Add((Vector3)contour[i - 1] + offset);
                    vertices.Add((Vector3)contourMin3D[i - 1] - offset);
                }
                vertices.Add((Vector3)contour[i] + offset);
                vertices.Add((Vector3)contourMin3D[i] - offset);

                if (contourMin3D.Count == 1)
                    MakeMesh(vertices.Count - 2, vertices.Count - 1, n + 4, n + 0);
                else
                    MakeMesh(vertices.Count - 4, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2);
            }

            if (Mathf.Abs(angle3D.Last()) <= minAngle)
            {
                vertices.Add((Vector3)contour.Last() + offset);
                vertices.Add((Vector3)contourMin3D.Last() - offset);
            }
            if (Mathf.Abs(Vector2.Angle(contour.Last() - c_tmp[0], c_tmp[1] - c_tmp[0])) <= minAngle)
            {
                vertices.Add((Vector3)c_tmp[0] + offset);
                vertices.Add((Vector3)c_tmp[0] - offset + offsetRight);
                MakeMesh(vertices.Count - 4, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2);
            }
            else
                MakeMesh(n + 4, n + 0, vertices.Count - 2, vertices.Count - 1);
        }

        #endregion

        #region Верхняя часть стамески

        n = vertices.Count;

        vertices.Add((Vector3)c_tmp[0] - offset + offsetRight);
        vertices.Add((Vector3)c_tmp[1] - offset - offsetX);
        vertices.Add((Vector3)c_tmp[2] - offset + offsetX);
        vertices.Add((Vector3)c_tmp[3] - offset + offsetLeft);

        MakeMesh(n + 3, n + 2, n + 1, n + 0);

        for (int i = 0; i < contourMin3D.Count; i++)
        {
            if (!(i == 0 || i == contourMin3D.Count - 1))
                vertices.Add(new Vector3(contourMin3D[i].x, c_tmp[3].y + offsetRight.y, 0) - offset);
            vertices.Add((Vector3)contourMin3D[i] - offset);

            if (contour.Count == 1)
                MakeMesh(n + 3, n + 0, vertices.Count - 1);
            else if (i == contour.Count - 1 && i - 1 == 0)
                MakeMesh(n + 3, n + 0, vertices.Count - 1, vertices.Count - 2);
            else if (i == contour.Count - 1)
                MakeMesh(n + 0, n + i * 2 + 3, n + i * 2 + 2, n + i * 2 + 1);
            else if (i > 0)
                MakeMesh(n + i * 2 + 3, n + i * 2 + 4, n + i * 2 + 2, n + i * 2 + 1);
        }

        #endregion 

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.subMeshCount = 1;
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
    }

    // Method to generate triangle mesh from the 3 points
    void MakeMesh(int id0, int id1, int id2)
    {
        triangles.Add(id0);
        triangles.Add(id2);
        triangles.Add(id1);
    }

    // Method to generate 2 triangles mesh from the 4 points 0 1 2 3
    void MakeMesh(int id0, int id1, int id2, int id3)
    {
        MakeMesh(id0, id1, id2);
        MakeMesh(id0, id2, id3);
    }

    // returns the maximum height of the chisel
    public float GetMaxY()
    {
        return maxY - 0.02f;
    }
}