using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public abstract class ObjectPlacer : MonoBehaviour
    {
        [SerializeField]
        protected GameObject objectTemplate = null;
        public abstract void UpdatePlacing();
        public abstract void Clear();
        public abstract int GetObjectCount();
    }
}
