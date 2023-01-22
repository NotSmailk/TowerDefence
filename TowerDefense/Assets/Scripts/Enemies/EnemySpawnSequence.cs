using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnSequence
{
    [field: SerializeField] private EnemyFactory _factory;
    [field: SerializeField] private EnemyType _type;
    [field: SerializeField, Range(1, 100)] private int _amount = 1;
    [field: SerializeField, Range(1f, 100f)] private float _cooldown = 1f;

    public State Begin() => new State(this);

    [Serializable]
    public struct State
    {
        private EnemySpawnSequence _sequence;
        private int _count;
        private float _cooldown;

        public State(EnemySpawnSequence sequence)
        {
            _sequence = sequence;
            _count = 0;
            _cooldown = sequence._cooldown;
        }

        public float Progress(float deltaTime)
        {
            _cooldown += deltaTime;
            while(_cooldown >= _sequence._cooldown)
            {
                _cooldown -= _sequence._cooldown;
                if (_count >= _sequence._amount)
                {
                    return _cooldown;
                }

                _count++;
                QuickGame.SpawnEnemy(_sequence._factory, _sequence._type);
            }

            return -1f;
        }
    }
}
