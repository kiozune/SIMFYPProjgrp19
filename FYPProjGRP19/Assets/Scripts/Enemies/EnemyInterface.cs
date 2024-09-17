using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EnemyInterface
{
    // Interface for common variable
    public interface IEnemy
    {
        int expGained { get; }
        float hp { get; }
        Sprite icon { get; }

        void TakeDamage(float damage);
    }
}