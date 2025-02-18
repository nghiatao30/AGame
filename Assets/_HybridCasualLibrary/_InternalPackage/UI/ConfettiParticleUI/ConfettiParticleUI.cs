using System.Collections;
using System.Collections.Generic;
using LatteGames;
using LatteGames.Template;
using UnityEngine;

public class ConfettiParticleUI : Singleton<ConfettiParticleUI>
{
    [SerializeField] ParticleSystem confettiFX;
    Coroutine delayToPlayConfettiCoroutine;
    public void PlayFX()
    {
        if (delayToPlayConfettiCoroutine != null)
        {
            StopCoroutine(delayToPlayConfettiCoroutine);
        }
        confettiFX.Stop();
        confettiFX.Clear();
        delayToPlayConfettiCoroutine = StartCoroutine(CommonCoroutine.Delay(0.01f, false, () =>
        {
            confettiFX.Play();
            SoundManager.Instance.PlaySFX(GeneralSFX.Confetti);
        }));
    }
}
