using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum AttackPreparingKind { None, Melee, Ranged }
public enum AttackKind { Pistol, Rifle, Shotgun, Blaster, Circuit, Sword, Katana, Knife, Hammer, Grenade }

public class UnitAnimator : MonoBehaviour, IUnitComponent
{
    public event Action<string> AnimationEvent;
    public event Action OnAttackCompleted;

    [Header("Jumping settings")]
    [SerializeField] private float _jumpDuration = 1f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private AnimationCurve _jumpHightCurve;
    [SerializeField] private AnimationCurve _jumpMovingCurve;
    [Header("Links")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _objectForJumpAnimation;
    [Header("Animator keys")]
    [SerializeField] private string _movingSpeedParameterName = "MovingSpeed";
    [Space]
    [SerializeField] private string _attackStartKey = "attack_start";
    [SerializeField] private string _attackPistolKey = "attack_pistol";
    [SerializeField] private string _attackRifleKey = "attack_rifle";
    [SerializeField] private string _attackShotgunKey = "attack_shotgun";
    [SerializeField] private string _attackBlasterKey = "attack_blaster";
    [SerializeField] private string _attackCircuitKey = "attack_circuit";
    [SerializeField] private string _attackSwordKey = "attack_sword";
    [SerializeField] private string _attackKatanaKey = "attack_katana";
    [SerializeField] private string _attackKnifeKey = "attack_knife";
    [SerializeField] private string _attackHammerKey = "attack_hammer";
    [SerializeField] private string _attackGrenadeKey = "attack_grenade";
    [Space]
    [SerializeField] private string _damageStartKey = "damage_start";
    [SerializeField] private string _damageLightKey = "damage_light";
    [SerializeField] private string _damageHeavyKey = "damage_heavy";
    [Space]
    [SerializeField] private string _deathStartKey = "death_start";
    [SerializeField] private string _deathExplosionKey = "death_explosion";
    [SerializeField] private string _deathShotKey = "death_shot";
    [SerializeField] private string _deathMeleeKey = "death_melee";
    [Space]
    [SerializeField] private string _victoryKey = "victory";
    [Space]
    [SerializeField] private string _idleStartKey = "idle_start";
    [Space]
    [SerializeField] private string _prepareMeleeKey = "prepare_melee";
    [SerializeField] private string _prepareRangedKey = "prepare_ranged";
    [Space]
    [SerializeField] private string _locationKey = "location";
    [Space]
    [SerializeField] private string _jumpInKey = "jump_in";
    [SerializeField] private string _jumpOutKey = "jump_out";

    private Unit _unit;
    private IEnumerator _jumpingRoutine;
    private Vector3 _jumpObjectDefaultPosition;
    private Vector3? _jumpTarget;
    private bool _jumping;
    private Transform _jumpPoint;

    public Unit Unit => _unit;
    public Transform JumpPoint { get => _jumpPoint; set => _jumpPoint = value; }

    public void InitializeOn(Unit unit)
    {
        _unit = unit;
        _jumpObjectDefaultPosition = _objectForJumpAnimation.position;
    }

    public void SetTrigger(string parameterName)
    {
        _animator.SetTrigger(parameterName);
    }
    public void ResetTrigger(string parameterName)
    {
        _animator.ResetTrigger(parameterName);
    }
    public void SetFloat(string parameterName, float value)
    {
        _animator.SetFloat(parameterName, value);
    }
    public void SetBool(string parameterName, bool value)
    {
        _animator.SetBool(parameterName, value);
    }
    public void SetInt(string parameterName, int value)
    {
        _animator.SetInteger(parameterName, value);
    }
    public void ApplyMovementSpeed(float speed)
    {
        _animator.SetFloat(_movingSpeedParameterName, speed);
    }

    public void PlayAttack(AttackPreparingKind attackPreparingKind, AttackKind attackKind)
    {
        PlayAttack(_jumpPoint, attackPreparingKind, attackKind);
    }
    public void PlayAttack(Transform jumpToPoint, AttackPreparingKind attackPreparingKind, AttackKind attackKind)
    {
        switch (attackPreparingKind)
        {
            case AttackPreparingKind.None:
                break;
            case AttackPreparingKind.Melee:
                SetBool(_prepareMeleeKey, true);
                break;
            case AttackPreparingKind.Ranged:
                SetBool(_prepareRangedKey, true);
                break;
        }
        switch (attackKind)
        {
            case AttackKind.Pistol:
                SetBool(_attackPistolKey, true);
                break;
            case AttackKind.Rifle:
                SetBool(_attackRifleKey, true);
                break;
            case AttackKind.Shotgun:
                SetBool(_attackShotgunKey, true);
                break;
            case AttackKind.Blaster:
                SetBool(_attackBlasterKey, true);
                break;
            case AttackKind.Circuit:
                SetBool(_attackCircuitKey, true);
                break;
            case AttackKind.Sword:
                SetBool(_attackSwordKey, true);
                break;
            case AttackKind.Katana:
                SetBool(_attackKatanaKey, true);
                break;
            case AttackKind.Knife:
                SetBool(_attackKnifeKey, true);
                break;
            case AttackKind.Grenade:
                SetBool(_attackGrenadeKey, true);
                break;
            case AttackKind.Hammer:
                SetBool(_attackHammerKey, true);
                break;
            default:
                break;
        }
        SetTrigger("attack_start");
        _jumping = jumpToPoint != null;
        _jumpTarget = jumpToPoint?.position;
        if (attackPreparingKind == AttackPreparingKind.None && _jumping)
        {
            if(_jumpingRoutine != null)
            {
                StopCoroutine(_jumpingRoutine);
            }
            _jumpingRoutine = JumpAnimation();
            StartCoroutine(_jumpingRoutine);
        }
    }
    public void PlayIdle()
    {
        SetTrigger(_idleStartKey);
    }
    public void PlayDamage(AttackKind fromAttack)
    {
        switch (fromAttack)
        {
            case AttackKind.Pistol:
                SetBool(_damageLightKey, true);
                break;
            case AttackKind.Rifle:
                SetBool(_damageLightKey, true);
                break;
            case AttackKind.Shotgun:
                SetBool(_damageHeavyKey, true);
                break;
            case AttackKind.Blaster:
                SetBool(_damageLightKey, true);
                break;
            case AttackKind.Circuit:
                SetBool(_damageLightKey, true);
                break;
            case AttackKind.Sword:
                SetBool(_damageLightKey, true);
                break;
            case AttackKind.Katana:
                SetBool(_damageLightKey, true);
                break;
            case AttackKind.Knife:
                SetBool(_damageLightKey, true);
                break;
            case AttackKind.Grenade:
                SetBool(_damageHeavyKey, true);
                break;
            case AttackKind.Hammer:
                SetBool(_damageHeavyKey, true);
                break;
            default:
                break;
        }
    }
    public void PlayDeath(AttackKind fromAttack)
    {
        switch (fromAttack)
        {
            case AttackKind.Pistol:
                SetBool(_deathShotKey, true);
                break;
            case AttackKind.Rifle:
                SetBool(_deathShotKey, true);
                break;
            case AttackKind.Shotgun:
                SetBool(_deathShotKey, true);
                break;
            case AttackKind.Blaster:
                SetBool(_deathShotKey, true);
                break;
            case AttackKind.Circuit:
                SetBool(_deathShotKey, true);
                break;
            case AttackKind.Sword:
                SetBool(_deathMeleeKey, true);
                break;
            case AttackKind.Katana:
                SetBool(_deathMeleeKey, true);
                break;
            case AttackKind.Knife:
                SetBool(_deathMeleeKey, true);
                break;
            case AttackKind.Grenade:
                SetBool(_deathExplosionKey, true);
                break;
            case AttackKind.Hammer:
                SetBool(_deathMeleeKey, true);
                break;
            default:
                break;
        }
    }
    public void SetLocation(int location)
    {
        SetInt(_locationKey, location);
    }
    public void FullReset()
    {
        ResetTrigger(_idleStartKey);
        ResetTrigger(_attackStartKey);
        ResetTrigger(_damageStartKey);
        ResetTrigger(_deathStartKey);

        SetBool(_damageHeavyKey, false);
        SetBool(_damageLightKey, false);

        SetBool(_deathExplosionKey, false);
        SetBool(_deathMeleeKey, false);
        SetBool(_deathShotKey, false);

        SetBool(_attackBlasterKey, false);
        SetBool(_attackCircuitKey, false);
        SetBool(_attackGrenadeKey, false);
        SetBool(_attackHammerKey, false);
        SetBool(_attackKatanaKey, false);
        SetBool(_attackKnifeKey, false);
        SetBool(_attackSwordKey, false);
        SetBool(_attackPistolKey, false);
        SetBool(_attackRifleKey, false);
        SetBool(_attackShotgunKey, false);

        SetBool(_prepareMeleeKey, false);
        SetBool(_prepareRangedKey, false);

        SetBool(_jumpInKey, false);
        SetBool(_jumpOutKey, false);

        _animator.Rebind();
    }

    public void AnimationEventHandler(string parameter)
    {
        AnimationEvent?.Invoke(parameter);
        switch (parameter)
        {
            case "jump_in":
                JumpHandler(true);
                break;
            case "jump_out":
                JumpHandler(false);
                break;
            case "attack":
                AttackHandler();
                break;
            case "attack_stop":
                AttackStopHandler();
                break;
            case "prepared":
                PreparedHandler();
                break;
            case "death":
                DeathHandler();
                break;
            case "damage":
                DamageHandler();
                break;
            default:
                break;
        }
    }

    private IEnumerator JumpAnimation(System.Action callback = null)
    {
        float t = 0f, progress = 0f;
        Vector3 startPos = _objectForJumpAnimation.position;
        Vector3 endPos = (Vector3)_jumpTarget;
        Vector3 currentPosition;
        while (progress < 1f)
        {
            t += Time.deltaTime;
            progress = t / _jumpDuration;
            currentPosition = Vector3.Lerp(startPos, endPos, _jumpMovingCurve.Evaluate(progress));
            currentPosition.y += Mathf.Lerp(0, _jumpHeight, _jumpHightCurve.Evaluate(progress));
            _objectForJumpAnimation.position = currentPosition;
            yield return null;
        }
        callback?.Invoke();
        yield break;
    }

    private void JumpHandler(bool intro)
    {
        SetBool(_jumpInKey, false);
        SetBool(_jumpOutKey, false);
    }
    private void PreparedHandler()
    {
        SetBool(_prepareMeleeKey, false);
        SetBool(_prepareRangedKey, false);
        if (_jumping)
        {
            _jumpTarget = _jumpObjectDefaultPosition;
            if (_jumpingRoutine != null)
            {
                StopCoroutine(_jumpingRoutine);
            }
            _jumpingRoutine = JumpAnimation();
            StartCoroutine(_jumpingRoutine);
        }
    }
    private void AttackHandler()
    {
        ResetTrigger(_attackStartKey);
        SetBool(_attackBlasterKey, false);
        SetBool(_attackCircuitKey, false);
        SetBool(_attackGrenadeKey, false);
        SetBool(_attackHammerKey, false);
        SetBool(_attackKatanaKey, false);
        SetBool(_attackKnifeKey, false);
        SetBool(_attackSwordKey, false);
        SetBool(_attackPistolKey, false);
        SetBool(_attackRifleKey, false);
        SetBool(_attackShotgunKey, false);
    }
    private void AttackStopHandler()
    {
        if(_jumping)
        {
            if (_jumpingRoutine != null)
            {
                StopCoroutine(_jumpingRoutine);
            }
            _jumpingRoutine = JumpAnimation(() => { OnAttackCompleted?.Invoke(); });
            StartCoroutine(_jumpingRoutine);
        }
        else
        {
            OnAttackCompleted?.Invoke();
        }
        _jumping = false;

    }
    private void DamageHandler()
    {
        ResetTrigger(_damageStartKey);
        SetBool(_damageHeavyKey, false);
        SetBool(_damageLightKey, false);
    }
    private void DeathHandler()
    {
        ResetTrigger(_deathStartKey);
        SetBool(_deathExplosionKey, false);
        SetBool(_deathMeleeKey, false);
        SetBool(_deathShotKey, false);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UnitAnimator))]
public class UnitAnimatorEditor : Editor
{
    private UnitAnimator _target;
    private void OnEnable()
    {
        _target = target as UnitAnimator;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Play attack: grenade"))
        {
            _target.PlayAttack(null, AttackPreparingKind.None, AttackKind.Grenade);
        }
        if (GUILayout.Button("Play attack: blaster"))
        {
            _target.PlayAttack(null, AttackPreparingKind.Ranged, AttackKind.Blaster);
        }
        if (GUILayout.Button("Play attack: pistol"))
        {
            _target.PlayAttack(null, AttackPreparingKind.Ranged, AttackKind.Pistol);
        }
        if (GUILayout.Button("Play attack: shotgun"))
        {
            _target.PlayAttack(null, AttackPreparingKind.Ranged, AttackKind.Shotgun);
        }
        if (GUILayout.Button("Play attack: rifle"))
        {
            _target.PlayAttack(null, AttackPreparingKind.Ranged, AttackKind.Rifle);
        }
        if (GUILayout.Button("Play attack: knife"))
        {
            _target.PlayAttack(_target.JumpPoint, AttackPreparingKind.Melee, AttackKind.Knife);
        }
        if (GUILayout.Button("Play attack: sword"))
        {
            _target.PlayAttack(_target.JumpPoint, AttackPreparingKind.Melee, AttackKind.Sword);
        }
        if (GUILayout.Button("Play attack: katana"))
        {
            _target.PlayAttack(_target.JumpPoint, AttackPreparingKind.Melee, AttackKind.Katana);
        }
        if (GUILayout.Button("Play attack: hammer"))
        {
            _target.PlayAttack(_target.JumpPoint, AttackPreparingKind.Melee, AttackKind.Hammer);
        }
        if (GUILayout.Button("Play attack: circuit"))
        {
            _target.PlayAttack(null, AttackPreparingKind.Ranged, AttackKind.Circuit);
        }
        GUILayout.Space(10f);
    }
}
#endif
