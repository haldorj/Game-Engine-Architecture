using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.State
{
    // handles
    [Serializable]
    public class StateMachine
    {
        public IState CurrentState { get; private set; }

        public GameObject light;
        
        // reference to the state objects
        public RedState redState;
        public YellowState yellowState;
        public GreenState greenState;

        // event to notify other objects of the state change
        public event Action<IState> StateChanged;

        // pass in necessary parameters into constructor 
        public StateMachine(GameObject light)
        {
            // create an instance for each state and pass in Light
            this.redState = new RedState(light);
            this.yellowState = new YellowState(light);
            this.greenState = new GreenState(light);
        }

        // set the starting state
        public void Initialize(IState state)
        {
            CurrentState = state;
            state.Enter();

            // notify other objects that state has changed
            StateChanged?.Invoke(state);
        }

        // exit this state and enter another
        public void TransitionTo(IState nextState)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            nextState.Enter();

            // notify other objects that state has changed
            StateChanged?.Invoke(nextState);
        }

        // allow the StateMachine to update this state
        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }
    }
}