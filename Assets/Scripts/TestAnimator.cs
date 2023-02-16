using System;
using Platformer.Mechanics;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestAnimator : MonoBehaviour
    {
        public AnimatorController _Controller;

        private void OnEnable()
        {
            Debug.Log("jebat");
        }
        
        void OnDisable()
        {
            Debug.Log("sds");
            // Creates the controller
            //var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Mecanim/My.controller");
            _Controller.AddParameter("IntroDialogueFace", AnimatorControllerParameterType.Trigger);
            _Controller.AddParameter("DialogueFacePart", AnimatorControllerParameterType.Int);
            
            var rootStateMachine = _Controller.layers[0].stateMachine;
            //rootStateMachine.defaultState // semka pridam dalsi state
            
            //var stateA1 = rootStateMachine.AddState("stateA1");
            //stateA1.motion = new AnimationClip();
            
            var dialogueIdleState = rootStateMachine.AddState("IntroDialogue_idle");

            var transition = rootStateMachine.defaultState.AddTransition(dialogueIdleState);
            transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "IntroDialogueFace");
            
            // clips
            var first = rootStateMachine.AddState("IntroDialogue_DEDA_1");
            var firstBack = first.AddTransition(dialogueIdleState);
            firstBack.hasExitTime = true;

            var firstTrans = dialogueIdleState.AddTransition(first);
            firstTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, 1, "DialogueFacePart");
            
            // list clipu
            
            // no muzu to brat z timeline.. kdyz to budu kontrolovat.. tak budu mit seznam clipu
            // nazev vemu z nazvu clipu
        }

        [MenuItem("MyMenu/Create Controller")]
        static void CreateController()
        {
            // Creates the controller
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Mecanim/StateMachineTransitions.controller");

            // Add parameters
            controller.AddParameter("TransitionNow", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Reset", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("GotoB1", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("GotoC", AnimatorControllerParameterType.Trigger);

            // Add StateMachines
            var rootStateMachine = controller.layers[0].stateMachine;
            var stateMachineA = rootStateMachine.AddStateMachine("smA");
            var stateMachineB = rootStateMachine.AddStateMachine("smB");
            var stateMachineC = stateMachineB.AddStateMachine("smC");

            // Add States
            var stateA1 = stateMachineA.AddState("stateA1");
            var stateB1 = stateMachineB.AddState("stateB1");
            var stateB2 = stateMachineB.AddState("stateB2");
            stateMachineC.AddState("stateC1");
            var stateC2 = stateMachineC.AddState("stateC2"); // don’t add an entry transition, should entry to state by default

            // Add Transitions
            var exitTransition = stateA1.AddExitTransition();
            exitTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow");
            exitTransition.duration = 0;

            var resetTransition = rootStateMachine.AddAnyStateTransition(stateA1);
            resetTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Reset");
            resetTransition.duration = 0;

            var transitionB1 = stateMachineB.AddEntryTransition(stateB1);
            transitionB1.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoB1");
            stateMachineB.AddEntryTransition(stateB2);
            stateMachineC.defaultState = stateC2;
            var exitTransitionC2 = stateC2.AddExitTransition();
            exitTransitionC2.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow");
            exitTransitionC2.duration = 0;

            var stateMachineTransition = rootStateMachine.AddStateMachineTransition(stateMachineA, stateMachineC);
            stateMachineTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoC");
            rootStateMachine.AddStateMachineTransition(stateMachineA, stateMachineB);
        }
    }
}