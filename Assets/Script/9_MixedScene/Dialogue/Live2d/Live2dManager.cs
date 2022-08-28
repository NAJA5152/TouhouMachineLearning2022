using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using System.Linq;
using Live2D.Cubism.Rendering;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Live2dManager : MonoBehaviour
{
    CubismModel model;
    //public AnimatorController animator;
    public Animator animator;
    public List<AnimationClip> faces;
    public List<AnimationClip> action;

    public Vector3 defaultSightPoint;
    public Vector3 biasSightPoint;
    CubismParameter ParamAngleX;
    CubismParameter ParamAngleY;
    // Start is called before the first frame update
    void Awake()
    {
        model = this.FindCubismModel();
        ParamAngleX = model.Parameters.FindById("ParamAngleX");
        ParamAngleY = model.Parameters.FindById("ParamAngleY");
        ParamAngleX.Value = 30;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.LogError("Input.mousePosition"+Input.mousePosition);
            Debug.LogError("Input.mousePosition1"+ Camera.main.ScreenToViewportPoint(Input.mousePosition));
            Debug.LogError("transform.position" + transform.position);
            Debug.LogError("transform.position1" +Camera.main.WorldToViewportPoint(transform.position));
            Vector3 vector3= Camera.main.WorldToViewportPoint(transform.position);
            //var targetSightPoint = ((Camera.main.ScreenToViewportPoint(Input.mousePosition) * 2) - Vector3.one) * 30;
            var targetSightPoint = (Camera.main.ScreenToViewportPoint(Input.mousePosition ) - vector3 ) * 60;
            biasSightPoint = Vector3.Lerp(biasSightPoint, targetSightPoint, Time.deltaTime * 5);
        }
        else
        {
            biasSightPoint = Vector3.Lerp(biasSightPoint, defaultSightPoint, Time.deltaTime * 5);
        }
        ParamAngleX.Value = biasSightPoint.x;
        ParamAngleY.Value = biasSightPoint.y;
        model.ForceUpdateNow();
    }

    [Button("初始化动画片段")]
    public void Init()
    {
        animator = GetComponent<Animator>();
        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        animatorController.parameters = new AnimatorControllerParameter[0];
        faces.ForEach(clip => animatorController.AddParameter(clip.name, AnimatorControllerParameterType.Trigger));
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        stateMachine.states = new ChildAnimatorState[0];
        AnimatorState state = stateMachine.AddState("默认");
        state.motion = faces[0];
        stateMachine.AddEntryTransition(state);
        //导入表情动画，并为循环播放
        faces.ForEach(clip =>
        {
            AnimatorState state = stateMachine.AddState(clip.name);
            state.motion = clip;
            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
            transition.AddCondition(AnimatorConditionMode.If, 1, clip.name);
        });
        //导入动作动画，只播放一次
        action.ForEach(clip =>
        {
            AnimatorState state = stateMachine.AddState(clip.name);
            state.motion = clip;
            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
            transition.AddCondition(AnimatorConditionMode.If, 1, clip.name);
        });
        AssetDatabase.SaveAssets();
    }
    [Button("播放")]
    public void Play(string tag) => GetComponent<Animator>().SetTrigger(tag);
    [Button("变灰")]
    public void Togray() => model.Drawables.ToList().ForEach(drawable => drawable.GetComponent<CubismRenderer>().Color = Color.gray);
    [Button("变白")]
    public void ToWhite()
    {
        model.Drawables.ToList().ForEach(drawable => drawable.GetComponent<CubismRenderer>().Color = Color.white);
    }
    public void ToActive(float process)
    {
        float value = (process * 0.8f) + 0.2f;
        model.Drawables.ToList().ForEach(drawable => drawable.GetComponent<CubismRenderer>().Color = new Color(value, value, value));
    }
}
