using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames
{
    public class MainTabUnit : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private Vector3 normalScale = Vector2.one;
        [SerializeField] private Vector3 highlightScale = Vector2.one * 1.2f;

        public Transform imgContainer;
        public Button button;
        public Text title;
        public Image activeIcon;
        public Image inActiveIcon;
        public Color color;
        private Coroutine animationCR;


        public void ChangeState(bool isActive, float runTime)
        {
            if (!transform.gameObject.activeSelf)
                return;
            if (animationCR != null)
                StopCoroutine(animationCR);
            animationCR = StartCoroutine(CR_ChangeState(isActive, runTime));
        }

        public void ChangeStateImmediately(bool isActive)
        {
            activeIcon.gameObject.SetActive(isActive);
            inActiveIcon.gameObject.SetActive(!isActive);
            Vector3 endScale = isActive ? highlightScale : normalScale;
            imgContainer.localScale = endScale;
        }

        IEnumerator CR_ChangeState(bool isActive, float runTime)
        {
            float value = 0;
            float speed = 1 / runTime;
            Vector3 startScale = imgContainer.localScale;
            Vector3 endScale = isActive ? highlightScale : normalScale;
            activeIcon.gameObject.SetActive(isActive);
            inActiveIcon.gameObject.SetActive(!isActive);
            while (value < 1)
            {
                value += Time.deltaTime * speed;
                imgContainer.localScale = Vector3.Lerp(startScale, endScale, value);
                yield return null;
            }
        }
    }
}