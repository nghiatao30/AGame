using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] protected int topCount = 20;
    //[SerializeField] float mergeDuration = .3f;
    [SerializeField] protected int rankCap = 200;
    
    [Header("References")]
    [SerializeField] protected LeaderboardManagerSO leaderboardManagerSO;
    [SerializeField] protected Transform rowParent;
    [SerializeField] protected RectTransform playerSection;
    [SerializeField] protected LeaderboardRow playerRow;
    [SerializeField] protected RectTransform scrollView;
    [SerializeField] protected RectMask2D mask;

    [Header("Prefabs")]
    [SerializeField] protected LeaderboardRow rowPrefab;
    [SerializeField] protected LeaderboardRow[] topRowPrefabs;
    [SerializeField] protected LeaderboardRow bottomRowPrefab;
    [SerializeField] protected LeaderboardRow playerRowPrefab;

    protected List<LeaderboardRow> rows;
    protected List<LeaderboardRow> disabledRows;
    protected float topRowLocalY;
    protected int playerSectionMinRank;
    protected LeaderboardRow mergeRow;
    protected float basePlayerSectionLocalY;
    protected Vector3 playerSectionPos;
    protected Vector2 baseScrollViewSize;
    protected Vector2 mergedScrollViewSize;
    protected bool merged;
    protected Vector2 baseScrollBarSize;
    protected Vector2 mergedScrollBarSize;
    protected bool masked;

    protected virtual void Start()
    {
        disabledRows = new();
        Canvas.ForceUpdateCanvases();

        // save the lowest y (local) position for player section for clamping
        basePlayerSectionLocalY = playerSection.position.y - transform.position.y;

        // save the highest y (local) postion for player section for clamping
        topRowLocalY = scrollView.position.y - transform.position.y;

        playerSectionPos = playerSection.position;

        // calculate scroll view's size when merged
        baseScrollViewSize = scrollView.sizeDelta;
        mergedScrollViewSize = baseScrollViewSize;
        mergedScrollViewSize.y += playerSection.sizeDelta.y;

        // re-anchor scroll bar + preserve size
        baseScrollBarSize.y = baseScrollViewSize.y;
        mergedScrollBarSize = baseScrollBarSize;
        mergedScrollBarSize.y += playerSection.sizeDelta.y;

        GenerateEntries();
        Refresh();
    }

    protected virtual void GenerateEntries()
    {
        var existedRows = rowParent.GetComponentsInChildren<LeaderboardRow>();
        rows = (from i in Enumerable.Range(0, topCount - existedRows.Length) select Instantiate(rowPrefab, rowParent)).ToList();
        rows.InsertRange(0, existedRows);
    }

    protected virtual void UpdatePlayerSection()
    {
        // get values
        var sectionRows = playerSection.GetComponentsInChildren<LeaderboardRow>(true).ToList();
        var playerIndexInSection = sectionRows.IndexOf(playerRow);
        var playerRank = leaderboardManagerSO.PlayerRank;

        // update each rows in player section
        for (int i = 0; i < sectionRows.Count; i++)
        {
            // get values
            LeaderboardRow row = sectionRows[i];
            var offsetIndex = i - playerIndexInSection;
            var rank = Mathf.Clamp(playerRank + offsetIndex, 0, leaderboardManagerSO.Entries.Count - 1); // from 0
            var isPlayerRow = i == playerIndexInSection;

            // update row type: changing visual prefab
            LeaderboardRow newRow;

            if (rank < topRowPrefabs.Length) newRow = ChangeRowType(topRowPrefabs[rank], i, row); // in top 3
            else if (isPlayerRow) newRow = ChangeRowType(playerRowPrefab, i, row);                // is player's row
            else newRow = ChangeRowType(bottomRowPrefab, i, row);                                       // bot's row
            sectionRows[i] = row = newRow;
            if (isPlayerRow) playerRow = row;

            // cap bot's rank
            var maxBotRank = rankCap - 1;   // get the index
            if (!isPlayerRow)
                rank = Mathf.Min(rank, (maxBotRank + 1) + offsetIndex);   // (maxBotRank + 1) is the player's row

            // update row info
            var entry = leaderboardManagerSO.Entries[rank];
            row.RankCap = rankCap;
            row.Rank = rank + 1;
            row.Name = entry.name;
            row.Avatar = entry.avatar;
            row.AvatarFrame = entry.avatarFrame;
            row.NationalFlag = entry.nationalFlag;
            row.Medal = (int) entry.GetTotalNumOfPoints();
        }


        playerSectionMinRank = playerRank - playerIndexInSection;
        HandleSpecialCase(sectionRows);

        // find the row for the player section to follow when merged
        FindMergeRow();
    }

    protected virtual void HandleSpecialCase(List<LeaderboardRow> sectionRows)
    {
        // handle when the player's rank < index of the player's row in player section
        // this will happen when there's a row BEFORE the player's row in player section
        // so the minimum rank BEFORE the player's row will be < 0
        if (playerSectionMinRank < 0)
        {
            // hide all other rows in player section
            disabledRows.Clear();
            foreach (var row in sectionRows)
            {
                if (row == playerRow) continue;
                row.gameObject.SetActive(false);
                disabledRows.Add(row);
            }
            masked = false;
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            // show them back again
            foreach (var row in disabledRows) row.gameObject.SetActive(true);
            disabledRows.Clear();
        }
    }

    protected virtual void FindMergeRow()
    {
        try
        {
            // it will be the row with the same rank as the first row in player section
            playerSectionMinRank = Mathf.Max(playerSectionMinRank, 0);
            mergeRow = rows[playerSectionMinRank];
        }
        catch (ArgumentOutOfRangeException)
        {
            // in case the rank to follow is outside top 20: don't merge
            mergeRow = null;
        }
    }

    protected virtual LeaderboardRow ChangeRowType(LeaderboardRow rowPrefab, int i, LeaderboardRow row)
    {
        DestroyImmediate(row.gameObject);
        var newRow = Instantiate(rowPrefab, playerSection);
        newRow.transform.SetSiblingIndex(i);
        return newRow;
    }

    protected virtual void LateUpdate()
    {
        // when player's rank is in top 20, the player section will merge with the scroll view
        // the player section will then follow the merge row
        if (!mergeRow) return;

        // player section y: following the merge row
        // while preventing it from scrolling past the scroll view in both ends
        float y = transform.position.y;
        playerSectionPos.y = Mathf.Clamp(mergeRow.transform.position.y - y, basePlayerSectionLocalY, topRowLocalY) + y;
        playerSection.position = playerSectionPos;

        // scroll view sizing: expand when the player section merged inside the scroll view
        if (playerSectionPos.y - y > basePlayerSectionLocalY)   // inside the scroll view's viewport
        {
            if (!merged)
            {
                // expand scroll view's size
                scrollView.sizeDelta = mergedScrollViewSize;
                //scrollBar.DOSizeDelta(mergedScrollBarSize, mergeDuration);
                merged = true;
            }
        }
        else if (merged)    // below the scroll view's viewport
        {
            // restore scroll view's size
            scrollView.sizeDelta = baseScrollViewSize;
            //scrollBar.DOSizeDelta(baseScrollBarSize, mergeDuration);
            merged = false;
        }

        // scroll view masking
        if (Mathf.Approximately(playerSectionPos.y - y, topRowLocalY)) // when the player section is at the top of the scroll view
        {
            if (!masked)
            {
                // mask the top of the scroll view to hide scrolling rows
                var padding = mask.padding;
                padding.w = playerSection.sizeDelta.y;
                mask.padding = padding;
                masked = true;
            }
        }
        else if (masked)
        {
            // turn off the mask
            mask.padding = Vector4.zero;
            masked = false;
        }
    }

    internal void Refresh()
    {
        var top = leaderboardManagerSO.Entries.Take(topCount).ToArray();
        for (int i = 0; i < top.Length; i++)
        {
            PersonalInfo entry = top[i];
            var row = rows[i];
            row.Rank = i + 1;
            row.Name = entry.name;
            row.Medal = entry.GetTotalNumOfPoints().RoundToInt();
            row.Avatar = entry.avatar;
            row.AvatarFrame = entry.avatarFrame;
            row.NationalFlag = entry.nationalFlag;
        }

        UpdatePlayerSection();
    }
}