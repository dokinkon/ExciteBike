using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// FSMSystem class represents the Finite State Machine class.
///  It has a List with the States the NPC has and methods to add,
///  delete a state, and to change the current state the Machine is on.
/// </summary>
public class FSMSystem
{
    private List<FSMState> _states;
 
    // The only way one can change the state of the FSM is by performing a transition
    // Don't change the CurrentState directly
    private string _currentStateName;
    public string CurrentStateName { get { return _currentStateName; } }
    private FSMState _currentState;
    public FSMState CurrentState { get { return _currentState; } }
	private bool _isStarted = false; 
    public FSMSystem()
    {
        _states = new List<FSMState>();
    }
	
	public void Start() {
		if (_isStarted)
			return;
		if (_currentState!=null) {
			_currentState.DoBeforeEntering();
		}
		_isStarted = true;
	}
	
	public void Stop() {
		if (!_isStarted) 
			return;
		
		if (_currentState!=null) {
			_currentState.DoBeforeLeaving();
		}
	}
 
    /// <summary>
    /// This method places new states inside the FSM,
    /// or prints an ERROR message if the state was already inside the List.
    /// First state added is also the initial state.
    /// </summary>
    public void AddState(FSMState s)
    {
        // Check for Null reference before deleting
        if (s == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }
 
        // First State inserted is also the Initial state,
        //   the state the machine is in when the simulation begins
        if (_states.Count == 0)
        {
            _states.Add(s);
            _currentState = s;
            _currentStateName = s.Name;
            return;
        }
 
        // Add the state to the List if it's not inside it
        foreach (FSMState state in _states)
        {
            if (state.Name == s.Name)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + s.Name + 
                               " because state has already been added");
                return;
            }
        }
        _states.Add(s);
    }
 
    /// <summary>
    /// This method delete a state from the FSM List if it exists, 
    ///   or prints an ERROR message if the state was not on the List.
    /// </summary>
    public void DeleteState(string stateName)
    {
        // Check for NullState before deleting
        if (stateName == null)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
            return;
        }
 
        // Search the List and delete the state if it's inside it
        foreach (FSMState state in _states)
        {
            if (state.Name == stateName)
            {
                _states.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: Impossible to delete state " + stateName + 
                       ". It was not on the list of states");
    }
	
	public void Update() 
	{
		if (!_isStarted)
			return;
		
		if (_currentState!=null) {
			_currentState.Update();
		}
	}
 
    /// <summary>
    /// This method tries to change the state the FSM is in based on
    /// the current state and the transition passed. If current state
    ///  doesn't have a target state for the transition passed, 
    /// an ERROR message is printed.
    /// </summary>
    public void PerformTransition(string transitionName)
    {
        // Check for NullTransition before changing the current state
        if (transitionName == null)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
            return;
        }
 
        // Check if the currentState has the transition passed as argument
        string stateName = _currentState.GetOutputState(transitionName);
        if (stateName == null)
        {
            Debug.LogError("FSM ERROR: State " + _currentStateName +  " does not have a target state " + 
                           " for transition " + transitionName);
            return;
        }
 
        // Update the currentStateID and currentState		
        _currentStateName = stateName;
        foreach (FSMState state in _states)
        {
            if (state.Name == _currentStateName)
            {
                // Do the post processing of the state before setting the new one
                _currentState.DoBeforeLeaving();
                _currentState = state;
                // Reset the state to its desired condition before it can reason or act
                _currentState.DoBeforeEntering();
                break;
            }
        }
 
    } // PerformTransition()
 
} //class FSMSystem