using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames
{
    public class EZAnimColor : EZAnim<Color>
    {
        [SerializeField, BoxGroup("Specific")]
        protected Graphic graphic;

        protected override void SetAnimationCallBack()
        {
            AnimationCallBack = t =>
            {
                graphic.color = Color.LerpUnclamped(from, to, t);
            };
            base.SetAnimationCallBack();
        }
    }
}