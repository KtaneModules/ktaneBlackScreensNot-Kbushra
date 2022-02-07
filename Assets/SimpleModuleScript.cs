using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMBombModule module;
	public KMSelectable[] BigButton;
	static int ModuleIdCounter = 1;
	int ModuleId;

	public TextMesh[] screenTexts;

	public AudioSource correct;

	public int textMessage1;
	public string textFinder1;
	public int textMessage2;
	public string textFinder2;

	bool _isSolved = false;
	bool incorrect = false;

	public int inputAns = 0;
	public int ans = 0;

	void Awake() 
	{
		ModuleId = ModuleIdCounter++;

		foreach (KMSelectable button in BigButton)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { BigButtonPresser(pressedButton); return false; };
		}
	}

	void Start ()
	{
		textMessage1 = Rnd.Range(1, 5);
		for (int i = 0; i < textMessage1; i++)
			textFinder1 = textMessage1.ToString();
		screenTexts[0].text = textFinder1;

		textMessage2 = Rnd.Range(1, 12);
		for (int i = 0; i < textMessage2; i++)
			textFinder2 = textMessage2.ToString();
		screenTexts[1].text = textFinder2;

		Invoke ("IntegerMesser", 0);
	}

	void Update()
	{
		ans = ans % 10;
	}

	void IntegerMesser()
	{
		if (textMessage2 == 2 || textMessage2 == 3 || textMessage2 == 5 || textMessage2 == 7 || textMessage2 == 11 || textMessage2 == 13) 
		{
			textMessage1 = textMessage1 + textMessage2;
			textMessage1 = textMessage1 * textMessage2;
			Log ("Prime sequence activated");
		}
		if (textMessage2 % 2 == 0) 
		{
			textMessage1 = textMessage1 % 2;
			textMessage1 = textMessage1 + 1;
			textMessage1 = textMessage1 * 5;
			Log ("Even sequence activated");
		}
		if (textMessage2 % 2 == 1) 
		{
			textMessage1 = textMessage1 * 9;
			textMessage1 = textMessage1 + 3;
			Log ("Odd sequence activated");
		}
		if (textMessage2 == textMessage1) 
		{
			textMessage1 = textMessage1 + 1;
			Log ("Equal sequence activated");
		}

		ans = textMessage1;

		if (info.IsIndicatorOn (Indicator.BOB)) 
		{
			inputAns = textMessage1;
			Log ("Bob is sorry for you");
		}
	}

	void BigButtonPresser(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < BigButton.Length; i++)
		{
			if (pressedButton == BigButton[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (_isSolved == false) 
		{
			switch (buttonPosition) 
			{
			case 0:
				if (inputAns == ans) 
				{
					module.HandlePass ();
					_isSolved = true;
				} 
				else
				{
					incorrect = true;
					Log ("Wrong submission!");
					inputAns = 0;
				}
				break;
			case 1:
				correct.Play ();
				Log ("Input remembered");
				inputAns++;
				break;
			}
			if (incorrect) 
			{
				module.HandleStrike ();
				incorrect = false;
			}
		}
	}

	void Log(string message)
	{
		Debug.LogFormat("[Not Black Screens #{0}] {1}", ModuleId, message);
	}
}
