using GLG;
using GLG.UI;
using UnityEngine;
using UnityEngine.UI;

public class BattleSearch_Screen : UIController, IManaged
{
    public event System.Action onCloseButton;

    [SerializeField] private Button _btnClose;
    [SerializeField] private PlayerInfoBlock _playerInfoBlock;
    [SerializeField] private RectTransform _loadingIndicator;

    private float _loadingIndicatorDelay = 0.2f;
    private float _loadingIndicatorAngleStep = 360f / 12f;
    private float _nextIndicatorUpdateTime = 0;

    public PlayerInfoBlock PlayerInfoBlock => _playerInfoBlock;

    private void Awake()
    {
        _btnClose.onClick.AddListener(CloseButtonHandler);
    }
    private void OnEnable()
    {
        Kernel.RegisterManaged(this);
    }
    private void OnDisable()
    {
        Kernel.UnregisterManaged(this);
    }
    private void OnDestroy()
    {
        _btnClose.onClick.RemoveAllListeners();
    }


    public void ManagedUpdate()
    {
        if(Time.time >= _nextIndicatorUpdateTime)
        {
            _loadingIndicator.rotation *= Quaternion.Euler(0f, 0f, _loadingIndicatorAngleStep);
            _nextIndicatorUpdateTime = Time.time + _loadingIndicatorDelay;
        }
    }

    private void CloseButtonHandler()
    {
        onCloseButton?.Invoke();
    }
}
