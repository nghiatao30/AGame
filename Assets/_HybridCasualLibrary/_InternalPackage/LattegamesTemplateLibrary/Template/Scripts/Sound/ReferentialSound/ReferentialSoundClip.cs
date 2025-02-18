using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
[CreateAssetMenu(fileName = "audio", menuName = "LatteGames/ScriptableObject/ReferentialSoundClip")]
public class ReferentialSoundClip : ScriptableObject
{
    [SerializeField] private AudioClip clip = null;
    public AudioClip Clip => clip;
    public AudioClip[] clips;
}
}