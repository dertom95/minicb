
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

/// <summary>
/// Statemachine
/// Need to set a default state to which the state jumps back if an updatable state exits
/// </summary>
/// <typeparam name="TStateEnum"></typeparam>
/// <typeparam name="TContext"></typeparam>
public class StateMachine<TStateEnum,TContext> {
    
    private Dictionary<TStateEnum, IState<TContext>> stateRegistry;
    private IState<TContext> currentStateLogic;
    private TStateEnum currentStateEnum;
    private TStateEnum defaultState;
    private object defaultUserData;

    public TStateEnum CurrentState => currentStateEnum;

    private TContext context;

    public StateMachine(TContext initialContext, TStateEnum defaultState, object defaultUserData=null) {
        this.context = initialContext;
        stateRegistry = new Dictionary<TStateEnum, IState<TContext>>();
    }

    public void SetDefaultState(TStateEnum defaultState, object defaultStateUserData=null) {
        this.defaultState = defaultState;
        this.defaultUserData = defaultStateUserData;
    }

    public void RegisterState(TStateEnum stateEnum, IState<TContext> stateLogic) {
        Assert.IsFalse(stateRegistry.ContainsKey(stateEnum));

        stateRegistry[stateEnum] = stateLogic;
    }

    public void ChangeState(TStateEnum newState, object userObject=null) {
        Assert.IsTrue(stateRegistry.ContainsKey(newState));

        currentStateLogic?.OnStateExit();
        currentStateEnum = newState;
        currentStateLogic = stateRegistry[newState];
        currentStateLogic.OnStateEnter(context, userObject);
    }

    public void Update() {
        if (currentStateLogic != null && currentStateLogic is IUpdatableState<TContext> updatable) {
            bool keepRunning = updatable.OnUpdate(ref context);
            if (!keepRunning) {
                ChangeState(defaultState,defaultUserData);
            }
        }
    }

    public ref TContext GetContext() => ref context;
}

public interface IState<TContext> {
    void OnStateEnter(TContext ctx, object userObject=null);
    void OnStateExit();
}

public interface IUpdatableState<TContext> {
    /// <summary>
    /// Update state, return true to keep running, false to exit the state
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    bool OnUpdate(ref TContext ctx);
}
