using System;
using UniRx;
using UnityEngine;

public class AvatarStateObservalbe : StateMachineBehaviour
{
    #region OnStateEnter ============================================================================================
    public IObservable<AnimatorStateInfo> OnStateEnterObservable { get { return onStateEnterSubject.AsObservable(); } }
    private Subject<AnimatorStateInfo> onStateEnterSubject = new Subject<AnimatorStateInfo>();

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateEnterSubject.OnNext(stateInfo);
    }
    #endregion



    #region OnStateExit ============================================================================================
    public IObservable<AnimatorStateInfo> OnStateExitObservable { get { return onStateExitSubject.AsObservable(); } }
    private Subject<AnimatorStateInfo> onStateExitSubject = new Subject<AnimatorStateInfo>();

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateExitSubject.OnNext(stateInfo);
    }
    #endregion



    #region OnStateMachineEnter ============================================================================================
    public IObservable<int> OnStateMachineEnterObservable { get { return onStateMachineEnterSubject.AsObservable(); } }
    private Subject<int> onStateMachineEnterSubject = new Subject<int>();

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        onStateMachineEnterSubject.OnNext(stateMachinePathHash);
    }
    #endregion



    #region OnStateMachineExit ============================================================================================
    public IObservable<int> OnStateMachineExitObservable { get { return onStateMachineExitrSubject.AsObservable(); } }
    private Subject<int> onStateMachineExitrSubject = new Subject<int>();

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        onStateMachineExitrSubject.OnNext(stateMachinePathHash);
    }
    #endregion



    #region OnStateMove ============================================================================================
    public IObservable<AnimatorStateInfo> OnStateMoveObservable { get { return onStateMoveSubject.AsObservable(); } }
    private Subject<AnimatorStateInfo> onStateMoveSubject = new Subject<AnimatorStateInfo>();

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateMoveSubject.OnNext(stateInfo);
    }
    #endregion



    #region OnStateMove ============================================================================================
    public IObservable<AnimatorStateInfo> OnStateUpdateObservable { get { return onStateUpdateSubject.AsObservable(); } }
    private Subject<AnimatorStateInfo> onStateUpdateSubject = new Subject<AnimatorStateInfo>();

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateUpdateSubject.OnNext(stateInfo);
    }
    #endregion



    #region OnStateIK ============================================================================================
    public IObservable<AnimatorStateInfo> OnStateIKObservable { get { return onStateIKSubject.AsObservable(); } }
    private Subject<AnimatorStateInfo> onStateIKSubject = new Subject<AnimatorStateInfo>();

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateIKSubject.OnNext(stateInfo);
    } 
    #endregion
}
