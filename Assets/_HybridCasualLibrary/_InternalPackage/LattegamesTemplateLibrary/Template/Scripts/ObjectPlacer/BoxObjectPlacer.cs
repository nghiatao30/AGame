using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames.Utils;

namespace LatteGames
{
    public class BoxObjectPlacer : ObjectPlacer
    {
        public Vector3 center = Vector3.zero;
        public Vector3 size = Vector3.one;
        public Vector3 spacing = Vector3.one;

        public bool fillInterior = false;
        public bool gizmo = false;

        public override void UpdatePlacing()
        {
            transform.DestroyChildren();
            IterateSpawnPoint((index, position)=>{
                var newObject = Instantiate(objectTemplate, transform);
                newObject.transform.localPosition = position;
            });
        }

        public override void Clear()
        {
            transform.DestroyChildren();
        }

        public override int GetObjectCount()
        {
            Vector3Int objectCount = new Vector3Int(
                Mathf.RoundToInt(size.x/spacing.x),
                Mathf.RoundToInt(size.y/spacing.y),
                Mathf.RoundToInt(size.z/spacing.z)   
            );
            var interiorCount = Mathf.RoundToInt(Mathf.Clamp01(objectCount.x - 2)*Mathf.Clamp01(objectCount.y - 2)*Mathf.Clamp01(objectCount.z - 2));
            return objectCount.x*objectCount.y*objectCount.z - (!fillInterior?interiorCount:0);
        }

        public void IterateSpawnPoint(Action<Vector3Int, Vector3> callback)
        {
            Vector3Int objectCount = new Vector3Int(
                Mathf.RoundToInt(size.x/spacing.x),
                Mathf.RoundToInt(size.y/spacing.y),
                Mathf.RoundToInt(size.z/spacing.z)   
            );
            Vector3 correctSize = new Vector3(
                (objectCount.x - 1)*spacing.x,
                (objectCount.y - 1)*spacing.y,
                (objectCount.z - 1)*spacing.z
            );
            for (int i = 0; i < objectCount.x; i++)
            {
                for (int j = 0; j < objectCount.y; j++)
                {
                    for (int k = 0; k < objectCount.z; k++)
                    {
                        bool interior =
                            i > 0 && i < objectCount.x - 1
                            && j > 0 && j < objectCount.y - 1
                            && k > 0 && k < objectCount.z - 1;

                        if(interior && !fillInterior)
                            continue;
                        callback(new Vector3Int(i,j,k), new Vector3(i*spacing.x,j*spacing.y,k*spacing.z) + center - correctSize/2);
                    }
                }
            }
        }

        private void OnDrawGizmos() {
            if(!gizmo)
                return;
            IterateSpawnPoint((indexPosition, position)=>{
                Gizmos.DrawSphere(transform.TransformPoint(position), 0.2f);
            });
        }
    }
}