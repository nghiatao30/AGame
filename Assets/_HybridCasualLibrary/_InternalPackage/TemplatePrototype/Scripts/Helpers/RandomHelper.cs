using System;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.ParticleSystem;

namespace HyrphusQ.Helpers
{
    public static class RandomHelper
    {
        /// <summary>
        /// Prevents infinite loop and crash UnityEditor in while loop
        /// </summary>
        private readonly static int MaxAttempt = 100;

        public static float Random01(Predicate<float> predicate = null) =>
            RandomRange(Const.FloatValue.ZeroF, Const.FloatValue.OneF, predicate);
        public static float RandomOpposite(float range, Predicate<float> predicate = null) =>
            RandomRange(-range, range, predicate);
        public static int RandomOpposite(int range, Predicate<int> predicate = null) =>
            RandomRange(-range, range, predicate);
        public static float RandomRange(RangeFloatVariable rangeFloatVariable, Predicate<float> predicate = null) =>
            RandomRange(rangeFloatVariable.minValue, rangeFloatVariable.maxValue, predicate);
        public static int RandomRange(RangeIntVariable rangeIntVariable, Predicate<int> predicate = null) =>
            RandomRange(rangeIntVariable.minValue, rangeIntVariable.maxValue, predicate);
        public static float RandomRange(float min, float max, Predicate<float> predicate = null)
        {
            int attempt = Const.IntValue.Zero;
            while (true)
            {
                var randomValue = UnityEngine.Random.Range(min, max);
                if ((predicate?.Invoke(randomValue) ?? true) || attempt > MaxAttempt)
                    return randomValue;
                attempt++;
            }
        }
        public static int RandomRange(int min, int max, Predicate<int> predicate = null)
        {
            int attempt = Const.IntValue.Zero;
            while (true)
            {
                var randomValue = UnityEngine.Random.Range(min, max);
                if ((predicate?.Invoke(randomValue) ?? true) || attempt > MaxAttempt)
                    return randomValue;
                attempt++;
            }
        }
        public static Vector3 RandomRange(Vector3 min, Vector3 max, Predicate<Vector3> predicate = null)
        {
            int attempt = Const.IntValue.Zero;
            while (true)
            {
                var randomValue = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
                if ((predicate?.Invoke(randomValue) ?? true) || attempt > MaxAttempt)
                    return randomValue;
                attempt++;
            }
        }
        public static float RandomRange(MinMaxCurve minMaxCurve) =>
            RandomRange(minMaxCurve.constantMin, minMaxCurve.constantMax);
        public static Vector3 RandomDirection(Axis axisFlag = Axis.X | Axis.Y | Axis.Z)
        {
            var randomDirection = Vector3.zero;
            if ((axisFlag & Axis.X) != 0)
                randomDirection += Vector3.right * RandomOpposite(Const.FloatValue.OneF);
            if ((axisFlag & Axis.Y) != 0)
                randomDirection += Vector3.up * RandomOpposite(Const.FloatValue.OneF);
            if ((axisFlag & Axis.Z) != 0)
                randomDirection += Vector3.forward * RandomOpposite(Const.FloatValue.OneF);
            return randomDirection;
        }
        public static Vector3 RandomDirection(Predicate<Vector3> predicate, Axis axisFlag = Axis.X | Axis.Y | Axis.Z)
        {
            int attempt = Const.IntValue.Zero;
            while (true)
            {
                var randomDirection = RandomDirection(axisFlag);
                if ((predicate?.Invoke(randomDirection) ?? true) || attempt > MaxAttempt)
                    return randomDirection;
                attempt++;
            }
        }
        public static Vector3 RandomPositionByBounds(BoundingSphere boundingSphere, Predicate<Vector3> predicate = null)
        {
            int attempt = Const.IntValue.One;
            while (true)
            {
                var randomNormalizedDirection = RandomDirection().normalized;
                var randomPoint = boundingSphere.position + randomNormalizedDirection * boundingSphere.radius;
                if ((predicate?.Invoke(randomPoint) ?? true) || attempt > MaxAttempt)
                    return randomPoint;
                attempt++;
            }
        }
        public static Vector3 RandonPositionByBounds(Bounds boundingBox, Predicate<Vector3> predicate = null)
        {
            int attempt = Const.IntValue.One;
            while (true)
            {
                var randomNormalizedDirection = new Vector3(
                    RandomOpposite(boundingBox.extents.x), 
                    RandomOpposite(boundingBox.extents.y), 
                    RandomOpposite(boundingBox.extents.z));

                var randomPoint = boundingBox.center + randomNormalizedDirection;
                if ((predicate?.Invoke(randomPoint) ?? true) || attempt > MaxAttempt)
                    return randomPoint;
                attempt++;
            }
        }
    }
}
