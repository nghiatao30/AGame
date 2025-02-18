using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_CINEMACHINE
using Cinemachine;
#endif
using HyrphusQ.Events;

[EventCode]
public enum CameraEventCode
{
    /// <summary>
    /// This event raised when a script need to change camera to destinated Camera Category
    /// <para> <typeparamref name="Enum"/>: CameraCategory </para>
    /// </summary>
    OnCameraChangeStart,
    /// <summary>
    /// This event raised when camera manager complete change to destinated camera
    /// <para> <typeparamref name="Enum"/>: CameraCategory </para>
    /// </summary>
    OnCameraChangeComplete,
}

namespace LatteGames.CinemachineUtils
{
    [OptionalDependency("Cinemachine.CinemachineVirtualCamera", "UNITY_CINEMACHINE")]
    public class CameraController<EnumType> : MonoBehaviour where EnumType : Enum
    {
#if UNITY_CINEMACHINE
        [Serializable]
        public class CameraType
        {
            public EnumType Category;
            public CinemachineVirtualCamera Camera;
        }


        [SerializeField] protected List<CameraType> cameras;
        [SerializeField] EnumType startCategory;

        protected Camera mainCam;

        protected EnumType currentCategory;
        protected virtual EnumType CurrentCategory
        {
            set
            {
                if (currentCategory.Equals(value) && GetCamera(currentCategory).enabled) return;
                if (!HasCamera(value)) return;
                SetCamera(currentCategory, false);
                currentCategory = value;
                SetCamera(currentCategory, true);
                GameEventHandler.Invoke(CameraEventCode.OnCameraChangeComplete, currentCategory);
            }
        }

        protected void SetCamera(EnumType category, bool isEnable)
        {
            if (!HasCamera(category)) return;
            GetCamera(category).enabled = isEnable;
        }

        protected virtual void Awake()
        {
            foreach (var cam in cameras) cam.Camera.enabled = false;
            mainCam = Camera.main;
            CurrentCategory = startCategory;

            GameEventHandler.AddActionEvent(CameraEventCode.OnCameraChangeStart, HandleCameraChangeStarted);

        }

        private void OnDestroy()
        {
            GameEventHandler.RemoveActionEvent(CameraEventCode.OnCameraChangeStart, HandleCameraChangeStarted);
        }

        protected CinemachineVirtualCamera GetCamera(EnumType category)
        {
            return cameras.Find(cam => cam.Category.Equals(category)).Camera;
        }

        protected bool HasCamera(EnumType category)
        {
            if (cameras == null || cameras.Count <= 0) return false;
            var cameraType = cameras.Find(cam => cam.Category.Equals(category));
            if (cameraType == null || cameraType.Camera == null) return false;
            return true;
        }

        void HandleCameraChangeStarted(object[] parameters)
        {
            if (parameters[0] is not Enum) return;
            var destinatedCategory = (EnumType)parameters[0];
            CurrentCategory = destinatedCategory;
        }
#endif
    }
}