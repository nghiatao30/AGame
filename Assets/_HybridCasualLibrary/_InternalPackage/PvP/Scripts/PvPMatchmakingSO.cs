using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LatteGames.PvP
{
    public abstract class PvPMatchmakingSO<T> : ScriptableObject where T : PlayerInfo
    {
        [SerializeField, BoxGroup("Configs")]
        protected int m_SearchingDuration = 7;
        [SerializeField, BoxGroup("Configs")]
        protected int m_PreparingMatchDuration = 7;

        public int searchingDuration => m_SearchingDuration;
        public int preparingMatchDuration => m_PreparingMatchDuration;

        public abstract T FindOpponent(PvPArenaSO arenaSO, Predicate<T> predicate = null);

        public virtual IEnumerator FindOpponent_CR(PvPArenaSO arenaSO, Action<T> callback, Predicate<T> predicate = null)
        {
            var opponent = FindOpponent(arenaSO, predicate);
            yield return new WaitForSeconds(m_SearchingDuration);
            callback?.Invoke(opponent);
        }
    }
}