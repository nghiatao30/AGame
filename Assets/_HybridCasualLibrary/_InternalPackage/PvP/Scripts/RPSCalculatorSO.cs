using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.PvP
{
    public abstract class RPSCalculatorSO : ScriptableObject
    {
        public abstract class RPSData
        {
            #region Constructor
            public RPSData(float scoreOfPlayer, float scoreOfOpponent)
            {
                this.scoreOfPlayer = scoreOfPlayer;
                this.scoreOfOpponent = scoreOfOpponent;
            }
            #endregion

            /// <summary>
            /// RPS state label
            /// </summary>
            protected const string k_EvenLabel = "Even";
            protected const string k_AdvantageLabel = "Advantage";

            public virtual float scoreOfOpponent { get; private set; }
            public virtual float scoreOfPlayer { get; private set; }
            public virtual float rpsInverseLerp => Mathf.InverseLerp(rpsRange.minValue, rpsRange.maxValue, rpsValue);
            public abstract float rpsValue { get; }
            public abstract string stateLabel { get; }
            public abstract RangeValue<float> rpsRange { get; }
        }

        [SerializeField]
        protected FloatVariableReference m_ScoreOfPlayer;
        [SerializeField]
        protected FloatVariableReference m_ScoreOfOpponent;

        protected virtual float currentScoreOfPlayer => m_ScoreOfPlayer.value;
        protected virtual float currentScoreOfOpponent => m_ScoreOfOpponent.value;

        /// <summary>
        /// Calculate relative power score value between player and current opponent
        /// </summary>
        /// <returns>Return relative power score data</returns>
        public virtual RPSData CalcCurrentRPSValue()
        {
            return CalcRPSValue(currentScoreOfPlayer, currentScoreOfOpponent);
        }
        /// <summary>
        /// Calculate relative power score value in range [rpsRange.min, rpsRange.max]
        /// </summary>
        /// <param name="scoreOfPlayer">Overall score of player</param>
        /// <param name="scoreOfOpponent">Overall score of opponent</param>
        /// <returns>Return relative power score data</returns>
        public abstract RPSData CalcRPSValue(float scoreOfPlayer, float scoreOfOpponent);
    }
}