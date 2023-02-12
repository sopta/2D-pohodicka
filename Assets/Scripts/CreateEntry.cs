using System;
using System.Collections.Generic;
using DefaultNamespace.zDialogos;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    [Serializable]
    public class DialogueEntry
    {
        public string Key;
        public List<DialogEntryValue> Values;
    }

    [Serializable]
    public class DialogEntryValue
    {
        public LocaleIdentifier Locale;
        [TextArea(minLines: 10, maxLines: 10)] public string Value;
    }
    public class CreateEntry : MonoBehaviour
    {
        public LocalizedStringTable _Table;
        public StringTable _StringTable;
        public StringTableCollection _Collection;
        public LocaleIdentifier _Locale;
        public List<DialogueEntry> _Entries;

        [FormerlySerializedAs("_TestGraph")] public DialogueNodeGraph dialogueNodeGraph;

        private Dictionary<string, StringTable> Tables = new ();

        private void Start()
        {
            foreach (var table in _Collection.StringTables)
            {
                Tables.Add(table.LocaleIdentifier.Code, table);
            }
            
            /*var tableEntry = _Table.GetTable().CreateTableEntry();
            tableEntry.Value = "ahoj";
            tableEntry.IsSmart = false;
            
    
            */
            //_Table.TableReference.TableCollectionName
            
            /*var tableEntry = _Table.GetTable().AddEntry("test_key", "ahoj");
            tableEntry.IsSmart = false;*/

            //_StringTable.AddEntry("test_key", "ahoj");
            //_StringTable.LocaleIdentifier = _Locale;

            //_Collection.GetTable(LocalizationSettings.SelectedLocale.Identifier).
            /*
            foreach (var entry in _Entries)
            {
                foreach (var localEntry in entry.Values)
                {
                    foreach (var table in _Collection.StringTables)
                    {
                        if (table.LocaleIdentifier.Code == localEntry.Locale.Code)
                        {
                            var aa = table.AddEntry(entry.Key, localEntry.Value);
                            aa.IsSmart = false;
                        }
                    }
                }
            }
            */
            
            GetComponent<DialogueManager>().Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                GetComponent<DialogueManager>().Play("Novej");
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                FileReader.ReadFile("Assets/test.txt", dialogueNodeGraph);
            }
        }
        
        private void LocalizationSettingsOnSelectedLocaleChanged(Locale obj)
        {
            Debug.Log(_Table.GetTable().GetEntry("aaa_001").GetLocalizedString());
        }
    }
}