using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Localization;
using XNode;

public enum PublicKeyList
{
	None,
	TestName,
	NextTestName,
}

public enum ActorID
{
	None,
	Grandpa,
	Ben,
}

[NodeWidth(300)]
public class DialogueActorNode : Node  // todo DialogueActorNode -> DialogueNode
{
	public static string PrevInput = "Prev";
	public static string NextOutput = "Next";
	
	[Input(ShowBackingValue.Never, ConnectionType.Override)] public int Prev;
	[Output] public int Next;
	[HideInInspector] public string ID;
	public PublicKeyList PublicKey; // todo @refactoring mit moznost deklarovat enum separatne
	public ActorID ActorID;
	public DialogEntryValue CZ;
	public DialogEntryValue EN;

	public override object GetValue(NodePort port)
	{
		return null;
	}
	
	public string GetSentence(LocaleIdentifier locale)  // todo dat do DialogueActor->GetSentence(), poresi jmeno
	{
		switch (locale.Code)
		{
			case "cs":
				return CZ.Value;
			case "en":
				return EN.Value;
		}

		return null;
	}
}