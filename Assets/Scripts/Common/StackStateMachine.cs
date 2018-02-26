using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackStateMachine<T>
{
    Dictionary<string, State<T>> m_states = new Dictionary<string, State<T>>();

    public Stack<State<T>> m_stateStack = new Stack<State<T>>();
    State<T> m_state = null;
    
    public void Update()
    {
        m_state = m_stateStack.Peek();

        if (m_state != null)
        {
            m_state.Update();
        }
    }

    public void AddState(string stateID, State<T> state)
    {
        m_states[stateID] = state;
    }

    public void PushState(string stateID)
    {
        if (m_states.ContainsKey(stateID))
        {
            State<T> state = m_states[stateID];
            m_stateStack.Push(state);
            state.Enter();
        }
    }

    public void PopState()
    {
        m_state.Exit();
        m_stateStack.Pop();
    }

    public void SetState(string stateID)
    {
        if (m_states.ContainsKey(stateID))
        {
            State<T> state = m_states[stateID];
            if (state != m_state)
            {
                if (m_state != null)
                {
                    m_state.Exit();
                }

                m_state = state;
                m_state.Enter();
                m_state.Update();
            }
        }
    }
}
