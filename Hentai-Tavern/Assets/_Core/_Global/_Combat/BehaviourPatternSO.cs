using System.Collections.Generic;
using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "BehaviourPattern", menuName = "Combat/Behaviour Pattern")]
    public class BehaviourPatternSO : ScriptableObject
    {
        public List<AbilitySO> Abilities = new List<AbilitySO>();
    }
}