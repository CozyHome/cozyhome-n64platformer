using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoFSM<T1, T2> where T2 : MonoFSM<T1, T2>.IMonoState
{
    private Dictionary<T1, T2> StateRegistry;
    private T2 _Current;

    public MonoFSM()
    {
        StateRegistry = new Dictionary<T1, T2>();
    }

    public void SetState(T1 key) => _Current = StateRegistry[key];
    public void SetState(T2 value) => _Current = value;

    public void SwitchState(T1 nextkey)
    {
        // notify exit
        // notify enter
        // swap
        T2 next = StateRegistry[nextkey];
        _Current.Exit(next);
        next.Enter(_Current);
        _Current = next;
    }

    public void SwitchState(Action<T2> prepare, T1 nextkey)
    {
        T2 next = StateRegistry[nextkey];
        prepare.Invoke(next);
        // notify exit
        // notify enter
        // swap
        _Current.Exit(next);
        next.Enter(_Current);
        _Current = next;
    }

    public void SwitchState(Func<T2, bool> query, Action<T2> prepare, T1 nextkey)
    {
        T2 next = StateRegistry[nextkey];
        
        if(!query.Invoke(next))
            return;

        prepare.Invoke(next);
        // notify exit
        // notify enter
        // swap
        _Current.Exit(next);
        next.Enter(_Current);
        _Current = next;
    }

    public bool TrySwitchState(Func<T2, bool> query, T1 nextkey)
    {
        T2 next = StateRegistry[nextkey];
        
        if(!query.Invoke(next))
            return false;

        // notify exit
        // notify enter
        // swap
        _Current.Exit(next);
        next.Enter(_Current);
        _Current = next;

        return true;
    }

    public void AddState(T1 key, T2 value) => StateRegistry.Add(key, value);
    public void RemoveState(T1 key) => StateRegistry.Remove(key);

    public T2 Current => _Current;

    public interface IMonoState
    {
        void Enter(T2 prev);
        void Exit(T2 next);
    }
}