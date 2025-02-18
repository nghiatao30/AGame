using UnityEngine;
using LatteGames.UnpackAnimation;

public abstract class AbstractPack : MonoBehaviour
{
    [SerializeField] Transform packModel;

    public BagDropOnGroundFX packDropOnGroundFX;
    public GameObject packShadow, startDropPoint, endDropPoint, startCamDropRot, endCamDropRot, packCenterCamRot;
    public ParticleSystem lightFX;

    public Transform packTransform => packModel;
    public GameObject packGameObject => packModel.gameObject;
    public Animator packAnimator;
}