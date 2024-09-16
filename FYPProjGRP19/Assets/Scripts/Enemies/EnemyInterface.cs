using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyInterface
{
    // Interface for common variable
    public interface IEnemy
    {
        int expGained { get; }
        float hp { get; }

        void TakeDamage(float damage);
    }
}
