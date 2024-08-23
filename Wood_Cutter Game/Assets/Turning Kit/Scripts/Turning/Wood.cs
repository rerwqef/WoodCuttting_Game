using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Wood : MonoBehaviour
{
    public Vector2 Size = new Vector2(2, 1);                        // Bar size
    public Material MaterialIn;                                     // External material
    public Material MaterialOut;                                    // Inner material

    List<Vector2> contour = new List<Vector2>();                    // bar contour
    List<Vector2> all_contour = new List<Vector2>();                // bar circuit with the indentation
    List<Vector2> all_contour_tmp = new List<Vector2>();            // temporary variable is needed for cutting in the contour
    List<Vector2> contour_tmp = new List<Vector2>();                // temporary variable is needed for cutting in the contour
    List<List<Vector2>> contour_DEL = new List<List<Vector2>>();    // excluded contours, 3d models are formed on them that will be discarded for deletion

    List<Vector3> vertices = new List<Vector3>();                   // points 3d models
    List<int> triangles = new List<int>();                          // internal triangulation
    List<int> trianglesTOP = new List<int>();                       // external triangulation

    Mesh mesh = null;                                               // 3D model

    public bool isCutting=false;
    private void Awake()
    {
        /*  ContourWoodReset();
          GetAllContour();
          Create3D();*/
        CreateWood();

    }
    public void CreateWood()
    {
        ContourWoodReset();
        GetAllContour();
        Create3D();
    }
    // forms a contour according to the size of the bar
    void ContourWoodReset()
    {
        contour.Clear();

        contour.Add(new Vector2(-Size.x, 0));
        contour.Add(new Vector2(-Size.x, -Size.y));
        contour.Add(new Vector2(Size.x, -Size.y));
        contour.Add(new Vector2(Size.x, 0));
    }

    // indent contour
    void GetAllContour()
    {
        all_contour.Clear();

        foreach (Vector2 v2 in contour)
        {
            all_contour.Add((Vector2)transform.position + v2);
        }

        if (Expantions.HasСlockwise(all_contour))
            all_contour.Reverse();
    }

    // function of cutting in the contour and forming a 3d model
    public bool CutContour(List<Vector2> contour_cut)
    {
       
        bool isEdit = false;

        GetAllContour();

        if (all_contour.Count > 0)
        {
            Bounds bounds_contour = new Bounds();
            bounds_contour.SetMinMax(new Vector3(all_contour.Min(v => v.x), all_contour.Min(v => v.y), 0), new Vector3(all_contour.Max(v => v.x), all_contour.Max(v => v.y), 0));
            Bounds bounds_remove = new Bounds();
            bounds_remove.SetMinMax(new Vector3(contour_cut.Min(v => v.x), contour_cut.Min(v => v.y), 0), new Vector3(contour_cut.Max(v => v.x), contour_cut.Max(v => v.y), 0));

            if (bounds_contour.Intersects(bounds_remove))
            {
              
                if (_cut(contour_cut))
                {
                    isEdit = true;

                    Create3D();
                }
            }
        }
      
        return isEdit;
    }
 
    // cut in the contour
    bool _cut(List<Vector2> contour_cut)
    {
        isCutting = true;
        bool isEdit = false;

        int i_prev;
        int j_prev;
        int j_next;
        Vector2 pR0;
        Vector2 pR1;
        Vector2 pB0;
        Vector2 pB1;
        Vector2 crossPoint;
        bool isIn;
        bool isIn_prev;
        int last_i;
        float dist;
        int new_i;
        float x0;
        bool isErr = false;


        all_contour_tmp.Clear();
        contour_tmp.Clear();
        contour_DEL.Clear();


        i_prev = all_contour.Count - 1;
        isIn_prev = i_prev >= 0 ? Expantions.HasIntersections(all_contour[i_prev], contour_cut) : false;
        // Update the main contour (remove points from it that fell into the cutting contour and add intersection points with the cutting contour)
        for (int i = 0; i <= all_contour.Count; i++)
        {
            pR0 = all_contour[i_prev];
            pR1 = all_contour[i >= all_contour.Count ? 0 : i];

            if (!isIn_prev && !(i_prev == all_contour.Count - 1 && i == 0))
                all_contour_tmp.Add(pR0);

            isIn = Expantions.HasIntersections(pR1, contour_cut);
            if (!(i_prev == all_contour.Count - 1 && i == 0)
                || (i_prev == all_contour.Count - 1 && i == 0 && isIn))
            {
                last_i = all_contour_tmp.Count - 1;

                j_prev = contour_cut.Count - 1;
                for (int j = 0; j < contour_cut.Count; j++)
                {
                    pB0 = contour_cut[j_prev];
                    pB1 = contour_cut[j];

                    crossPoint = Vector2.zero;
                    if (Expantions.GetCross(pR0, pR1, pB0, pB1, ref crossPoint))
                    {
                        if (!Expantions.Equals(crossPoint, pR0) && !Expantions.Equals(crossPoint, pR1)
                            && ((all_contour_tmp.Count > 0 && !Expantions.Equals(crossPoint, all_contour_tmp.First()) && !Expantions.Equals(crossPoint, all_contour_tmp.Last()))
                                || all_contour_tmp.Count == 0)
                            )
                        {
                            new_i = -1;
                            dist = Vector2.Distance(pR0, crossPoint);
                            for (int p = last_i + 1; p < all_contour_tmp.Count; p++)
                            {
                                if (Vector2.Distance(pR0, all_contour_tmp[p]) > dist)
                                {
                                    new_i = p;
                                    break;
                                }
                            }
                            if (new_i == -1)
                                all_contour_tmp.Add(crossPoint);
                            else if (!Expantions.Equals(crossPoint, all_contour_tmp[new_i]) && !Expantions.Equals(crossPoint, all_contour_tmp[new_i - 1 < 0 ? all_contour_tmp.Count - 1 : new_i - 1]))
                                all_contour_tmp.Insert(new_i, crossPoint);
                        }

                        if (!Expantions.Equals(crossPoint, pB0) && !Expantions.Equals(crossPoint, pB1)
                            && !Expantions.Equals(crossPoint, contour_cut.First()) && !Expantions.Equals(crossPoint, contour_cut.Last()))
                        {
                            contour_cut.Insert(j, crossPoint);
                            j++;
                        }
                    }

                    j_prev = j;
                }
            }

            i_prev = i;
            isIn_prev = isIn;
        }

        // Remove points from the cutting contour that do not fall or do not intersect with the main contour
        for (int i = 0; i < contour_cut.Count; i++)
        {
            i_prev = i - 1 < 0 ? contour_cut.Count - 1 : i - 1;
            if (//i > 0 && 
                (!Expantions.HasIntersections(contour_cut[i], all_contour, true)
                || Expantions.Equals(contour_cut[i], contour_cut[i_prev])))
            {
                contour_cut.RemoveAt(i);
                i--;
            }
            else if (i == 0 &&
                (!Expantions.HasIntersections(contour_cut[i_prev], all_contour, true)
                || Expantions.Equals(contour_cut[i], contour_cut[i_prev])))
            {
                contour_cut.RemoveAt(i_prev);
                i--;
            }
        }



        // We form a new contour
        for (int i = 0; i < all_contour_tmp.Count; i++)
        {
            Vector2 v2_0 = all_contour_tmp[i];
            Vector2 v2_1 = all_contour_tmp[i + 1 < all_contour_tmp.Count ? i + 1 : 0];
            int j_start = Expantions.GetIndex(v2_0, contour_cut);
            int j_end = Expantions.GetIndex(v2_1, contour_cut);

            contour_tmp.Add(v2_0);
            all_contour_tmp.RemoveAt(i);
            i--;

            if (j_start != -1 && j_end != -1)
            {
                if (j_start < j_end)
                {
                    if (j_start + 1 < j_end)
                        contour_tmp.AddRange(contour_cut.GetRange(j_start + 1, j_end - (j_start + 1)));
                    contour_cut.RemoveRange(j_start, j_end - j_start);
                }
            }
        }



        // Check the contour so that it does not have the same points in a row
        // and the point should not lie on the line between the previous point and the next
        // (remove such extra points)
        if (contour_tmp.Count > 0)
        {
            if (!Expantions.Equals(contour_tmp.First().y, transform.position.y))
                contour_tmp.Insert(0, new Vector2(contour_tmp.First().x, transform.position.y));
            if (!Expantions.Equals(contour_tmp.Last().y, transform.position.y))
                contour_tmp.Add(new Vector2(contour_tmp.Last().x, transform.position.y));

            for (int j = 0; j < contour_tmp.Count; j++)
            {
                j_prev = j - 1 < 0 ? contour_tmp.Count - 1 : j - 1;
                j_next = j + 1 > contour_tmp.Count - 1 ? 0 : j + 1;

                if (contour_tmp[j].x > contour_tmp[j_next].x && j < contour_tmp.Count - 1)
                {
                    contour_tmp[j_next] = new Vector2(contour_tmp[j].x, contour_tmp[j_next].y);
                }

                if (Expantions.Equals(contour_tmp[j], contour_tmp[j_prev])                              // If current and previous point are equal
                    || Expantions.Equals(contour_tmp[j], contour_tmp[j_next])                           // If the current and next point are equal
                    || Expantions.Equals(contour_tmp[j_prev], contour_tmp[j_next])                      // If the previous and next point are equal
                    || Expantions.HasPointLies(contour_tmp[j_prev], contour_tmp[j_next], contour_tmp[j])// the point lies on the line between the previous point and the next)
                    || (j >= 2 && j < contour_tmp.Count - 2
                        && Vector2.Distance(contour_tmp[j_prev], contour_tmp[j]) <= 0.1f                // If the distance is very small between adjacent points
                        && Vector2.Distance(contour_tmp[j], contour_tmp[j_next]) <= 0.1f)
                    )
                {
                    contour_tmp.RemoveAt(j);
                    j--;
                }
            }

            // If one of the points of the cutting contour is inside the new contour, then this contour is considered erroneous
            foreach (Vector2 v2 in contour_cut)
            {
                if (Expantions.HasIntersections(v2, contour_tmp))
                {
                    isErr = true;
                    break;
                }
            }

            // If the new contour does not have a point of contact with the cutting contour, then we consider such a contour erroneous
            bool isYes = false;
            foreach (Vector2 v2 in contour_tmp)
            {
                foreach (Vector2 v2_2 in contour_cut)
                {
                    if (Vector2.Distance(v2, v2_2) <= 0.1f)
                    {
                        isYes = true;
                        break;
                    }
                }
                if (isYes)
                    break;
            }
            if (!isYes)
                isErr = true;

            if (!isErr)
            {
                isEdit = true;

                x0 = (contour_tmp.Last().x + contour_tmp.First().x) / 2.0f;
                for (int j = 2; j < contour_tmp.Count - 3; j++)
                {
                    j_next = j + 1 > contour_tmp.Count - 1 ? 0 : j + 1;
                    bool isFirst = Mathf.Abs(contour_tmp[j].y - transform.position.y) <= 0.01f;
                    bool isTwo = isFirst && Mathf.Abs(contour_tmp[j].y - contour_tmp[j_next].y) <= 0.01f;
                    if (isFirst || isTwo) // If the circuit needs to be divided into two parts
                    {
                        if (Mathf.Abs(contour_tmp[j].x - x0) < Mathf.Abs(contour_tmp[j_next].x - x0))
                        {
                            contour_DEL.Add(new List<Vector2>());
                            contour_DEL.Last().AddRange(contour_tmp.GetRange(isTwo ? j_next : j, contour_tmp.Count - (isTwo ? j_next : j)));
                            contour_tmp.RemoveRange(j_next, contour_tmp.Count - j_next);
                        }
                        else
                        {
                            contour_DEL.Add(new List<Vector2>());
                            contour_DEL.Last().AddRange(contour_tmp.GetRange(0, j_next));
                            contour_tmp.RemoveRange(0, isTwo ? j_next : j);
                            j = 0;
                        }

                        contour_DEL.Last()[0] = new Vector2(contour_DEL.Last().First().x, transform.position.y);
                        contour_DEL.Last()[contour_DEL.Last().Count - 1] = new Vector2(contour_DEL.Last().Last().x, transform.position.y);
                    }
                }

                if (contour_tmp.Count < 3)
                {
                    contour_tmp.Clear();
                }

                contour.Clear();
                foreach (Vector2 v2 in contour_tmp)
                {
                    contour.Add(v2 - (Vector2)transform.position);
                }

                for (int i = 0; i < contour_DEL.Count; i++)
                {
                    for (int j = 0; j < contour_DEL[i].Count; j++)
                    {
                        contour_DEL[i][j] -= (Vector2)transform.position;
                    }
                }
            }
        }
        isCutting = false;
        return isEdit;
    }

    // Create a 3d model
    void Create3D()
    {
        // DeleteCurrentMesh(); 
        if (mesh == null)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            GameObject child_obj = new GameObject();
            child_obj.transform.SetParent(transform, false);

            mesh = child_obj.AddComponent<MeshFilter>().mesh;

            MeshRenderer meshRenderer = child_obj.AddComponent<MeshRenderer>();
            meshRenderer.materials = new Material[2] { MaterialIn, MaterialOut };

            Rigidbody rb = child_obj.AddComponent<Rigidbody>();
            rb.mass = 100;

            HingeJoint hj = child_obj.AddComponent<HingeJoint>();
            hj.useMotor = true;
            hj.motor = new JointMotor() { force = 100, targetVelocity = 500 };
        }

        getMesh3D(contour, mesh);


        foreach (List<Vector2> lv2 in contour_DEL)
        {
            GameObject go = new GameObject();
            go.transform.position = transform.position;
            Mesh mm = go.AddComponent<MeshFilter>().mesh;

            go.AddComponent<MeshRenderer>().materials = new Material[2] { MaterialIn, MaterialOut };

            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 100;
            float r = Random.Range(-0.5f, -0f);
            rb.AddForce(new Vector3(0, r, 1 + r) * Random.Range(5000, 8000), ForceMode.Impulse);

            getMesh3D(lv2, mm);

            Destroy(go, 3);
        }
    }
    /*   public void DeleteCurrentMesh()
       {
           if (mesh != null)
           {
               Destroy(mesh);
               mesh = null;
           }

           // Destroy all child objects as they hold the mesh
           for (int i = transform.childCount - 1; i >= 0; i--)
           {
               Destroy(transform.GetChild(i).gameObject);
           }
       }*/
    // Returns the generated mesh along the contour
    void getMesh3D(List<Vector2> lv2, Mesh mm)
    {
        vertices.Clear();
        triangles.Clear();
        trianglesTOP.Clear();

        int n = 18;
        float Y_TOP = -Size.y + Size.y / 100.0f;
        bool isTOP;

        if (lv2.Count > 0)
        {
            vertices.Add(lv2.First());
            vertices.Add(lv2.Last());
            for (int i = 1; i < lv2.Count - 1; i++)
            {
                if (i == 1)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        if (j < n)
                            vertices.Add(Quaternion.Euler(360.0f / n * j, 0, 0) * lv2[i]);
                        if (j == n)
                            MakeMesh(0, vertices.Count - 1, vertices.Count - n, false);
                        else if (j > 0)
                            MakeMesh(0, vertices.Count - 2, vertices.Count - 1, false);
                    }
                }
                if (i > 1)
                {
                    isTOP = lv2[i - 1].y <= Y_TOP && lv2[i].y <= Y_TOP;
                    float ang = Vector3.Angle(lv2[i - 2] - lv2[i - 1], lv2[i] - lv2[i - 1]);
                    if (ang <= 135)
                        vertices.AddRange(vertices.GetRange(vertices.Count - n, n));
                    for (int j = 0; j <= n; j++)
                    {
                        if (j < n)
                            vertices.Add(Quaternion.Euler(360.0f / n * j, 0, 0) * lv2[i]);
                        if (j == n)
                            MakeMesh(vertices.Count - n, vertices.Count - n * 2, vertices.Count - n - 1, vertices.Count - 1, isTOP);
                        else if (j > 0)
                            MakeMesh(vertices.Count - 1, vertices.Count - n - 1, vertices.Count - n - 2, vertices.Count - 2, isTOP);
                    }
                }
                if (i == lv2.Count - 2)
                {
                    bool isAdd = false;
                    float ang = Vector3.Angle(lv2[i - 1] - lv2[i], lv2[i + 1] - lv2[i]);
                    if (ang <= 135)
                        isAdd = true;
                    for (int j = 0; j <= n; j++)
                    {
                        if (isAdd && j < n)
                            vertices.Add(Quaternion.Euler(360.0f / n * j, 0, 0) * lv2[i]);
                        if (j == n)
                            MakeMesh(1, vertices.Count - n, vertices.Count - 1, false);
                        else if (j > 0)
                            MakeMesh(1, vertices.Count + (isAdd ? -1 : (-n + j)), vertices.Count + (isAdd ? -2 : (-n + j - 1)), false);
                    }
                }
            }
        }

        mm.Clear();
        mm.SetVertices(vertices);
        mm.subMeshCount = 2;
        mm.SetTriangles(triangles, 0);
        mm.SetTriangles(trianglesTOP, 1);
        mm.RecalculateNormals();
    }

    // Method to generate triangle mesh from the 3 points
    void MakeMesh(int id0, int id1, int id2, bool isTOP)
    {
        if (isTOP)
        {
            trianglesTOP.Add(id0);
            trianglesTOP.Add(id2);
            trianglesTOP.Add(id1);
        }
        else
        {
            triangles.Add(id0);
            triangles.Add(id2);
            triangles.Add(id1);
        }
    }

    // Method to generate 2 triangles mesh from the 4 points 0 1 2 3
    void MakeMesh(int id0, int id1, int id2, int id3, bool isTOP)
    {
        MakeMesh(id0, id1, id2, isTOP);
        MakeMesh(id0, id2, id3, isTOP);
    }
}