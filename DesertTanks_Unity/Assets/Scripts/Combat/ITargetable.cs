using UnityEngine;

namespace RTSTutorial
{
    public interface ITargetable
    {
        Transform AimAtPoint { get; }

        GameObject gameObject { get; }

        bool IsFriendly { get; }
    }
}