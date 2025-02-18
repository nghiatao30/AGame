using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.PvP
{
    public abstract class PvPStageSpawner : MonoBehaviour
    {
        public abstract IAsyncTask SpawnStage(PvPArenaSO arenaSO);
    }
}