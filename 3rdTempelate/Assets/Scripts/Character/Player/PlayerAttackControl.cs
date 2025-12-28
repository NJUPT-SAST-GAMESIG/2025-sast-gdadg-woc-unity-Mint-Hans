using Character.Base;
using Config;
using Input;
using UnityEngine;

namespace Character.Player
{
    // 攻击类型枚举
    public enum AttackType
    {
        None,
        Light, // 左键
        Heavy  // 右键
    }

    [RequireComponent(typeof(PlayerMovementControl))]
    public class PlayerAttackControl : CharacterMovementControlBase
    {
        [Header("左键攻击配置 (LAttack)")]
        [SerializeField] private int maxLightCombo = 4; // 左键最大连击数
        
        [Header("右键攻击配置 (RAttack)")]
        [SerializeField] private int maxHeavyCombo = 3; // 右键最大连击数

        [Header("通用配置")]
        [SerializeField] private float comboResetTime = 1.2f; // 停止输入多久后重置连击
        
        // 运行时状态
        private int _currentComboStep = 0;
        private AttackType _currentAttackType = AttackType.None;
        private float _lastAttackInputTime;
        private bool _canNextCombo = true; // 是否允许输入下一段

        // 引用
        private Transform _mainCamera;
        // 如果需要控制移动脚本的锁定状态
        // private PlayerMovementControl _movementControl; 

        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main.transform;
            // _movementControl = GetComponent<PlayerMovementControl>();
        }

        protected override void Update()
        {
            base.Update();
            UpdateTimers();
            HandleInput();
        }

        private void UpdateTimers()
        {
            // 连击超时重置：如果很久没按键，且当前没在播放攻击动画，重置段数
            if (Time.time - _lastAttackInputTime > comboResetTime && !Animator.GetBool(AnimationID.IsAttacking))
            {
                ResetComboState();
            }
        }

        private void HandleInput()
        {
            // 检测攻击输入
            // 假设 GameInputManager.MainInstance.LAttack / RAttack 是 bool 或 trigger
            // 注意：这里建议 InputManager 的 LAttack/RAttack 是 "Down" 状态 (只触发一帧)，而不是 "Hold"
            
            if (GameInputManager.MainInstance.LAttack) 
            {
                PerformAttack(AttackType.Light);
            }
            else if (GameInputManager.MainInstance.RAttack)
            {
                PerformAttack(AttackType.Heavy);
            }
        }

        private void PerformAttack(AttackType inputType)
        {
            // 如果不允许下一段 (还在前摇中)，直接返回
            if (!_canNextCombo) return;

            // 如果切换了攻击方式 (比如从左键切到右键)，重置连击段数，从第1段开始
            if (_currentAttackType != AttackType.None && _currentAttackType != inputType)
            {
                _currentComboStep = 0;
            }

            // 更新当前攻击类型
            _currentAttackType = inputType;
            int maxCombo = (inputType == AttackType.Light) ? maxLightCombo : maxHeavyCombo;

            // 增加连击段数
            _currentComboStep++;
            if (_currentComboStep > maxCombo)
            {
                _currentComboStep = 1; // 循环或重置
            }

            // 记录时间
            _lastAttackInputTime = Time.time;
            
            // 转向
            RotateToCameraDirection();

            // 设置 Animator 参数
            Animator.SetInteger(AnimationID.ComboStep, _currentComboStep);
            Animator.SetBool(AnimationID.IsAttacking, true);

            // 根据类型触发不同的 Trigger
            if (inputType == AttackType.Light)
            {
                Animator.SetTrigger(AnimationID.LAttack);
                // 确保重置另一个Trigger，防止并在
                Animator.ResetTrigger(AnimationID.RAttack);
            }
            else
            {
                Animator.SetTrigger(AnimationID.RAttack);
                Animator.ResetTrigger(AnimationID.LAttack);
            }

            // 锁死输入，等待动画事件解锁
            _canNextCombo = false;
        }

        private void ResetComboState()
        {
            _currentComboStep = 0;
            _currentAttackType = AttackType.None;
            Animator.SetInteger(AnimationID.ComboStep, 0);
        }

        // 辅助：攻击时朝向相机正前方或输入方向
        private void RotateToCameraDirection()
        {
            // 如果有移动输入，朝向输入方向；否则朝向相机前方
            Vector2 input = GameInputManager.MainInstance.Movement;
            Vector3 targetDir;

            if (input.sqrMagnitude > 0.01f)
            {
                 float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
                 targetDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            }
            else
            {
                targetDir = _mainCamera.forward;
            }
            
            targetDir.y = 0; // 保持水平
            if (targetDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(targetDir);
            }
        }

        #region Animation Events (必须在动画片段中配置)

        // 1. 判定窗口开启：在动画“发力后”配置
        public void OnComboCheckStart()
        {
            _canNextCombo = true;
        }

        // 2. 攻击完全结束：在动画最后一帧配置
        public void OnAttackEnd()
        {
            _canNextCombo = true;
            Animator.SetBool(AnimationID.IsAttacking, false);
            // 可以在这里强制归零，也可以依赖 Update 中的超时重置
        }
        
        // 3. 伤害判定框开启/关闭
        public void EnableWeaponCollider() { /* ... */ }
        public void DisableWeaponCollider() { /* ... */ }

        #endregion
    }
}