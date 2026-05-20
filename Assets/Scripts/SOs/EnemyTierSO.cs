using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTierSO", menuName = "ScriptableObjects/EnemyTierSO")]
public class EnemyTierSO : ScriptableObject
{
    public List<GameObject> normalEnemyPrefab;
    public List<GameObject> tier2EnemyPrefab;
    public List<GameObject> tier3EnemyPrefab;
    public List<GameObject> tier4EnemyPrefab;
}
