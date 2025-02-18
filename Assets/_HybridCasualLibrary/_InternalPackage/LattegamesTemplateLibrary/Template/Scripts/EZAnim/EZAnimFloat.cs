using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LatteGames
{
    public class EZAnimFloat : EZAnimUnityEvent<float>
    {
        protected override void SetAnimationCallBack()
        {
            AnimationCallBack = t =>
            {
                UnityAnimationCallBack?.Invoke(Mathf.LerpUnclamped(From, To, t));
            };
            base.SetAnimationCallBack();
        }
    }
}