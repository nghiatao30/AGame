using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI rankText;
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI medalText;
    [SerializeField] protected Image avatar;
    [SerializeField] protected Image avatarFrame;
    [SerializeField] protected Image nationalFlagImage;

    [SerializeField] protected Image bgImage;
    [SerializeField] protected Image rankImage;

    public virtual int RankCap { get; set; } = 200;

    public virtual int Rank
    {
        get => int.Parse(rankText.text);
        set => rankText.text = (value > RankCap) ? "#" + $"{RankCap}+" : "#" +value.ToString();
    }

    public virtual string Name
    {
        get => nameText.text;
        set => nameText.text = value.GetNameTruncate(PlayerNameInput.k_LeaderboardCharacterTruncate);
    }

    public virtual int Medal
    {
        get => int.Parse(medalText.text);
        set => medalText.text = value.ToString() + CurrencyManager.Instance[CurrencyType.Medal].TMPSprite;
    }

    public virtual Sprite Avatar
    {
        get => avatar.sprite;
        set => avatar.sprite = value;
    }

    public virtual Sprite AvatarFrame
    {
        get => avatarFrame.sprite;
        set
        {
            if (value != null)
                avatarFrame.sprite = value;
        }
    }

    public virtual Sprite NationalFlag
    {
        get => nationalFlagImage.sprite;
        set => nationalFlagImage.sprite = value;
    }

    public virtual void SetRowColor(Color _bgColor, Color _rankColor)
    {
        bgImage.color = _bgColor;
        rankImage.color = _rankColor;
    }
}