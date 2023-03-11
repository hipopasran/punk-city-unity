using DG.Tweening;
using GLG;
using GLG.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FlyingLabelsOverlay : UIController, IManaged
{
    public enum LabelKind { HPBar, HPLabel, Label, Pointer, PlayerData }

    #region CLASSES
    public class LayoutItem
    {
        public FlyingItem item;

        public bool screenConstrainResolved;
        public bool constrainedToScreen;

        public bool isMainInCombinedItems;
        public List<LayoutItem> combinedWith = new List<LayoutItem>();

        public bool isMainInLayout;
        public List<LayoutItem> collidingResolvedWith = new List<LayoutItem>();
    }
    public class TrackingTarget
    {
        private RectTransform _container;
        private Transform _cashedTransform;
        private Camera _cam;
        private Vector3 _offset;
        public Vector3 PositionOnCanvas => _cam.WorldToScreenPoint(_cashedTransform.position + _offset);
        public Vector3 PositionInContainer => _container.InverseTransformPoint(PositionOnCanvas);
        public bool HasTarget => _cashedTransform != null;
        public Vector3 Offset
        {
            set => _offset = value;
        }
        public Transform TargetTransform => _cashedTransform;

        public TrackingTarget(Transform targetTransform, RectTransform container, Vector3 offset)
        {
            _cashedTransform = targetTransform;
            _cam = Kernel.UI.mainCamera;
            _container = container;
            _offset = offset;
        }
    }
    public abstract class FlyingItem : IManaged
    {
        public static float animationDuration = 1f;
        public static float animationYoffset = 100f;

        protected TrackingTarget _target;
        protected RectTransform _container;
        protected bool _lockOnTarget;

        public abstract LabelKind LabelKind { get; }
        public bool HasTarget => _target.HasTarget;
        public TrackingTarget Target => _target;
        public abstract RectTransform RectTransform { get; }
        public bool StayOnScreen { get; set; }
        public bool CombineWithSimilarItems { get; set; }
        public bool LayoutThisItem { get; set; }
        public abstract bool Visible { get; set; }

        public FlyingItem(Transform target, RectTransform container, Vector3 offset, bool lockOnTarget)
        {
            _target = new TrackingTarget(target, container, offset);
            _container = container;
            _lockOnTarget = lockOnTarget;
        }
        public abstract void ManagedUpdate();
        protected abstract void DoAnimation();
        public abstract void Dispose();
    }
    public class FlyingHPBar : FlyingItem
    {
        private IDamageable _targetDamageable;
        private ProgressBar _progressBar;

        public override RectTransform RectTransform => _progressBar.CashedTransform;

        public FlyingHPBar(Transform target, RectTransform container, Vector3 offset, bool lockOnTarget, ProgressBar prefab, IDamageable targetDamageable) : base(target, container, offset, lockOnTarget)
        {
            _targetDamageable = targetDamageable;
            _progressBar = Instantiate(prefab, container);
            _progressBar.CashedTransform.localPosition = _target.PositionInContainer;
        }

        public override LabelKind LabelKind => LabelKind.HPBar;

        public override bool Visible { get => _progressBar.gameObject.activeSelf; set => _progressBar.gameObject.SetActive(value); }

        public override void Dispose()
        {
            if (_progressBar == null) return;
            GameObject.Destroy(_progressBar.gameObject);
            _progressBar = null;
            _target = null;
        }

        public override void ManagedUpdate()
        {
            if (_lockOnTarget)
            {
                _progressBar.CashedTransform.localPosition = _target.PositionInContainer;
            }
            _progressBar.SetValue(_targetDamageable.CurrentHP / (float)_targetDamageable.MaxHP);
        }

        protected override void DoAnimation()
        {
            _progressBar.CashedTransform.DOLocalMove(_progressBar.CashedTransform.localPosition + Vector3.up * animationYoffset, animationDuration).SetEase(Ease.OutSine);
            _progressBar.CanvasGroup.DOFade(0f, animationDuration / 2f).SetDelay(animationDuration / 2f);
        }
    }
    public class FlyingPlayerData : FlyingItem
    {
        private readonly float _screenOffset = 10f;
        private FlyingPlayerDataVisual _playerDataVisual;
        private bool _stayOnScreen;

        public FlyingPlayerDataVisual PlayerData => _playerDataVisual;
        public override LabelKind LabelKind => LabelKind.PlayerData;
        public override RectTransform RectTransform => _playerDataVisual.CashedTransform;
        public List<FlyingItem> Obstacles { get; set; }
        public override bool Visible { get => _playerDataVisual.gameObject.activeSelf; set => _playerDataVisual.gameObject.SetActive(value); }

        public FlyingPlayerData(Transform target, RectTransform container, Vector3 offset, bool lockOnTarget, FlyingPlayerDataVisual prefab, bool stayOnScreen = true) : base(target, container, offset, lockOnTarget)
        {
            _playerDataVisual = Instantiate(prefab, container);
            _playerDataVisual.transform.localPosition = _target.PositionInContainer;
            _stayOnScreen = false;
        }

        public override void Dispose()
        {
            if (_playerDataVisual == null) return;
            GameObject.Destroy(_playerDataVisual.gameObject);
            _playerDataVisual = null;
            _target = null;
        }
        public override void ManagedUpdate()
        {
            if (_lockOnTarget)
            {
                _playerDataVisual.CashedTransform.localPosition = _target.PositionInContainer;
                if (_stayOnScreen)
                {
                    Vector3 pos = _target.PositionInContainer;
                    Vector2 size = _playerDataVisual.CashedTransform.sizeDelta;
                    Vector2 containerSize = _container.rect.size;
                    if (pos.x - size.x / 2f - _screenOffset < -containerSize.x / 2f)
                    {
                        pos.x = -containerSize.x / 2f + size.x / 2f + _screenOffset;
                    }
                    else if (pos.x + size.x / 2f + _screenOffset > containerSize.x / 2f)
                    {
                        pos.x = containerSize.x / 2f - size.x / 2f - _screenOffset;
                    }
                    _playerDataVisual.CashedTransform.localPosition = pos;
                }
            }
        }
        protected override void DoAnimation()
        {

        }
    }
    public class FlyingPointer : FlyingItem
    {
        private Transform _instanceTransform;
        private CanvasPointer _instance;
        private bool _stayOnScreen;

        public override RectTransform RectTransform => _instance.RectTransform;
        public Vector3 PositiononScreen => _instanceTransform.position;
        public Vector2 Size => _instance.RectTransform.rect.size;
        public override bool Visible { set => _instance.gameObject.SetActive(value); get => _instance.gameObject.activeSelf; }
        public int VisibleInFrames { get; set; }

        public FlyingPointer(Transform target, RectTransform container, Vector3 offset, bool lockOnTarget, bool stayOnScreen, CanvasPointer pointerPrefab) : base(target, container, offset, lockOnTarget)
        {
            _instance = Instantiate(pointerPrefab, container);
            _instanceTransform = _instance.transform;
            _instanceTransform.localPosition = _target.PositionInContainer;
            _stayOnScreen = stayOnScreen;
        }

        public override LabelKind LabelKind { get => LabelKind.Pointer; }
        public override void ManagedUpdate()
        {
            if (_lockOnTarget)
            {
                if (_stayOnScreen)
                {
                    if (VisibleInFrames > 0)
                    {
                        VisibleInFrames--;
                        Visible = false;
                    }
                    else if (VisibleInFrames == -1)
                    {
                        Visible = false;
                    }
                    else
                    {
                        Visible = true;
                    }
                    Vector3 pos = _target.PositionInContainer;
                    Vector2 size = _instance.RectTransform.sizeDelta;
                    Vector2 containerSize = _container.rect.size;
                    if (pos.x - size.x / 2f < -containerSize.x / 2f)
                    {
                        _instance.SetDirection(CanvasPointer.PointerDirection.Left);
                        pos.x = -containerSize.x / 2f;
                    }
                    else if (pos.x + size.x / 2f > containerSize.x / 2f)
                    {
                        _instance.SetDirection(CanvasPointer.PointerDirection.Right);
                        pos.x = containerSize.x / 2f;
                    }
                    else
                    {
                        _instance.SetDirection(CanvasPointer.PointerDirection.Down);
                    }
                    _instanceTransform.localPosition = pos;
                }
                else
                {
                    _instanceTransform.localPosition = _target.PositionInContainer;
                }

            }
        }

        protected override void DoAnimation()
        {

        }

        public override void Dispose()
        {
            if (_instance) Destroy(_instance.gameObject);
            _target = null;
        }
    }
    #endregion


    [Header("Prefabs")]
    [SerializeField] private ProgressBar _hpBarPrefab;
    [SerializeField] private CanvasPointer _canvasPointerPrefab;
    [SerializeField] private FlyingPlayerDataVisual _playerDataVisualPrefab;
    [Header("Settings")]
    [SerializeField] private float _combineItemsDistance;
    [SerializeField] private float _layoutItemsDistance;
    [SerializeField] private float _screenBordersOffset;

    private List<LayoutItem> _items = new List<LayoutItem>();
    private readonly Dictionary<IDamageable, FlyingHPBar> _hpBars = new Dictionary<IDamageable, FlyingHPBar>();
    private readonly Dictionary<Transform, FlyingPointer> _pointers = new Dictionary<Transform, FlyingPointer>();
    private readonly Dictionary<Transform, FlyingPlayerData> _playerDatas = new Dictionary<Transform, FlyingPlayerData>();

    private List<Vector3> _pointersFilter = new List<Vector3>(10);
    private float _avgPointerSize = 0f;
    private float _sqrAvgPointerSize;
    private RectTransform _itemsContainer;

    private void Awake()
    {
        _itemsContainer = _container as RectTransform;
        Kernel.RegisterManaged(this);
    }
    private void OnDestroy()
    {
        Kernel.UnregisterManaged(this);
    }
    public void ManagedUpdate()
    {
        foreach (var item in _hpBars)
        {
            item.Value.ManagedUpdate();
        }

        _pointersFilter.Clear();
        foreach (var item in _pointers)
        {
            bool update = true;
            for (int i = 0; i < _pointersFilter.Count; i++)
            {
                Vector3 pointer = _pointersFilter[i];
                if ((item.Value.PositiononScreen - pointer).sqrMagnitude < _sqrAvgPointerSize)
                {
                    item.Value.VisibleInFrames = -1;
                    update = false;
                    break;
                }
            }
            if (update)
            {
                _pointersFilter.Add(item.Value.PositiononScreen);
                if (item.Value.VisibleInFrames < 0) item.Value.VisibleInFrames = 10;
            }
            item.Value.ManagedUpdate();
        }

        foreach (var item in _playerDatas)
        {
            item.Value.ManagedUpdate();
        }


        foreach (var item in _items)
        {
            FlyingItem flyingItem = item.item;
            //----------------------------------------------------------------------------------------------
            //                                               LAYOUT
            //----------------------------------------------------------------------------------------------
            if (flyingItem.LayoutThisItem && item.collidingResolvedWith.Count == 0)
            {
                List<LayoutItem> intersections = GetIntersectionsForLayout(item);
                if (intersections.Count > 0)
                {
                    item.collidingResolvedWith.AddRange(intersections);
                    foreach (var intersectionItem in intersections)
                    {
                        intersectionItem.collidingResolvedWith.Add(item);
                    }
                    bool useY = false;
                    float yDiff = 0;
                    float xDiff = 0;
                    List<LayoutItem> allItems = new List<LayoutItem>(intersections);
                    allItems.Add(item);
                    xDiff = Mathf.Abs(allItems[0].item.RectTransform.localPosition.x - allItems[1].item.RectTransform.localPosition.x);
                    yDiff = Mathf.Abs(allItems[0].item.RectTransform.localPosition.y - allItems[1].item.RectTransform.localPosition.y);
                    for (int i = 2; i < allItems.Count; i++)
                    {

                        xDiff += Mathf.Abs(allItems[0].item.RectTransform.localPosition.x - allItems[1].item.RectTransform.localPosition.x);
                        xDiff /= 2f;
                        yDiff += Mathf.Abs(allItems[0].item.RectTransform.localPosition.y - allItems[1].item.RectTransform.localPosition.y);
                        yDiff /= 2f;
                    }
                    useY = yDiff > xDiff;

                    List<LayoutItem> sortedItems;
                    if (useY)
                    {
                        sortedItems = allItems.OrderBy(y => y.item.RectTransform.localPosition.y).ToList();
                    }
                    else
                    {
                        sortedItems = allItems.OrderBy(x => x.item.RectTransform.localPosition.x).ToList();
                    }


                    float currX = sortedItems[0].item.RectTransform.localPosition.x + sortedItems[0].item.RectTransform.sizeDelta.x / 2f + _layoutItemsDistance;
                    float currY = sortedItems[0].item.RectTransform.localPosition.y + sortedItems[0].item.RectTransform.sizeDelta.y / 2f + _layoutItemsDistance;
                    for (int i = 1; i < sortedItems.Count; i++)
                    {
                        RectTransform rectTransform = sortedItems[i].item.RectTransform;
                        Vector3 localPos = rectTransform.localPosition;
                        if (useY)
                        {
                            localPos.y = currY + rectTransform.sizeDelta.y / 2f;
                        }
                        else
                        {
                            localPos.x = currX + rectTransform.sizeDelta.x / 2f;
                        }
                        rectTransform.localPosition = localPos;
                        currX += rectTransform.sizeDelta.x + _layoutItemsDistance;
                        currY += rectTransform.sizeDelta.y + _layoutItemsDistance;
                    }
                }
            }
            //----------------------------------------------------------------------------------------------
            //                                               COMBINE
            //----------------------------------------------------------------------------------------------
            else if (flyingItem.CombineWithSimilarItems)
            {
                Debug.Log($"CombineWithSimilarItems: {flyingItem.RectTransform.name}", flyingItem.RectTransform.gameObject);
                if (item.combinedWith.Count > 0) continue;
                List<LayoutItem> intersections = GetIntersectionsForCombine(item);
                if (intersections.Count > 0)
                {
                    Debug.Log($"intersections.Count > 0: {intersections.Count} {intersections[0].item.RectTransform.name}", intersections[0].item.RectTransform.gameObject);
                    flyingItem.Visible = true;
                    item.isMainInCombinedItems = true;
                    foreach (var intersection in intersections)
                    {
                        intersection.item.Visible = false;
                    }
                }
                else
                {
                    if (flyingItem.Visible == false)
                    {
                        flyingItem.Visible = true;
                    }
                }
            }
            //----------------------------------------------------------------------------------------------
            //                                               STAY IN SCREEN
            //----------------------------------------------------------------------------------------------
            if (flyingItem.StayOnScreen && flyingItem.Visible && !item.screenConstrainResolved)
            {
                //--------------------------------------------------
                if (item.collidingResolvedWith.Count > 0)
                {
                    LayoutItem mainItem = null;
                    if (item.isMainInLayout)
                    {
                        mainItem = item;
                    }
                    else
                    {
                        mainItem = item.collidingResolvedWith[0];
                    }
                    Vector2 delta = GetScreenIntersectionDelta(mainItem);
                    if (delta != Vector2.zero)
                    {
                        mainItem.screenConstrainResolved = true;
                        mainItem.item.RectTransform.localPosition += new Vector3(delta.x, delta.y, 0f);
                        foreach (var collision in mainItem.collidingResolvedWith)
                        {
                            collision.screenConstrainResolved = true;
                            collision.item.RectTransform.localPosition += new Vector3(delta.x, delta.y, 0f);
                        }
                    }
                }
                //--------------------------------------------------
                else
                {
                    Vector2 delta = GetScreenIntersectionDelta(item);
                    if (delta != Vector2.zero)
                    {
                        flyingItem.RectTransform.localPosition += new Vector3(delta.x, delta.y, 0f);
                    }
                }
            }
        }
        foreach (var item in _items)
        {
            item.combinedWith.Clear();
            item.collidingResolvedWith.Clear();
            item.isMainInCombinedItems = false;
            item.isMainInLayout = false;
            item.screenConstrainResolved = false;
        }

    }

    public void CreateHpBar(IDamageable target, Vector3 offset)
    {
        FlyingHPBar hpBar = new FlyingHPBar(target.Transform, _itemsContainer, offset, true, _hpBarPrefab, target);
        _hpBars.Add(target, hpBar);
        RegisterItem(hpBar);
    }
    public void RemoveHpBar(IDamageable target)
    {
        _hpBars[target].Dispose();
        _hpBars.Remove(target);
        UnregisterItem(target.Transform);
    }

    public void CreatePointer(Transform target, Vector3 offset)
    {
        FlyingPointer pointer = new FlyingPointer(target, _itemsContainer, offset, true, true, _canvasPointerPrefab);
        Vector2 pointerSize = pointer.Size;
        if (_avgPointerSize == 0f)
        {
            _avgPointerSize = Mathf.Max(pointerSize.x, pointerSize.y);
        }
        else
        {
            _avgPointerSize += Mathf.Max(pointerSize.x, pointerSize.y);
            _avgPointerSize /= 2f;
        }
        //_avgPointerSize /= 2f;
        _sqrAvgPointerSize = _avgPointerSize * _avgPointerSize;
        _pointers.Add(target, pointer);
        RegisterItem(pointer);
    }
    public void RemovePointer(Transform target)
    {
        _pointers[target].Dispose();
        _pointers.Remove(target);
        UnregisterItem(target.transform);
    }

    public FlyingPlayerData CreatePlayerData(Transform target, Vector3 offset, bool lockOnTarget, bool stayOnScreen, bool layout, bool combine)
    {
        FlyingPlayerData flyingPlayerData = new FlyingPlayerData(target, _itemsContainer, offset, lockOnTarget, _playerDataVisualPrefab);
        flyingPlayerData.StayOnScreen = stayOnScreen;
        flyingPlayerData.LayoutThisItem = layout;
        flyingPlayerData.CombineWithSimilarItems = combine;
        _playerDatas.Add(target, flyingPlayerData);
        RegisterItem(flyingPlayerData);
        return flyingPlayerData;
    }
    public void RemovePlayerData(Transform target)
    {
        _playerDatas[target].Dispose();
        _playerDatas.Remove(target);
        UnregisterItem(target.transform);
    }

    private void RegisterItem(FlyingItem flyingItem)
    {
        _items.Add(new LayoutItem() { item = flyingItem });
    }
    private void UnregisterItem(Transform target)
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (_items[i].item.Target.TargetTransform == target)
            {
                _items.RemoveAt(i);
                return;
            }
        }
    }

    private List<LayoutItem> GetIntersectionsForLayout(LayoutItem flyingItem)
    {
        Vector3 itemPos = flyingItem.item.RectTransform.localPosition;
        Vector3 itemSize = flyingItem.item.RectTransform.sizeDelta;
        List<LayoutItem> result = new List<LayoutItem>();
        foreach (var otherItem in _items)
        {
            if (flyingItem == otherItem) continue;
            Vector3 otherItemPos = otherItem.item.RectTransform.localPosition;
            Vector3 otherItemSize = otherItem.item.RectTransform.sizeDelta;
            float xSize = (itemSize.x + otherItemSize.x) / 2f + _layoutItemsDistance;
            if (Mathf.Abs(itemPos.x - otherItemPos.x) < xSize) // intersects on X axis
            {
                float ySize = (itemSize.y + otherItemSize.y) / 2f + _layoutItemsDistance;
                if (Mathf.Abs(itemPos.y - otherItemPos.y) < ySize) // intersects on Y axis
                {
                    result.Add(otherItem);
                }
            }
        }
        return result;
    }
    private List<LayoutItem> GetIntersectionsForCombine(LayoutItem flyingItem)
    {
        Vector3 itemPos = flyingItem.item.RectTransform.localPosition;
        Vector3 itemSize = flyingItem.item.RectTransform.sizeDelta;
        List<LayoutItem> result = new List<LayoutItem>();
        foreach (var otherItem in _items)
        {
            if (flyingItem == otherItem) continue;
            if (otherItem.item.LabelKind != flyingItem.item.LabelKind) continue;
            Vector3 otherItemPos = otherItem.item.RectTransform.localPosition;
            Vector3 otherItemSize = otherItem.item.RectTransform.sizeDelta;
            float xSize = (itemSize.x + otherItemSize.x) / 2f + _combineItemsDistance;
            if (Mathf.Abs(itemPos.x - otherItemPos.x) < xSize) // intersects on X axis
            {
                float ySize = (itemSize.y + otherItemSize.y) / 2f + _combineItemsDistance;
                if (Mathf.Abs(itemPos.y - otherItemPos.y) < ySize) // intersects on Y axis
                {
                    result.Add(otherItem);
                }
            }
        }
        return result;
    }
    private Vector2 GetScreenIntersectionDelta(LayoutItem flyingItem)
    {
        Vector3 localPos = flyingItem.item.RectTransform.localPosition;
        Vector2 size = flyingItem.item.RectTransform.sizeDelta;
        float rightX = localPos.x + size.x / 2f + _screenBordersOffset;
        float leftX = localPos.x - size.x / 2f - _screenBordersOffset;
        float topY = localPos.y + size.y / 2f + _screenBordersOffset;
        float bottomY = localPos.y - size.y / 2f - _screenBordersOffset;

        Vector2 containerSize = _itemsContainer.rect.size;
        Vector2 result = Vector2.zero;

        if (rightX > containerSize.x / 2f)
        {

            result.x = -(rightX - containerSize.x / 2f);
        }
        if (leftX < -containerSize.x / 2f)
        {

            result.x = -containerSize.x / 2f - leftX;
        }
        if (topY > containerSize.y / 2f)
        {

            result.y = -(topY - containerSize.y / 2f);
        }
        if (bottomY < -containerSize.y / 2f)
        {

            result.y = -containerSize.y / 2f - bottomY;
        }
        return result;
    }
}
