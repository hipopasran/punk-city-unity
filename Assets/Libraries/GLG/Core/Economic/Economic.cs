using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Economic : MonoBehaviour
{
    [SerializeField] private PlayerShopsManager _playerShopsManager;
    private PlayerMoney _playerMoney;
    public static Economic i;
    public PlayerMoney PlayerMoney => _playerMoney;
    public PlayerShopsManager PlayerShopsManager => _playerShopsManager;
    private void Awake()
    {
        i = this;
        _playerMoney = new PlayerMoney();
    }

    /// <summary>
    /// @binding M - give money;
    /// </summary>
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            _playerMoney.Money += 1000;
        }
#endif
        _playerMoney.ManagedUpdate();
    }
    private void OnDestroy()
    {
        _playerMoney.Save();
    }
    private void OnApplicationQuit()
    {
        _playerMoney.Save();
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        _playerMoney.Save();
    }
}

public class PlayerMoney
{
    private int _money;
    public static event System.Action<int> onUpdateMoney;
    public static event Action<MoneyTypes> onUpdateOtherMoney = (_) => { };
    private Dictionary<MoneyTypes, int> _otherMoney = new Dictionary<MoneyTypes, int>();
    private float _timeToNextSaveMoney;
    private bool _needSave;
    private int _startMoney;
    private List<MoneyTypes> _otherMoneyToSave = new List<MoneyTypes>();

    public PlayerMoney()
    {
        _startMoney = PlayerPrefs.GetInt("Player_money", _startMoney);
        Money = _startMoney;
        onUpdateMoney?.Invoke(_startMoney);
    }
    public int Money
    {
        get => _money;
        set
        {
            _money = value;
            onUpdateMoney?.Invoke(_money);
            _timeToNextSaveMoney = Time.time + 1f;
            _needSave = true;
        }
    }
    public void AddMoney(int value)
    {
        Money += value;
    }
    public bool TrySpendMoney(int moneyToSpend)
    {
        if (_money >= moneyToSpend)
        {
            Money -= moneyToSpend;
            return true;
        }
        else return false;
    }
    public bool InteractWithOtherMoney(MoneyTypes key, int value)
    {
        if (value == 0) return true;
        if (value > 0)
        {
            _otherMoney[key] += value;
            if (!_otherMoneyToSave.Contains(key)) _otherMoneyToSave.Add(key);
            _timeToNextSaveMoney = Time.time + 1f;
            _needSave = true;
            return true;
        }
        else
        {
            if (_otherMoney[key] >= value)
            {
                _otherMoney[key] += value;
                if (!_otherMoneyToSave.Contains(key)) _otherMoneyToSave.Add(key);
                _timeToNextSaveMoney = Time.time + 1f;
                _needSave = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public void ManagedUpdate()
    {
        if (_needSave && Time.time > _timeToNextSaveMoney)
        {
            Save();
        }
    }
    public void Save()
    {
        _needSave = false;
        PlayerPrefs.SetInt("Player_money", Money);
        foreach (var item in _otherMoneyToSave)
        {
            PlayerPrefs.SetInt(item.ToString(), Money);
        }
        PlayerPrefs.Save();
    }
}