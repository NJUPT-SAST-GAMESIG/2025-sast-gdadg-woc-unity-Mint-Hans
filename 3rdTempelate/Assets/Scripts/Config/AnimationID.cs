using UnityEngine;

namespace Config
{
    public class AnimationID
    {
        public static readonly int Movement = Animator.StringToHash("Movement");
        public static readonly int Lock = Animator.StringToHash("Lock");
        public static readonly int Horizontal = Animator.StringToHash("Horizontal");
        public static readonly int Vertical = Animator.StringToHash("Vertical");
        public static readonly int HasInput = Animator.StringToHash("HasInput");
        public static readonly int Run = Animator.StringToHash("Run");
        public static readonly int DeltaAngle = Animator.StringToHash("DeltaAngle");
        public static readonly int CanMove = Animator.StringToHash("CanMove");
        public static readonly int Die = Animator.StringToHash("Die");
        public static readonly int Parry = Animator.StringToHash("Parry");
        public static readonly int Roll = Animator.StringToHash("Roll");
        public static readonly int LAttack = Animator.StringToHash("LAttack"); // 左键攻击触发
        public static readonly int RAttack = Animator.StringToHash("RAttack"); // 右键攻击触发
        public static readonly int ComboStep = Animator.StringToHash("ComboStep"); // 连击段数
        public static readonly int IsAttacking = Animator.StringToHash("IsAttacking"); // 是否在攻击中 
    }
}
