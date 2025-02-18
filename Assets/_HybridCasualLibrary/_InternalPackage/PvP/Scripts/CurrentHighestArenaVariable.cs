using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using LatteGames.PvP;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentHighestArenaVariable", menuName = "LatteGames/PvP/CurrentHighestArenaVariable")]
public class CurrentHighestArenaVariable : PvPArenaVariable
{
    [SerializeField]
    protected PvPTournamentSO m_TournamentSO;

    public override PvPArenaSO value
    {
        get
        {
            if (m_TournamentSO == null)
                return m_InitialValue;
            if (m_RuntimeValue == null)
                m_RuntimeValue = m_TournamentSO.arenas.FindLast(item => item.IsUnlocked());
            return m_RuntimeValue;
        }
        set => base.value = value;
    }

    protected void OnEnable()
    {
        foreach (var arenaSO in m_TournamentSO.arenas)
        {
            if (arenaSO.TryGetModule(out UnlockableItemModule unlockableModule))
                unlockableModule.onItemUnlocked += OnArenaUnlocked;
        }
    }

    protected void OnDisable()
    {
        foreach (var arenaSO in m_TournamentSO.arenas)
        {
            if (arenaSO.TryGetModule(out UnlockableItemModule unlockableModule))
                unlockableModule.onItemUnlocked -= OnArenaUnlocked;
        }
    }

    protected void OnArenaUnlocked(UnlockableItemModule unlockableModule)
    {
        value = m_TournamentSO.arenas.FindLast(item => item.IsUnlocked());
    }

    public override void OnAfterDeserialize()
    {
        // Do nothing
    }
}