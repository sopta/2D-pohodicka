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
            
            var controller = new ObjectField("Controller");
            controller.objectType = typeof(AnimatorController);

            var nameTextField = new TextField();
            nameTextField.label = "Name";

            var isFaceToggle = new Toggle();
            isFaceToggle.label = "Is Face";
            
            var btn = new Button();
            btn.text = "Test";
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
                    string stateName = nameTextField.value + "_" + clips[i].displayName;
                    if (FindState(rootStateMachine.states, stateName)) continue;
                    
                    var first = rootStateMachine.AddState(stateName);
                    var firstBack = first.AddTransition(dialogueIdleState);
                    firstBack.hasExitTime = true;

                    var firstTrans = dialogueIdleState.AddTransition(first);
                    firstTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, i+1, isFaceToggle.value ? "DialogueFacePart" : "DialoguePart");
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