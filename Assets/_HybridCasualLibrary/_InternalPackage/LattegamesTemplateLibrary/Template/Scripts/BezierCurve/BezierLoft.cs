using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    [RequireComponent(typeof(Bezier))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class BezierLoft : MonoBehaviour
    {
        [SerializeField] private Vector2[] profile = {Vector2.zero, Vector2.one};
        [SerializeField] private PolyPath profilePath;
        [SerializeField] private float thickness = 0.1f;
        [SerializeField] private bool duplicationClose = false;
        [SerializeField] private bool close = false;
        
        [SerializeField] private int subCount = 20;
        private Mesh mesh = null;
        private Bezier _bezier;
        private Bezier bezier {
            get{
                if(_bezier == null)
                    _bezier = GetComponent<Bezier>();
                return _bezier;
            }
        }
        private MeshFilter _meshFilter;
        private MeshFilter meshFilter{
            get{
                if(_meshFilter == null)
                    _meshFilter = GetComponent<MeshFilter>();
                return _meshFilter;
            }
        }
        private MeshRenderer _meshRenderer;
        private MeshRenderer meshRenderer{
            get{
                if(_meshRenderer == null)
                    _meshRenderer = GetComponent<MeshRenderer>();
                return _meshRenderer;
            }
        }
        [SerializeField] private bool autoUpdate = false;

        public void Build()
        {
            meshFilter.sharedMesh = GenerateMesh();
        }

        public Mesh GenerateMesh()
        {
            if(profilePath != null)
                profile = profilePath.points.ConvertAll(p => new Vector2(p.position.x, p.position.y)).ToArray();
            var genProfile = profile;
            if(duplicationClose)
            {
                genProfile = new Vector2[profile.Length*2];
                for (int i = 0; i < profile.Length; i++)
                {
                    var normal = Vector2.zero;
                    if(i + 1 < profile.Length)
                        normal = (Vector2) (Quaternion.Euler(0,0,90)*(profile[i + 1] - profile[i]));
                    if(i - 1 >= 0)
                        normal += (Vector2) (Quaternion.Euler(0,0,90)*(profile[i] - profile[i - 1]));
                    normal.Normalize();
                    genProfile[i] = profile[i];
                    genProfile[profile.Length*2 - i - 1] = profile[i] + normal*thickness;
                }
            }
            if(!ValidateProfile())
                return mesh;
            var vertices = new List<Vector3>();
            var uvs = new List<Vector3>();
            var triangles = new List<int>();
            var totalCount = (bezier.controlPoints.Count - 1)*subCount;
            for (int i = 0; i < bezier.controlPoints.Count; i++)
            {
                var extra = (i == bezier.controlPoints.Count - 2)?1:0;
                for (int j = 0; j < subCount + extra; j++)
                {
                    if(bezier.LocalEvaluate(i, j*1.0f/subCount, out Matrix4x4 mat))
                    {
                        int startIndex = vertices.Count;
                        for (int k = 0; k <= genProfile.Length; k++)
                        {
                            vertices.Add(mat.MultiplyPoint3x4(genProfile[k%genProfile.Length]));
                            uvs.Add(new Vector2(k*1.0f/genProfile.Length, (i*subCount + j)*1.0f/totalCount));
                        }
                        if (startIndex > 0)
                        {
                            //connect triangle with previous one
                            int length = genProfile.Length - 1;
                            if(close)
                                length += 1;
                            for (int k = 0; k < length; k++)
                            {
                                triangles.Add(startIndex + k);
                                triangles.Add(startIndex - genProfile.Length - 1 + k);
                                triangles.Add(startIndex - genProfile.Length - 1 + k + 1);
                                triangles.Add(startIndex + k);
                                triangles.Add(startIndex - genProfile.Length - 1 + k + 1);
                                triangles.Add(startIndex + k + 1);
                            }
                        }
                    }
                }
            }
            if(mesh == null)
                mesh = new Mesh();

            mesh.MarkDynamic();
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }

        private void Update() {
            if(autoUpdate)
                Build();
        }

        private bool ValidateProfile()
        {
            return profile.Length > 1;
        }
    }
}