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
        
        private void OnEnable()
        {
            _Root = new VisualElement();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var box = new Foldout();
            box.text = "Default inspector";
            
            InspectorElement.FillDefaultInspector(box, serializedObject, this);

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
                    string stateName = clips[i].displayName;
                    if (FindState(rootStateMachine.states, stateName)) continue;
                    
                    var state = rootStateMachine.AddState(stateName);
                    state.motion = clips[i].animationClip;
                    
                    var transitionToIdle = state.AddTransition(dialogueIdleState);
                    transitionToIdle.hasExitTime = true;

                    var transitionToClip = dialogueIdleState.AddTransition(state);
                    transitionToClip.AddCondition(AnimatorConditionMode.Equals, i+1, isFaceToggle.value ? "DialogueFacePart" : "DialoguePart");
                }
            };
            
            _Root.Add(box);
            _Root.Add(boxAnimator);
            boxAnimator.Add(controller);
            boxAnimator.Add(nameTextField);
            boxAnimator.Add(isFaceToggle);
            boxAnimator.Add(btn);
            
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