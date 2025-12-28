using System;
using System.Collections.Generic;
using Character.Base;

namespace Tools
{
    public interface IStateMachineOwner//状态机宿主接口
    {
    }

    public class StateMachine
    {
       private StateBase _currentState;//当前状态
       private IStateMachineOwner _owner;//状态宿主
       private Dictionary<Type,StateBase> _stateDic = new Dictionary<Type,StateBase>();
       
        /// <summary>
        /// 进入动画状态
        /// </summary>
        /// <typeparam name="T">状态实例</typeparam>
       public void EnterState<T>() where T : StateBase, new()
       {
           if (_currentState.GetType() == typeof(T)) return;
           
           if (_currentState != null) 
               _currentState.Exit();
           _currentState = GetState<T>();
           _currentState.Enter();
       }
        /// <summary>
        /// 从字典中取出状态类
        /// </summary>
        /// <typeparam name="T">状态类</typeparam>
        /// <returns>状态实例</returns>
        private StateBase GetState<T>() where T : StateBase, new()
        {
            Type stateType = typeof(T);//获取状态类型
            if (!_stateDic.TryGetValue(stateType, out StateBase state))
            {
                state = new T();
                state.Init(_owner);
                _stateDic.Add(stateType, state);
            }
            return state;
        }

        public void Stop()
        {
            if (_currentState != null)
                _currentState.Exit();
            foreach(var state in _stateDic.Values)
                state.Destroy();
            _stateDic.Clear();
        }
    }
}