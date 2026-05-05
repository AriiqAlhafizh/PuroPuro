using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "ScriptableObjects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    public float IdleDuration = 1f;
    public float WalkDuration = 2f;
}
