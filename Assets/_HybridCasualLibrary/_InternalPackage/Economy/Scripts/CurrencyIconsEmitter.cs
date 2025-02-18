using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using LatteGames;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System.Collections;
using LatteGames.Utils;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CurrencyIconsEmitter : MonoBehaviour
{
    [SerializeField] RectTransform currencyIconPrefab;
    [SerializeField] RectTransform baseSrcDes;
    [SerializeField] CurrencyDictionarySO currencySODictionary;
    [SerializeField] int poolPrewarmAmount = 10;
    [InlineEditor]
    public CurrencyEmitterConfigSO currencyEmitterConfigSO;
    ObjectPooling<RectTransform> imagePooling;

    public CurrencyDictionarySO CurrencySODictionary { get => currencySODictionary; }

    private void Awake()
    {
        imagePooling = new ObjectPooling<RectTransform>(
            instantiateMethod: () =>
            {
                var currencyIconImg = Instantiate(currencyIconPrefab);
                currencyIconImg.transform.SetParent(transform, false);
                currencyIconImg.transform.position = new Vector3(0, 100000);
                return currencyIconImg;
            },
            destroyMethod: (obj) => { Destroy(obj.gameObject); }
        );
        imagePooling.PregenerateOffset = poolPrewarmAmount;
    }

    public Emission CreateEmission(CurrencyType currencyType, Vector3 from, Vector3 to, int iconAmount, float emitDuration, float moveDuration, float startRadius = 0f, float endRadius = 0f)
    {
        var emission = new Emission(this, currencyType, baseSrcDes, imagePooling, from, to, iconAmount, emitDuration, moveDuration, startRadius, endRadius);
        return emission;
    }

    public Emission CreateEmission(CurrencyType currencyType, Vector3 from, Vector3 to, int iconAmount)
    {
        return CreateEmission(currencyType, from, to, iconAmount, currencyEmitterConfigSO.emitDuration, currencyEmitterConfigSO.moveDuration, currencyEmitterConfigSO.startRadius, currencyEmitterConfigSO.endRadius);
    }

    public Emission CreateEmission(CurrencyType currencyType, Vector3 from, Vector3 to, float currencyAmount)
    {
        var iconAmount = Mathf.RoundToInt(currencyEmitterConfigSO.currencyIconAmountCurve[currencyType].Evaluate(currencyAmount));
        return CreateEmission(currencyType, from, to, iconAmount);
    }

    public class Emission : Object
    {
        public Emission(
            CurrencyIconsEmitter currencyIconsEmitter,
            CurrencyType currencyType,
            RectTransform baseSrcDes,
            ObjectPooling<RectTransform> pool,
            Vector3 from,
            Vector3 to,
            int amount,
            float emitDuration,
            float moveDuration,
            float startRadius,
            float endRadius)
        {
            this.currencyIconsEmitter = currencyIconsEmitter;
            this.currencyType = currencyType;
            this.baseSrcDes = baseSrcDes;
            this.pool = pool;
            this.from = from;
            this.to = to;
            this.amount = amount;
            this.emitDuration = emitDuration;
            this.moveDuration = moveDuration;
            this.startRadius = startRadius;
            this.endRadius = endRadius;
        }

        CurrencyIconsEmitter currencyIconsEmitter;
        CurrencyType currencyType;
        RectTransform baseSrcDes;
        ObjectPooling<RectTransform> pool;
        Vector3 spawnPos;
        Vector3 from;
        Vector3 to;
        int amount;
        float emitDuration;
        float moveDuration;
        float spawnToFromPosDuration;
        float startRadius;
        float endRadius;
        float startDelay = 0;

        Action eachMoveStart;
        Action eachMoveComplete;
        Action allMoveComplete;

        public void OnEachMoveStart(Action action) => eachMoveStart += action;
        public void OnEachMoveComplete(Action action) => eachMoveComplete += action;
        public void OnAllMoveComplete(Action action) => allMoveComplete += action;
        public void SetStartDelay(float time) => startDelay = time;
        public void SetSpawnToFromPosDuration(float time) => spawnToFromPosDuration = time;
        public void SetSpawnPosition(Vector3 pos) => spawnPos = pos;

        public void Emit()
        {
            if (amount <= 0f)
            {
                allMoveComplete?.Invoke();
                Destroy(this);
                return;
            }

            var src = Instantiate(baseSrcDes, currencyIconsEmitter.transform);
            src.position = from;
            var des = Instantiate(baseSrcDes, currencyIconsEmitter.transform);
            des.position = to;

            var interval = emitDuration / amount;
            int i = 0;
            int moveCompleteCount = 0;
            currencyIconsEmitter.StartCoroutine(CommonCoroutine.Interval(interval, true, () => i >= amount, emitOne));
            void emitOne(int _)
            {
                i++;
                eachMoveStart?.Invoke();
                var iconInstance = pool.Get();
                iconInstance.GetComponent<Image>().sprite = currencyIconsEmitter.CurrencySODictionary[currencyType].emittedIcon;
                if (iconInstance == null)
                {
                    moveComplete();
                }
                else
                {
                    if (spawnPos != default)
                    {
                        iconInstance.anchoredPosition = spawnPos;
                        iconInstance.gameObject.SetActive(true);
                        iconInstance.DOAnchorPos(src.anchoredPosition + startRadius * Random.insideUnitCircle, spawnToFromPosDuration).OnComplete(() =>
                        {
                            moveToDestination();
                        });
                    }
                    else
                    {
                        iconInstance.anchoredPosition = src.anchoredPosition + startRadius * Random.insideUnitCircle;
                        moveToDestination();
                    }
                }

                void moveToDestination()
                {
                    iconInstance.gameObject.SetActive(true);
                    iconInstance.DOAnchorPos(des.anchoredPosition + endRadius * Random.insideUnitCircle, moveDuration).SetUpdate(true).OnComplete(moveComplete).SetDelay(startDelay);
                }

                void moveComplete()
                {
                    if (iconInstance != null)
                    {
                        iconInstance.gameObject.SetActive(false);
                        pool.Add(iconInstance);
                    }
                    eachMoveComplete?.Invoke();
                    moveCompleteCount++;
                    if (moveCompleteCount >= amount)
                    {
                        if (src != null) Destroy(src.gameObject);
                        if (des != null) Destroy(des.gameObject);
                        allMoveComplete?.Invoke();
                        Destroy(this);
                    }
                }
            }
        }

        public void BurstEmit()
        {
            if (amount <= 0f)
            {
                allMoveComplete?.Invoke();
                Destroy(this);
                return;
            }
            var src = Instantiate(baseSrcDes, currencyIconsEmitter.transform);
            src.position = from;
            var des = Instantiate(baseSrcDes, currencyIconsEmitter.transform);
            des.position = to;

            var interval = emitDuration / amount;
            int moveCompleteCount = 0;
            List<BurstData> burstDatas = new List<BurstData>();

            currencyIconsEmitter.StartCoroutine(burstEmitCR());

            IEnumerator burstEmitCR()
            {
                yield return currencyIconsEmitter.StartCoroutine(spawnAllEmitCR());
                if (spawnPos != default || spawnToFromPosDuration <= 0)
                {
                    yield return currencyIconsEmitter.StartCoroutine(moveToFromCR());
                }
                yield return currencyIconsEmitter.StartCoroutine(emitAllCR());
            }

            IEnumerator spawnAllEmitCR()
            {
                for (int i = 0; i < amount; i++)
                {
                    var iconInstance = pool.Get();
                    iconInstance.GetComponent<Image>().sprite = currencyIconsEmitter.CurrencySODictionary[currencyType].emittedIcon;
                    if (iconInstance == null)
                    {
                        moveComplete(null);
                    }
                    if (spawnPos != default)
                    {
                        iconInstance.anchoredPosition = spawnPos;
                        iconInstance.gameObject.SetActive(true);
                        burstDatas.Add(new BurstData()
                        {
                            IconInstance = iconInstance,
                            FromPosition = src.anchoredPosition + startRadius * Random.insideUnitCircle
                        });
                    }
                    else
                    {
                        iconInstance.anchoredPosition = src.anchoredPosition + startRadius * Random.insideUnitCircle;
                        iconInstance.gameObject.SetActive(true);
                        burstDatas.Add(new BurstData()
                        {
                            IconInstance = iconInstance,
                            FromPosition = iconInstance.anchoredPosition
                        });
                    }

                }
                yield break;
            }

            IEnumerator moveToFromCR()
            {
                if (spawnToFromPosDuration <= 0) yield break;
                float t = 0;
                float speed = 1 / spawnToFromPosDuration;
                while (t <= 1)
                {
                    t += Time.deltaTime * speed;
                    foreach (var data in burstDatas)
                    {
                        data.IconInstance.anchoredPosition = Vector3.Lerp(spawnPos, data.FromPosition, t);
                    }
                    yield return null;
                }
            }

            IEnumerator emitAllCR()
            {
                float t = 0;
                float speed = 1 / moveDuration;
                yield return new WaitForSeconds(startDelay);
                while (t <= 1)
                {
                    t += Time.deltaTime * speed;
                    foreach (var data in burstDatas)
                    {
                        data.IconInstance.anchoredPosition = Vector3.Lerp(data.FromPosition, des.anchoredPosition + endRadius * Random.insideUnitCircle, t);
                    }
                    yield return null;
                }
                foreach (var data in burstDatas)
                {
                    moveComplete(data.IconInstance);
                }
                burstDatas.Clear();
            }

            void moveComplete(RectTransform iconInstance)
            {
                if (iconInstance != null)
                {
                    iconInstance.gameObject.SetActive(false);
                    pool.Add(iconInstance);
                }
                eachMoveComplete?.Invoke();
                moveCompleteCount++;
                if (moveCompleteCount >= amount)
                {
                    if (src != null) Destroy(src.gameObject);
                    if (des != null) Destroy(des.gameObject);
                    allMoveComplete?.Invoke();
                    Destroy(this);
                }
            }
        }

        public class BurstData
        {
            public RectTransform IconInstance;
            public Vector2 FromPosition;
        }

        public void RadiateEmit()
        {
            if (amount <= 0f)
            {
                allMoveComplete?.Invoke();
                Destroy(this);
                return;
            }

            var src = Instantiate(baseSrcDes, currencyIconsEmitter.transform);
            src.position = from;
            var des = Instantiate(baseSrcDes, currencyIconsEmitter.transform);
            des.position = to;

            int moveCompleteCount = 0;
            float emitRadius = 0;
            float emitDelay = 0;
            for (int i = 0; i < amount; i++)
            {
                var lerpValue = (float)i / Mathf.Max(1, amount - 1);
                emitRadius = Mathf.Lerp(startRadius * 0.1f, startRadius, lerpValue);
                emitDelay = Mathf.Lerp(currencyIconsEmitter.currencyEmitterConfigSO.radiateDelayRange.x, currencyIconsEmitter.currencyEmitterConfigSO.radiateDelayRange.y, lerpValue);
                emitOne();
            }

            void emitOne()
            {
                eachMoveStart?.Invoke();
                var iconInstance = pool.Get();
                iconInstance.GetComponent<Image>().sprite = currencyIconsEmitter.CurrencySODictionary[currencyType].emittedIcon;
                iconInstance.gameObject.SetActive(true);
                iconInstance.anchoredPosition = src.anchoredPosition;
                iconInstance.DOAnchorPos(src.anchoredPosition + emitRadius * Random.insideUnitCircle.normalized, currencyIconsEmitter.currencyEmitterConfigSO.radiateDuration).SetDelay(emitDelay).SetUpdate(true).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    moveToDestination();
                });
                iconInstance.transform.localScale = Vector2.one * currencyIconsEmitter.currencyEmitterConfigSO.initScale;
                iconInstance.transform.DOScale(Vector3.one, currencyIconsEmitter.currencyEmitterConfigSO.radiateDuration).SetDelay(emitDelay).SetUpdate(true).SetEase(Ease.InCubic);

                void moveToDestination()
                {
                    var moveDelay = Random.Range(currencyIconsEmitter.currencyEmitterConfigSO.moveDelayRange.x, currencyIconsEmitter.currencyEmitterConfigSO.moveDelayRange.y);
                    iconInstance.gameObject.SetActive(true);
                    iconInstance.DOAnchorPos(des.anchoredPosition + endRadius * Random.insideUnitCircle, moveDuration).SetUpdate(true).SetDelay(currencyIconsEmitter.currencyEmitterConfigSO.delayToMove + moveDelay).SetEase(Ease.InBack).OnComplete(moveComplete);
                }

                void moveComplete()
                {
                    if (iconInstance != null)
                    {
                        iconInstance.gameObject.SetActive(false);
                        pool.Add(iconInstance);
                    }
                    eachMoveComplete?.Invoke();
                    moveCompleteCount++;
                    if (moveCompleteCount >= amount)
                    {
                        if (src != null) Destroy(src.gameObject);
                        if (des != null) Destroy(des.gameObject);
                        allMoveComplete?.Invoke();
                        Destroy(this);
                    }
                }
            }
        }
    }
}
