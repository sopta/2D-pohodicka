using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace DefaultNamespace.zDialogos
{
    // Runtime obalka
    // DialogueRuntimeContext
    // Jakej node se ted prehrava, external signals kdy se ma prehrat dalsi
    public class Dialogue
    {
        public DialogueNodeGraph Graph;
        public DialogueActorNode CurrentNode;

        private int NextIndex;

        public void Play()
        {
            CurrentNode = Graph.StartAt;
            NextIndex = 0;
        }

        public DialogueActorNode Continue()
        {
            if (NextIndex == 0)
            {
                NextIndex++;
                return CurrentNode;
            }

            CurrentNode = CurrentNode.GetOutputPort(DialogueActorNode.NextOutput)?.Connection?.node as DialogueActorNode;
            if (CurrentNode != null) NextIndex++;
            return CurrentNode;
        }
    }
    
    public class DialogueManager : MonoBehaviour
    {
        public Text _Text;
        //private List<Dialogue> _Dialogues = new(); // todo dictionary, zas podle adresy

        private LocaleIdentifier _LocaleIdentifier;

        public void Init()
        {
            _LocaleIdentifier = LocalizationSettings.ProjectLocale.Identifier;
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettingsOnSelectedLocaleChanged;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= LocalizationSettingsOnSelectedLocaleChanged;
        }

        private void LocalizationSettingsOnSelectedLocaleChanged(Locale locale)
        {
            _LocaleIdentifier = locale.Identifier;
        }

        public void Play(string address)
        {
            var op = Addressables.LoadAssetAsync<DialogueNodeGraph>(address).WaitForCompletion();
            
            StartCoroutine(PlayInLoop(new Dialogue
            {
                Graph = op
            }));
        }
        
        public void Play(AssetReferenceT<DialogueNodeGraph> assetReferenceT)
        {
            var op = Addressables.LoadAssetAsync<DialogueNodeGraph>(assetReferenceT.RuntimeKey).WaitForCompletion();
            
            StartCoroutine(PlayInLoop(new Dialogue
            {
                Graph = op
            }));
        }

        private IEnumerator PlayInLoop(Dialogue dialogue)
        {
            dialogue.Play();
            
            while (true)
            {
                var actor = dialogue.Continue();
                if (actor == null)
                {
                    _Text.text = "";
                    _Text.transform.parent.gameObject.SetActive(false);
                    yield break;
                }

                _Text.text = actor.GetSentence(_LocaleIdentifier);
                _Text.transform.parent.gameObject.SetActive(true);
                
                yield return new WaitForSeconds(2f); // todo audio length + 0.2f
            }
        }
    }
}