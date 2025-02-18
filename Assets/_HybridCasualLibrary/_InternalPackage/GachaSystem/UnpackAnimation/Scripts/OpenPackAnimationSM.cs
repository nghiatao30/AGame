using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LatteGames.EditableStateMachine;
using HyrphusQ.Events;
using GachaSystem.Core;
using TMPro;

[EventCode]
public enum UnpackEventCode
{
    /// <summary>
    /// Raised when begin unpack process.
    /// <para>Parameters:</para>
    /// <list type="number">
    ///     <item>[TYPE: List&lt;GachaCard &gt;] : nonPackCards</item>
    ///     <item>[TYPE: List&lt;GachaPack &gt;] : packsToOpen</item>
    ///     <item>[TYPE: List&lt;OfferGachaPack &gt;] : packsToOffer</item>
    /// </list>
    /// </summary>
    OnUnpackStart,
    /// <summary>
    /// Raised when unpack process is done.
    /// <list type="number">
    ///     <item>[TYPE: List&lt;SubPackInfo &gt;] : subPackInfos</item>
    /// </list>
    /// </summary>
    OnUnpackDone,
    /// <summary>
    /// Raised when starting the opening a sub pack.
    /// <list type="number">
    ///     <item>[TYPE: SubPackInfo] : subPackInfo</item>
    /// </list>
    /// </summary>
    OnOpenSubPackStart
}
public enum CardPlace
{
    NonPack,
    NormalPack,
    OfferPack
}
namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    public class OpenPackAnimationSM : EditableStateMachineController
    {
        /// <summary>
        /// Raised when player tap inside OpenPack screen
        /// </summary>
        public event Action OnMouseClicked = delegate { };
        /// <summary>
        /// Raised when player click on 'Skip' button
        /// </summary>
        public event Action OnSkipButtonClicked = delegate { };
        // ----------------------- Configs -----------------------------
        [SerializeField] protected bool hasBonusCard;

        // ----------------------- References -----------------------------
        [SerializeField] protected Camera m_Cam;
        [SerializeField] protected Light m_Light;
        [SerializeField] protected GameObject unpackGameObject;
        [SerializeField] protected Button fullScreenBtn, skipBtn;
        [SerializeField] protected TMP_Text tapCTA, tapToOpenCTA, packNameTxt, freeTxt;
        [SerializeField] protected Transform m_3dObjectContainer, nonInteractiveCanvas, interactiveCanvas, fxCanvas;
        [SerializeField] protected CanvasGroup unpackUICanvasGroup;
        [SerializeField] protected TMP_Text remainingCardAmountText;
        [SerializeField] protected GameObject unpackFTUE_CTA;
        [SerializeField] protected Image darkenImage;
        [SerializeField] protected Image lightImage;
        [SerializeField] protected TMP_Text titleNewCard;
        [SerializeField] protected TMP_Text newCardUnlockedTxt;

        // ----------------------- Public References -----------------------------
        public Transform NonInteractiveCanvas => nonInteractiveCanvas;
        public Transform InteractiveCanvas => interactiveCanvas;
        public Transform FXCanvas => fxCanvas;

        public TMP_Text TapCTA => tapCTA;
        public TMP_Text TapToOpenCTA => tapToOpenCTA;
        public TMP_Text PackNameTxt => packNameTxt;
        public TMP_Text FreeTxt => freeTxt;
        public TMP_Text TitleNewCardTxt => titleNewCard;
        public TMP_Text NewCardUnlockedTxt => newCardUnlockedTxt;
        public Light Light => m_Light;
        public Camera Camera => m_Cam;
        public CanvasGroup UnpackUICanvasGroup => unpackUICanvasGroup;
        public GameObject UnpackFTUE_CTA => unpackFTUE_CTA;
        public Button FullScreenBtn => fullScreenBtn;
        public Button SkipBtn => skipBtn;
        public Image DarkenImage => darkenImage;
        public Image LightImage => lightImage;

        // ----------------------- Instances ------------------------------
        protected AbstractPack packInstance;

        // ----------------------- Public Variables -----------------------
        [HideInInspector] public List<SubPackInfo> subPackInfos = new List<SubPackInfo>();

        // ----------------------- Properties -----------------------------
        public int CurrentSubPackIndex
        {
            get
            {
                return currentPackIndex;
            }
            set
            {
                if (currentPackIndex != value)
                {
                    currentPackIndex = value;
                }
            }
        }
        public int CurrentShowingCardIndex
        {
            get
            {
                return CurrentGroupedCards.Count - RemainingItemAmount;
            }
        }
        public bool HasOfferSubPack => subPackInfos.Find((item) => item.cardPlace == CardPlace.OfferPack) != null;
        /// <summary>
        /// First normal bag in unpack session
        /// </summary>
        public bool IsFirstNormalSubPack => CurrentSubPackIndex == firstNormalPackIndex;

        /// <summary>
        /// Current grouped card list
        /// </summary>
        public List<DuplicateGachaCardsGroup> CurrentGroupedCards
        {
            get
            {
                return CurrentSubPackInfo.duplicateGachaCardsGroups;
            }
        }
        /// <summary>
        /// Current grouped card
        /// </summary>
        public DuplicateGachaCardsGroup CurrentGroupedCard
        {
            get
            {
                return CurrentGroupedCards[CurrentShowingCardIndex];
            }
        }

        /// <summary>
        /// Current open gacha pack
        /// </summary>
        public GachaPack CurrentGachaPack
        {
            get
            {
                return CurrentSubPackInfo.gachaPack;
            }
        }
        public SubPackInfo CurrentSubPackInfo => subPackInfos[CurrentSubPackIndex];
        /// <summary>
        /// 3D model pack instance
        /// </summary>
        public AbstractPack PackInstance
        {
            get
            {
                if (CurrentPackPrefab == null)
                    return null;
                if (currentPackPrefabParent == null || currentPackPrefabParent != CurrentPackPrefab || packInstance == null)
                {
                    currentPackPrefabParent = CurrentPackPrefab;
                    packInstance = Instantiate(currentPackPrefabParent, m_3dObjectContainer);
                }
                return packInstance;
            }
        }


        /// <summary>
        /// Remain card amount in the opening gacha pack
        /// </summary>
        public int RemainingItemAmount
        {
            get => remainingItemAmount;
            set
            {
                remainingItemAmount = value;
                UpdateRemainCardText();
            }
        }
        public bool IsLastSubPack => CurrentSubPackIndex >= subPackInfos.Count - 1;
        protected AbstractPack CurrentPackPrefab => CurrentGachaPack?.PackPrefab;
        public bool HasBonusCard { get => hasBonusCard; set => hasBonusCard = value; }

        // ---------------------- protected Variables -----------------------
        protected AbstractPack currentPackPrefabParent = null;
        protected int remainingItemAmount = 0;
        protected int currentPackIndex = 0;
        protected int firstNormalPackIndex;

        protected override void Awake()
        {
            object[] parameters = new object[]
            {
                this
            };
            foreach (var state in states)
            {
                state.SetupState(parameters);
            }

            unpackGameObject.SetActive(false);

            GameEventHandler.AddActionEvent(UnpackEventCode.OnUnpackStart, Unpack);
        }

        protected void OnDestroy()
        {
            GameEventHandler.RemoveActionEvent(UnpackEventCode.OnUnpackStart, Unpack);
        }

        protected void Start()
        {
            skipBtn.onClick.AddListener(() => OnSkipButtonClicked());
            fullScreenBtn.onClick.AddListener(() => OnMouseClicked());
        }

        public void StartOpenSubPack()
        {
            if (IsLastSubPack)
            {
                Debug.LogError("There are no more pack to open");
                return;
            }

            PackNameTxt.gameObject.SetActive(false);
            FreeTxt.gameObject.SetActive(false);

            //Refresh the pack instance
            if (packInstance)
            {
                DestroyImmediate(packInstance.gameObject);
            }
            CurrentSubPackIndex++;
            RemainingItemAmount = CurrentGroupedCards.Count;
        }

        public virtual void Unpack(object[] parameters)
        {
            subPackInfos.Clear();
            if (parameters[0] is List<GachaCard>)
            {
                var nonPackCards = (List<GachaCard>)parameters[0];
                HandleGachaCardList(nonPackCards, CardPlace.NonPack);
            }
            if (parameters[1] is List<GachaPack>)
            {
                var packsToOpen = (List<GachaPack>)parameters[1];
                if (packsToOpen.Count > 0)
                {
                    firstNormalPackIndex = subPackInfos.Count;
                }
                HandleGachaPackList(packsToOpen, CardPlace.NormalPack);
            }

            if (parameters[2] is List<OfferGachaPack>)
            {
                var packsToOffer = (List<OfferGachaPack>)parameters[2];
                HandleOfferGachaPackList(packsToOffer, CardPlace.OfferPack);
            }
            CurrentSubPackIndex = -1;

            unpackGameObject.SetActive(true);
            StartStateMachine();
        }

        public override void StopStateMachine()
        {
            base.StopStateMachine();
            GameEventHandler.Invoke(UnpackEventCode.OnUnpackDone, subPackInfos);
            unpackGameObject.SetActive(false);
        }

        protected virtual void HandleGachaCardList(List<GachaCard> gachaCards, CardPlace cardPlace, GachaPack gachaPack = null)
        {
            if (gachaCards.Count <= 0)
            {
                return;
            }
            foreach (var card in gachaCards)
            {
                card.GrantReward();
            }
            var duplicateGachaCardsGroups = gachaCards.GroupDuplicate();
            if (hasBonusCard && cardPlace == CardPlace.NormalPack)
            {
                duplicateGachaCardsGroups.Add(new DuplicateGachaCardsGroup() { isBonusCard = true });
            }
            subPackInfos.Add(new SubPackInfo()
            {
                duplicateGachaCardsGroups = duplicateGachaCardsGroups,
                cardPlace = cardPlace,
                gachaPack = gachaPack
            });
        }

        protected virtual void HandleGachaPackList(List<GachaPack> gachaPacks, CardPlace cardPlace)
        {
            foreach (var gachaPack in gachaPacks)
            {
                var cards = gachaPack.GenerateCards();
                HandleGachaCardList(cards, cardPlace, gachaPack);
            }
        }

        protected virtual void HandleOfferGachaPackList(List<OfferGachaPack> offerGachaPacks, CardPlace cardPlace)
        {
            var gachaPacks = offerGachaPacks.ConvertAll(x => x.gachaPack);
            foreach (var offerGachaPack in offerGachaPacks)
            {
                var cards = offerGachaPack.gachaPack.GenerateCards();
                var duplicateGachaCardsGroups = cards.GroupDuplicate();
                subPackInfos.Add(new SubPackInfo()
                {
                    duplicateGachaCardsGroups = duplicateGachaCardsGroups,
                    cardPlace = cardPlace,
                    gachaPack = offerGachaPack.gachaPack,
                    offerRVAdsLocation = offerGachaPack.offerRVAdsLocation
                });
            }
        }

        public virtual void GrantRewardOfferPack()
        {
            foreach (var duplicateGachaCardsGroup in CurrentSubPackInfo.duplicateGachaCardsGroups)
            {
                for (int i = 0; i < duplicateGachaCardsGroup.cardsAmount; i++)
                {
                    duplicateGachaCardsGroup.representativeCard.GrantReward();
                }
            }
        }

        public virtual void UpdateRemainCardText()
        {
            remainingCardAmountText.text = RemainingItemAmount.ToString();
        }

        public class MouseClickEvent : StateMachine.Event
        {
            internal OpenPackAnimationSM controller;
            internal Action callback;

            public override void Enable()
            {
                base.Enable();
                if (controller == null) return;
                controller.OnMouseClicked += HandleMouseClicked;
            }

            public override void Disable()
            {
                base.Disable();
                if (controller == null) return;
                controller.OnMouseClicked -= HandleMouseClicked;
            }

            protected virtual void HandleMouseClicked()
            {
                if (controller == null) return;
                if (Enabled == false) return;
                Trigger();
                callback?.Invoke();
            }
        }

        public class SkipEvent : StateMachine.Event
        {
            internal OpenPackAnimationSM controller;
            internal Action callback;

            public override void Enable()
            {
                base.Enable();
                if (controller == null) return;
                controller.OnSkipButtonClicked += HandleSkipButtonClicked;
            }

            public override void Disable()
            {
                base.Disable();
                if (controller == null) return;
                controller.OnSkipButtonClicked -= HandleSkipButtonClicked;
            }

            protected virtual void HandleSkipButtonClicked()
            {
                if (controller == null) return;
                if (Enabled == false) return;
                Trigger();
                callback?.Invoke();
            }
        }
    }
    /// <summary>
    /// SubPackInfo contains info of the sub pack in the main pack.
    /// </summary>
    public class SubPackInfo
    {
        public List<DuplicateGachaCardsGroup> duplicateGachaCardsGroups;
        public CardPlace cardPlace;
        public GachaPack gachaPack;
        public AdsLocation offerRVAdsLocation;
    }

    [Serializable]
    public struct OfferGachaPack
    {
        public GachaPack gachaPack;
        public AdsLocation offerRVAdsLocation;
    }
}