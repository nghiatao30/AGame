using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace LatteGames
{
    public class EZAnimTextCount : EZAnim<float>
    {
        [SerializeField, BoxGroup("Specific")]
        protected TMP_Text text;
        [SerializeField, BoxGroup("Specific")]
        protected string format = "{value}";

        protected override void SetAnimationCallBack()
        {
            AnimationCallBack = t =>
            {
                text.text = format.Replace("{value}", Mathf.Lerp(from, to, t).ToRoundedText());
            };
            base.SetAnimationCallBack();
        }
    }
}