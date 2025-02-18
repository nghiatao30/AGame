using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LatteGames;
using LatteGames.Template;
using Sirenix.OdinInspector;

namespace LatteGames.UnpackAnimation
{
    public class BagDropOnGroundFX : MonoBehaviour
    {
        [SerializeField]
        private float m_OriginalBagShadowUniformScale;
        [SerializeField]
        private float m_DestinationBagShadowUniformScale;
        [SerializeField, Range(0, 1f)]
        private float m_OriginalAlpha = 0.2f;
        [SerializeField, Range(0, 1f)]
        private float m_DestinationAlpha = 0.7f;
        [SerializeField]
        private Transform m_BagShadowTransform;
        [SerializeField]
        private SpiralShapeParticleEmitter m_OpenBagVFX;

        public void PlayAnim()
        {
            m_BagShadowTransform.localScale = m_OriginalBagShadowUniformScale * Vector3.one;
            m_BagShadowTransform.DOScale(m_DestinationBagShadowUniformScale, AnimationDuration.SSHORT);
            if (m_BagShadowTransform.TryGetComponent(out Renderer bagShadowRenderer))
            {
                bagShadowRenderer.material.DOFade(m_OriginalAlpha, AnimationDuration.ZERO);
                bagShadowRenderer.material.DOFade(m_DestinationAlpha, AnimationDuration.SSHORT);
            }
            m_OpenBagVFX.PlayFX();
            SoundManager.Instance.PlaySFX(GeneralSFX.UIDropBox);
        }
    }
}