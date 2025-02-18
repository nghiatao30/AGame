using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public class Bezier : MonoBehaviour
    {
        [Serializable]
        public class Point
        {
            public Vector3 position;

            public Vector3 inTangentHandle;
            public Vector3 inTangentTwistHandle;

            public Vector3 outTangentHandle;
            public Vector3 outTangentTwistHandle;

            public Vector3 InTangentPosition => inTangentHandle;
            public Vector3 OutTangentPosition => outTangentHandle;
            
            public Quaternion OutRotation => Quaternion.LookRotation(outTangentHandle - position, outTangentTwistHandle - outTangentHandle);
            public Quaternion InRotation => Quaternion.LookRotation(position - inTangentHandle, inTangentTwistHandle - inTangentHandle);
            public bool broken;

            public Point()
            {
                inTangentHandle = position + Quaternion.identity*Vector3.back;
                inTangentTwistHandle = inTangentHandle + Quaternion.identity*Vector3.up;

                outTangentHandle = position + Quaternion.identity*Vector3.forward;
                outTangentTwistHandle = outTangentHandle + Quaternion.identity*Vector3.up;
            }
        }

        public List<Point> controlPoints = new List<Point>(){
            new Point(){position = new Vector3(0,0,0)},
            new Point(){position = new Vector3(0,0,1)},
        };
        public bool loop;

        private void OnDrawGizmos() {
            for (int i = 0; i < controlPoints.Count; i++)
            {
                Gizmos.color = new Color(0.1f, 0.8f, 0.2f);
                var position = transform.TransformPoint(controlPoints[i].position);
                var rotation = transform.rotation*controlPoints[i].OutRotation;

                if(i < controlPoints.Count - 1 || loop)
                {
                    var p0 = controlPoints[i];
                    var p1 = controlPoints[(i + 1)%controlPoints.Count];
                    var startP = p0.position;
                    var endP = p1.position;
                    var inTan = controlPoints[i].OutTangentPosition;
                    var outTan = controlPoints[i + 1].InTangentPosition;
                    
                    int segment = 20;
                    for (int j = 0; j < segment; j++)
                    {
                        Gizmos.DrawLine(
                            BezierPathCalculation(
                                transform.TransformPoint(startP), 
                                transform.TransformPoint(inTan), 
                                transform.TransformPoint(outTan),
                                transform.TransformPoint(endP),
                                j*1.0f/segment
                            ),
                            BezierPathCalculation(
                                transform.TransformPoint(startP), 
                                transform.TransformPoint(inTan), 
                                transform.TransformPoint(outTan),
                                transform.TransformPoint(endP),
                                (j+1)*1.0f/segment
                            ));
                    }
                }

                Gizmos.DrawLine(position, position + rotation*Vector3.up);
            }
        }

        public bool LocalEvaluate(int index, float t, out Matrix4x4 result)
        {
            if(index < controlPoints.Count - 1 || loop)
            {
                var p0 = controlPoints[index];
                var p1 = controlPoints[(index + 1)%controlPoints.Count];
                var startP = p0.position;
                var endP = p1.position;
                var inTan = controlPoints[index].OutTangentPosition;
                var outTan = controlPoints[index + 1].InTangentPosition;
                
                var pos = BezierPathCalculation(
                            startP, 
                            inTan, 
                            outTan,
                            endP,
                            t
                        );
                var rot = Quaternion.Lerp(p0.OutRotation, p1.InRotation, t);
                result = Matrix4x4.TRS(pos, rot, Vector3.one);
                return true;
            }
            result = Matrix4x4.identity;
            return false;
        }

        public bool Evaluate(int index, float t, out Matrix4x4 result)
        {
            Matrix4x4 local;
            bool success = LocalEvaluate(index, t, out local);
            result = transform.localToWorldMatrix*local;
            return success;
        }

        public Vector3 BezierPathCalculation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{	
			float tt = t * t;
			float ttt = t * tt;
			float u = 1.0f - t;
			float uu = u * u;
			float uuu = u * uu;
			
			Vector3 B = new Vector3();
			B = uuu * p0;
			B += 3.0f * uu * t * p1;
			B += 3.0f * u * tt * p2;
			B += ttt * p3;
			
			return B;
		}

        public float GetApproximateLength(int res)
        {
            float length = 0.0f;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                length += GetApproximateLength(
                    controlPoints[i].position,
                    controlPoints[i].outTangentHandle, 
                    controlPoints[i + 1].inTangentHandle, 
                    controlPoints[i + 1].position,
                    res);
            }
            return length;
        }

        public Matrix4x4[] GetPoints(int pointCount, int res = 20)
        {
            float length = GetApproximateLength(res);
            Matrix4x4[] points = new Matrix4x4[pointCount];
            float step = length/pointCount;
            for (int i = 0; i < pointCount; i++)
                points[i] = GetPoint(length, i/(pointCount - 1), res);
            return points;
        }

        private Matrix4x4 GetPoint(float length, float t, int res = 20)
        {
            float currentL = 0.0f;
            int foundIndex = -1;
            float localT = 0.0f;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                var l = GetApproximateLength(
                    controlPoints[i].position,
                    controlPoints[i].outTangentHandle, 
                    controlPoints[i + 1].inTangentHandle, 
                    controlPoints[i + 1].position,
                    res);
                if (currentL < length && currentL + l > length)
                {
                    foundIndex = i;
                    localT = (length - currentL)/length;
                    break;
                }
                currentL += l;
            }
            if(foundIndex == -1)
                return Matrix4x4.identity;
            if(LocalEvaluate(foundIndex, localT, out Matrix4x4 result))
                return result;
            return Matrix4x4.identity;
        }

        private float GetApproximateLength(
            Vector3 p0, 
            Vector3 p1, 
            Vector3 p2, 
            Vector3 p3,
            int res)
        {
            float l = 0.0f;
            for (int i = 0; i < res; i++)
            {
                var v0 = BezierPathCalculation(p0,p1, p2, p3, i*1.0f/res);
                var v1 = BezierPathCalculation(p0,p1, p2, p3, (i + 1)*1.0f/res);
                l += (v1 - v0).magnitude;
            }
            return l;
        }
    }
}