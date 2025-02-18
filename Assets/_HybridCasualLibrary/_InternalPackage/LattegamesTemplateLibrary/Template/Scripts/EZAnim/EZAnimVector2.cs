using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LatteGames
{
    public class EZAnimVector2 : EZAnimUnityEvent<Vector2>
    {
        protected override void SetAnimationCallBack()
        {
            AnimationCallBack = t =>
            {
                UnityAnimationCallBack?.Invoke(Vector2.LerpUnclamped(from, to, t));
            };
            base.SetAnimationCallBack();
        }
    }
}