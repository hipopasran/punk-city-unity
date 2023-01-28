using UnityEngine;

public interface IDamageable
{
    public int CurrentHP { get; }
    public int MaxHP { get; }
    public void TakeDamage(int damage);
    public Transform Transform { get; }
}
