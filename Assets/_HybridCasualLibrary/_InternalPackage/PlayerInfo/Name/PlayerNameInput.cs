using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(TMP_InputField))]
public class PlayerNameInput : MonoBehaviour
{
    public const int k_CharacterLimit = 13;
    public const int k_LeaderboardCharacterTruncate = 10;

    [SerializeField] ListVariable<PersonalInfo> playerDatabase;
    [SerializeField] PPrefStringVariable playerName;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] Ease ease = Ease.OutBack;

    private Transform Error => errorText.transform;

    private void Start()
    {
        Error.localScale = Vector3.zero;

        var input = GetComponent<TMP_InputField>();
        input.characterLimit = k_CharacterLimit;
        input.text = playerName.value;
        input.onValidateInput += (s, i, c) => char.ToUpper(c);
        input.onEndEdit.AddListener(name =>
        {
            var nameExisted = playerDatabase.value.Any(playerInfo => playerInfo.name == name && !playerInfo.isLocal);
            if (nameExisted)
            {
                Error.DOKill(true);
                var duration = .5f;
                Error.DOShakePosition(duration);
                Error.DOScale(1, duration).SetEase(ease);
                Error.DOScale(0, duration).SetDelay(2);
                input.text = playerName.value;
            }
            else
            {
                Error.DOKill(true);
                Error.ScaleDown();
                playerName.value = name;
            }
        });
    }
}
