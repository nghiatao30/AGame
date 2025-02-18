using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LatteGames
{
    public class EZAnimVector3 : EZAnimUnityEvent<Vector3>
    {
        protected override void SetAnimationCallBack()
        {
            AnimationCallBack = t =>
            {
                UnityAnimationCallBack?.Invoke(Vector3.LerpUnclamped(from, to, t));
            };
            base.SetAnimationCallBack();
        }
    }
}