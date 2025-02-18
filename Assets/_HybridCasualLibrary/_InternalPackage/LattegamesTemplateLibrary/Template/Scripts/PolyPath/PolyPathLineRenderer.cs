using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(PolyPath))]
    [RequireComponent(typeof(LineRenderer))]
    public class PolyPathLineRenderer : MonoBehaviour
    {
        public bool autoUpdate;

        private PolyPath _polyPath;
        private PolyPath polyPath {
            get{
                if(_polyPath == null)
                    _polyPath = GetComponent<PolyPath>();
                return _polyPath;
            }
        }

        private LineRenderer _lineRenderer;
        private LineRenderer lineRenderer{
            get{
                if(_lineRenderer == null)
                    _lineRenderer = GetComponent<LineRenderer>();
                return _lineRenderer;
            }
        }

        private void Update() {
            if(Application.isPlaying && !autoUpdate)
                return;
            Rebuild();
        }

        public void Rebuild()
        {
            lineRenderer.positionCount = polyPath.points.Count;
            lineRenderer.useWorldSpace = false;
            for (int i = 0; i < polyPath.points.Count; i++)
            {
                lineRenderer.SetPosition(i, polyPath.points[i].position);
            }
        }
    }
}