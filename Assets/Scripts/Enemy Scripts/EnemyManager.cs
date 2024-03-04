using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<Enemy> _enemies = new List<Enemy>();

    public List<Enemy> DeadEnemies { get; } = new List<Enemy>();

    [SerializeField] private GameObject _goalPrefab;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private Vector3 _debugPos;
    
    void Awake()
    {
        _enemies.AddRange(FindObjectsOfType<Enemy>());
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].OnDeath += OnEnemyDied;
        }
    }

    private void OnEnemyDied(Enemy deadEnemy)
    {
        if (_enemies.Count == 1)
        {
            if (Physics.Raycast(deadEnemy.transform.position + Vector3.up * 2, Vector3.down, out var raycastHit, 100, _layerMask))
            {
                Instantiate(_goalPrefab, raycastHit.point, _enemies[0].transform.rotation);
            }
            
        } 
        
        deadEnemy.OnDeath -= OnEnemyDied;
        _enemies.Remove(deadEnemy);
        DeadEnemies.Add(deadEnemy);
    }

    public Dictionary<GolfController, int> GetPlayerScores()
    {
        var scores = new Dictionary<GolfController, int>();
        for (int i = 0; i < DeadEnemies.Count; i++)
        {
            var p = DeadEnemies[i].OwnerPlayer;
            if (!scores.TryAdd(p, 1))
            {
                scores[p]++;
            }
        }

        return scores;
    }
    
    [ContextMenu("Spawn Goal")]
    public void DebugGoalSpawning()
    {
        if (Physics.Raycast(_debugPos, Vector3.down, out var raycastHit, 100, _layerMask))
        {
            Debug.Log("Where goal");
            Instantiate(_goalPrefab, raycastHit.point, transform.rotation);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_debugPos, 1);
    }
}
