using UnityEngine;
using HyrphusQ.Events;
using System.Collections;

namespace LatteGames.PvP.TrophyRoad
{
    public class TrophyRoad : MonoBehaviour
    {
        static SceneName PvPSceneName;
        public static bool BackFromPvP { get; private set; } = false;
        static bool eventSubscribed;

        [SerializeField] TrophyRoadSO trophyRoadSO;
        [Header("Detecting player back from PvP")]
        [SerializeField] SceneName pvpSceneName;
        [SerializeField] PPrefBoolVariable PPrefFTUEOpen;

        bool existsUnlocked = false;

        private void Awake()
        {
            PvPSceneName = pvpSceneName;
            if (!eventSubscribed)
                GameEventHandler.AddActionEvent(SceneManagementEventCode.OnLoadSceneCompleted, HandleLoadSceneCompleted);
            eventSubscribed = true;
        }

        private static void HandleLoadSceneCompleted(params object[] objs)
        {
            BackFromPvP = false;
            if (objs[1] is not string originSceneName) return;
            if (originSceneName != PvPSceneName.ToString()) return;
            BackFromPvP = true;
        }

        private IEnumerator Start()
        {
            yield return Yielders.Get(0.1f);
            CheckAutoOpen();
        }

        private void CheckAutoOpen()
        {
            trophyRoadSO.TryUnlockingArenas();
            existsUnlocked = trophyRoadSO.TryUnlockMilestones(BackFromPvP);
            //if (BackFromPvP && (existsUnlocked || PPrefFTUEOpen.value == false))
            //{
            //    PPrefFTUEOpen.value = true;
            //    GameEventHandler.Invoke(TrophyRoadEventCode.OnTrophyRoadOpened);
            //}
            if (BackFromPvP && existsUnlocked)
            {
                GameEventHandler.Invoke(TrophyRoadEventCode.OnTrophyRoadOpened);
            }
        }
    }
}
