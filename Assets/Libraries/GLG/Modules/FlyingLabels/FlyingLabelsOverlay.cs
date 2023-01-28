using DG.Tweening;
using GLG;
using GLG.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FlyingLabelsOverlay : UIController, IManaged
{
    public enum LabelKind { HPBar, HPLabel, Label, Pointer }

    #region CLASSES
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
        public abstract LabelKind LableKind { get; }
        protected TrackingTarget _target;
        protected RectTransform _container;
        protected bool _lockOnTarget;
        public bool HasTarget => _target.HasTarget;
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

        public FlyingHPBar(Transform target, RectTransform container, Vector3 offset, bool lockOnTarget, ProgressBar prefab, IDamageable targetDamageable) : base(target, container, offset, lockOnTarget)
        {
            _targetDamageable = targetDamageable;
            _progressBar = Instantiate(prefab, container);
            _progressBar.CashedTransform.localPosition = _target.PositionInContainer;
        }

        public override LabelKind LableKind => LabelKind.HPBar;

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
    public class FlyingHPLabel : FlyingItem
    {
        public FlyingHPLabel(Transform target, RectTransform container, Vector3 offset, bool lockOnTarget, Text prefab) : base(target, container, offset, lockOnTarget)
        {
        }

        public override LabelKind LableKind => throw new System.NotImplementedException();

        public override void Dispose()
        {

        }

        public override void ManagedUpdate()
        {

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

        public Vector3 PositiononScreen => _instanceTransform.position;
        public Vector2 Size => _instance.RectTransform.rect.size;
        public bool Visible { set => _instance.gameObject.SetActive(value); get => _instance.gameObject.activeSelf; }
        public int VisibleInFrames { get; set; }

        public FlyingPointer(Transform target, RectTransform container, Vector3 offset, bool lockOnTarget, bool stayOnScreen, CanvasPointer pointerPrefab) : base(target, container, offset, lockOnTarget)
        {
            _instance = Instantiate(pointerPrefab, container);
            _instanceTransform = _instance.transform;
            _instanceTransform.localPosition = _target.PositionInContainer;
            _stayOnScreen = stayOnScreen;
        }

        public override LabelKind LableKind { get => LabelKind.Pointer; }
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
            if(_instance) Destroy(_instance.gameObject);
            _target = null;
        }
    }
    #endregion


    [Header("Prefabs")] 
    [SerializeField] private ProgressBar _hpBarPrefab;
    [SerializeField] private CanvasPointer _canvasPointerPrefab;

    private readonly Dictionary<IDamageable, FlyingHPBar> _hpBars = new Dictionary<IDamageable, FlyingHPBar>();
    private readonly Dictionary<Transform, FlyingPointer> _pointers = new Dictionary<Transform, FlyingPointer>();
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
            if(update)
            {
                _pointersFilter.Add(item.Value.PositiononScreen);
                if(item.Value.VisibleInFrames < 0) item.Value.VisibleInFrames = 10;
            }
            item.Value.ManagedUpdate();
        }
    }

    public void CreateHpBar(IDamageable target, Vector3 offset)
    {
        FlyingHPBar hpBar = new FlyingHPBar(target.Transform, _itemsContainer, offset, true, _hpBarPrefab, target);
        _hpBars.Add(target, hpBar);
    }
    public void RemoveHpBar(IDamageable target)
    {
        _hpBars[target].Dispose();
        _hpBars.Remove(target);
    }

    public void CreatePointer(Transform target, Vector3 offset)
    {
        FlyingPointer pointer = new FlyingPointer(target, _itemsContainer, offset, true, true, _canvasPointerPrefab);
        Vector2 pointerSize = pointer.Size;
        if(_avgPointerSize == 0f)
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
    }
    public void RemovePointer(Transform target)
    {
        _pointers[target].Dispose();
        _pointers.Remove(target);
    }
}
