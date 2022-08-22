using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using System.Linq;
using Live2D.Cubism.Rendering;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using UnityEditor;

public class Live2dTest : MonoBehaviour
{
    public CubismModel _model;
    //public AnimatorController animator;
    public Animator animator;
    public List<AnimationClip> animationClips;
    // Start is called before the first frame update
    void Start()
    {
        _model = this.FindCubismModel();
    }

    // Update is called once per frame
    void LateUpdate() => _model.ForceUpdateNow();
    [Button("���")]
    void Togray() => _model.Drawables.ToList().ForEach(drawable => drawable.GetComponent<CubismRenderer>().Color = Color.gray);
    [Button("���")]
    void ToWhite() => _model.Drawables.ToList().ForEach(drawable => drawable.GetComponent<CubismRenderer>().Color = Color.white);
    [Button("��ʼ������Ƭ��")]
    void Init()
    {
        animator = GetComponent<Animator>();
        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        animatorController.parameters = new AnimatorControllerParameter[0];
        animationClips.ForEach(clip => animatorController.AddParameter(clip.name, AnimatorControllerParameterType.Trigger));
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        stateMachine.states = new ChildAnimatorState[0];
        AnimatorState state = stateMachine.AddState("idle");
        state.motion = animationClips[0];
        stateMachine.AddEntryTransition(state);
        animationClips.ForEach(clip =>
        {
            AnimatorState state = stateMachine.AddState(clip.name);
            state.motion = clip;
            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
            transition.AddCondition(AnimatorConditionMode.If, 1, clip.name);
        });
        AssetDatabase.SaveAssets();
    }
    [Button("����")]
    void Play(string tag)
    {
        GetComponent<Animator>().SetTrigger(tag);
    }
}
