using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T> : IState<T>
{
    protected AIManagerFSM<T> _fsm;
    private Dictionary<T, IState<T>> _transitions = new Dictionary<T, IState<T>>();

    public Dictionary<T, IState<T>> Transition => _transitions;

    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void LateExecute() { }
    public virtual void Sleep() { }

    public void AddTransition(T input, IState<T> state) 
    {
        if (this == state) return;
        _transitions.Add(input, state);
    }

    public void AddTransition(Dictionary<T, IState<T>> transitions)
    {
        _transitions = transitions;
    }

    public void RemoveTransition(T input)
    {
        if (_transitions.ContainsKey(input))
            _transitions.Remove(input);
    }

    public void RemoveTransition(IState<T> input)
    {
        foreach (var item in _transitions)
        {
            T key = item.Key;
            IState<T> value = item.Value;

            if (value == input)
            {
                _transitions.Remove(key);
                break;
            }
        }
    }

    public IState<T> GetTransition(T input)
    {
        if (_transitions.ContainsKey(input))
            return _transitions[input];

        return null;
    }

    public AIManagerFSM<T> SetFiniteStateMachine { set { _fsm = value; } }
}
