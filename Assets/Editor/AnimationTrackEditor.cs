using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace Platformer
{
    [CustomEditor(typeof(AnimationTrack))]
    public class AnimationTrackEditor : Editor
    {
        private VisualElement _Root;
        private VisualTreeAsset _Template;
        
        private void OnEnable()
        {
            _Root = new VisualElement();
            _Template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/zDialogos/Editor/text.uxml");
        }

        public override VisualElement CreateInspectorGUI()
        {
            var box = new Foldout();
            box.text = "Default inspector";
            
            InspectorElement.FillDefaultInspector(box, serializedObject, this);
            
            //_Root.Add(_Template.Instantiate());
            //_Root.Add(_Template.Instantiate());
            //_Root.Add(_Template.Instantiate());

            var boxAnimator = new Box();
            boxAnimator.style.marginTop = 30;
            boxAnimator.style.paddingTop = 5;
            boxAnimator.style.paddingBottom = 5;
            boxAnimator.style.paddingLeft = 5;
            boxAnimator.style.paddingRight = 5;

            var boxLabel = new Label();
            boxLabel.text = "Animator";
            boxLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            boxLabel.style.marginBottom = 15;
            boxAnimator.Add(boxLabel);
            
            var controller = new ObjectField("Controller");
            controller.objectType = typeof(AnimatorController);

            var nameTextField = new TextField();
            nameTextField.label = "Name";

            var isFaceToggle = new Toggle();
            isFaceToggle.label = "Is Face";
            
            var btn = new Button();
            btn.text = "Create";
            btn.clickable.clicked += () =>
            {
                var controllerValue = controller.value as AnimatorController;
                var rootStateMachine = controllerValue.layers[isFaceToggle.value ? 1 : 0].stateMachine;

                var dialogueIdleState = FindState(rootStateMachine.states, nameTextField.value + "_idle");

                if (dialogueIdleState == null)
                {
                    var parameter = nameTextField.value + (isFaceToggle.value ? "Face" : "");
                    controllerValue.AddParameter(parameter, AnimatorControllerParameterType.Trigger);
                
                    dialogueIdleState = rootStateMachine.AddState(nameTextField.value + "_idle");

                    var transition = rootStateMachine.defaultState.AddTransition(dialogueIdleState);
                    transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, parameter);
                }

                var clips = (target as AnimationTrack).GetClips().ToList();
                
                for (int i = 0; i < clips.Count; i++)
                {
                    //string stateName = nameTextField.value + "_" + clips[i].displayName;
                    //string stateName = clips[i].displayName;
                    string stateName = clips[i].animationClip.name;
                    if (FindState(rootStateMachine.states, stateName)) continue;
                    
                    var state = rootStateMachine.AddState(stateName);
                    state.motion = clips[i].animationClip;
                    
                    var transitionToIdle = state.AddTransition(dialogueIdleState);
                    transitionToIdle.hasExitTime = true;

                    var transitionToClip = dialogueIdleState.AddTransition(state);
                    transitionToClip.AddCondition(AnimatorConditionMode.Equals, i+1, isFaceToggle.value ? "DialogueFacePart" : "DialoguePart");
                }
            };

            var btnCreateTrack = new Button();
            btnCreateTrack.text = "Create new track";
            btnCreateTrack.clickable.clicked += () =>
            {
                var track = target as AnimationTrack;
                AnimationTrack newTrack = track.timelineAsset.CreateTrack<AnimationTrack>();
                newTrack.name = nameTextField.value != String.Empty ? nameTextField.value : "Animation Track";
                newTrack.name = isFaceToggle.value ? newTrack.name + ".Face" : newTrack.name;
                
                var clips = (target as AnimationTrack).GetClips().ToList();

                //TimelineUndo.RegisterCreatedObjectUndo(playableAsset, "Create Clip");
                //TimelineUndo.PushUndo(this, "Create Clip");
                
                for (int i = 0; i < clips.Count; i++)
                {
                    var newClip = newTrack.CreateClip<AnimationPlayableAsset>();
                    //newClip.asset = clips[i].animationClip; // todo fix
                    //newClip.asset = clips[i].asset; // todo fix
                    newClip.start = clips[i].start;
                    newClip.duration = 4f; // todo vzit z clipu - 3f treba
                }
            };
            
            _Root.Add(box);
            _Root.Add(boxAnimator);
            boxAnimator.Add(controller);
            boxAnimator.Add(nameTextField);
            boxAnimator.Add(isFaceToggle);
            boxAnimator.Add(btn);
            boxAnimator.Add(btnCreateTrack);
            
            return _Root;
        }

        private AnimatorState FindState(ChildAnimatorState[] states, string name)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state.name == name) return states[i].state;
            }

            return null;
        }
    }
}