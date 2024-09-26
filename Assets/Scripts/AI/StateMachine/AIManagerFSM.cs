using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManagerFSM<T>
{
    IState<T> _currentState;
    public IState<T> CurrentState => _currentState;

    public AIManagerFSM() { }

    public AIManagerFSM(IState<T> initialState)
    {
        _currentState = initialState;
        _currentState.SetFiniteStateMachine = this;
        _currentState.Enter();
    }

    public void SetInit(IState<T> initState)
    {
        _currentState = initState;
        _currentState.SetFiniteStateMachine = this;
        _currentState.Enter();
    }

    public void OnUpdate()
    {
        if (_currentState != null) _currentState.Execute();
    }

    public void OnLateUpdate()
    {
        if (_currentState != null) _currentState.LateExecute();
    }

    public void Transition(T input)
    {
        IState<T> newState = _currentState.GetTransition(input);

        if (newState != null)
        {
            _currentState.Sleep();
            _currentState = newState;
            _currentState.SetFiniteStateMachine = this;
            _currentState.Enter();
        }
    }
}
