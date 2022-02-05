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
	public KMSelectable[] TopButtons;
	public KMSelectable[] MiddleButtons;
	public KMSelectable[] BottomButtons;
	static int ModuleIdCounter = 1;
	int ModuleId;

	public TextMesh[] screenTexts;

	public AudioSource correct;

	public int textMessage1;
	public string textFinder1;
	public int textMessage2;
	public string textFinder2;
	public int textMessage3;
	public string textFinder3;

	bool _isSolved = false;
	bool incorrect = false;

	bool KeypadUnlocker1 = false;
	bool KeypadUnlocker2 = false;
    bool KeypadUnlocker3 = false;

	public Transform mover;
	public Transform target;
	public float t;


	void Awake() 
	{
		ModuleId = ModuleIdCounter++;

		foreach (KMSelectable button in TopButtons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttonsTop(pressedButton); return false; };
		}

		foreach (KMSelectable button in MiddleButtons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttonsMiddle(pressedButton); return false; };
		}

		foreach (KMSelectable button in BottomButtons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttonsBottom(pressedButton); return false; };
		}
	}

	void Start ()
	{
		textMessage1 = Rnd.Range(1, 7);
		for (int i = 0; i < textMessage1; i++)
			textFinder1 += "a";
		screenTexts[0].text = textFinder1;

		textMessage2 = Rnd.Range(1, 3);
		for (int i = 0; i < textMessage2; i++)
			textFinder2 += "b";
		screenTexts[1].text = textFinder2;

		textMessage3 = Rnd.Range(1, 9);
		for (int i = 0; i < textMessage3; i++)
			textFinder3 += "c";
		screenTexts[2].text = textFinder3;
	}

	void buttonsTop(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < TopButtons.Length; i++)
		{
			if (pressedButton == TopButtons[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (KeypadUnlocker1 == false) 
		{
			switch (buttonPosition) 
			{
			case 0:
				if (textMessage1 < 3) 
				{
					if (info.CountUniquePorts () > 2) 
					{
						incorrect = true;
						Log ("Strike! Display is less than 3 and types of ports are more than 2.");
					}
				}
				if (textMessage1 > 2 && textMessage1 < 6) 
				{
					if (info.GetTime() > 900) 
					{
						incorrect = true;
						Log ("Strike! Display is between 3 and 5 and bomb has more than 15 mins remaining.");
					}
				}
				if (textMessage1 > 5) 
				{
					if (info.GetTime() > 960) 
					{
						incorrect = true;
						Log ("Strike! Display is 6 or over and bomb has more than 16 mins remaining.");
					}
				}
				break;
			case 1:
				if (textMessage1 < 3) 
				{
					if (info.GetStrikes () > 0) 
					{
						incorrect = true;
						Log ("Strike! Display is less than 3 and there are strikes on the bomb");
					}
				}
				if (textMessage1 > 2 && textMessage1 < 6) 
				{
					if (info.GetTime() > 600) 
					{
						incorrect = true;
						Log ("Strike! Display is between 3 and 5 and bomb has more than 10 mins remaining.");
					}
				}
				if (textMessage1 > 5) 
				{
					if (info.GetStrikes() > 1) 
					{
						incorrect = true;
						Log ("Strike! Display is 6 or over and bomb has more than 1 strike.");
					}
				}
				break;
			case 2:
				if (textMessage1 < 3) 
				{
					if (info.GetStrikes () > 0) 
					{
						Log ("Otherwise condition for strikes.");
					} 
					else 
					{
						incorrect = true;
						Log ("Not an exception.");
					}
				}
				if (textMessage1 > 2 && textMessage1 < 6) 
				{
					if (info.GetBatteryCount() > 10) 
					{
						incorrect = true;
						Log ("Strike! Waaayyyyy too many batteries.");
					}
				}
				if (textMessage1 > 5) 
				{
					if (info.GetStrikes() > 1) 
					{
						Log ("Otherwise condition for more than 1 strike.");
					}
					else 
					{
						incorrect = true;
						Log ("Not an exception.");
					}
				}
				break;
			}
			if (incorrect) 
			{
				module.HandleStrike ();
				incorrect = false;
			}
			else
			{
				correct.Play ();
				KeypadUnlocker1 = true;
			}
		}
	}

	void buttonsMiddle(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < MiddleButtons.Length; i++)
		{
			if (pressedButton == MiddleButtons[i])
			{
				buttonPosition = i;
				break;
			}
		}
		if (KeypadUnlocker1 == true && KeypadUnlocker2 == true && KeypadUnlocker3 == true) 
		{
			switch (buttonPosition) 
			{
			case 1:
				Log ("Module solved!");
				module.HandlePass ();
				break;
			}

			{
				Vector3 a = mover.position;
				Vector3 b = target.position;
				mover.position = Vector3.Lerp (a, b, t);
				Invoke ("moveAfter", 0);
			}
		}


		if (KeypadUnlocker2 == false) 
		{
			switch (buttonPosition) 
			{
			case 0:
				if (textMessage2 == 1) 
				{
					if (info.GetBatteryHolderCount () > 3) 
					{
						incorrect = true;
						Log ("Strike! Display is one and there are more than 3 battery holders.");
					}
				}
				if (textMessage2 > 1) 
				{
					if (info.GetTwoFactorCounts() > 0) 
					{
						incorrect = true;
						Log ("Strike! Display is bigger than 1 and there are two factors.");
					}
				}
				break;
			case 1:
				if (textMessage2 == 1) 
				{
					if (info.GetBatteryHolderCount () > 3) 
					{
						Log ("Exceptions.");
					} 
					else 
					{
						incorrect = true;
						Log ("Not an exception!");
					}
				}
				if (textMessage2 > 1) 
				{
					if (info.GetPortPlateCount () > 1) 
					{
						Log ("Exception.");
					}
					else 
					{
						incorrect = true;
						Log ("Not an exception!");
					}
				}
				break;
			case 2:
				if (textMessage2 > 1) 
				{
					if (info.GetPortPlateCount() > 1) 
					{
						incorrect = true;
						Log ("Strike! Display is bigger than 1 and there are 2 or more port plates.");
					}
				}
				if (textMessage2 == 1) 
				{
					if (info.GetTwoFactorCounts() == 0) 
					{
						incorrect = true;
						Log ("Strike! Display is one and there are no two factors.");
					}
				}
				break;
			}
			if (incorrect) 
			{
				module.HandleStrike ();
				incorrect = false;
			}
			else
			{
				correct.Play ();
				KeypadUnlocker2 = true;
			}
		}
	}

	void buttonsBottom(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < BottomButtons.Length; i++)
		{
			if (pressedButton == BottomButtons[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (KeypadUnlocker3 == false) 
		{
			switch (buttonPosition) 
			{
			case 0:
				if (textMessage3 > 7) 
				{
					if (info.GetTime () > 60) 
					{
						incorrect = true;
						Log ("Strike! The display is bigger than 7 and there is more than a minute on the bomb.");
					}
				}
				if (textMessage3 > 3 && textMessage3 < 8) 
				{
					if (info.GetStrikes() > 3) 
					{
						incorrect = true;
						Log ("Strike! You have an inhumane amount of strikes when there is a number between 4 and 7 on display. Be ashamed");
					}
				}
				if (textMessage3 > 0 && textMessage3 < 4) 
				{
					if (info.GetSolvedModuleIDs().Contains("blackScreens")) 
					{
						incorrect = true;
						Log ("Strike! You have completed one of these modules and have a display number between 1 and 3!");
					}
				}
				break;
			case 1:
				if (textMessage3 > 7) 
				{
					if (info.GetTime () > 1000) 
					{
						incorrect = true;
						Log ("Strike! The display is bigger than 7 and there is more than a thousand seconds left.");
					}
				}
				if (textMessage3 > 3 && textMessage3 < 8) 
				{
					if (info.GetPortCount() == 0) 
					{
						incorrect = true;
						Log ("Strike! There are no ports AND there is a number between 4 and 7 on display.");
					}
				}
				if (textMessage3 > 0 && textMessage3 < 4) 
				{
					if (info.GetModuleIDs().Contains("YahtzeeModule")) 
					{
						incorrect = true;
						Log ("Strike! You have a yahtzee module on the bomb and have a display number between 1 and 3!");
					}
				}
				break;
			case 2:
				if (textMessage3 > 3 && textMessage3 < 8) 
				{
					if (info.GetStrikes() > 3) 
					{
						Log ("Exceptions for too many strikes.");
					}
					else
					{
						incorrect = true;
						Log ("No exceptions.");
					}
				}
				if (textMessage3 > 7) 
				{
					if (info.IsIndicatorPresent(Indicator.MSA) == true) 
					{
						incorrect = true;
						Log ("Strike! There is an MSA on the bomb and the display is more than 7.");
					}
				}
				if (textMessage3 > 0 && textMessage3 < 4) 
				{
					if (info.GetSolvedModuleIDs().Contains("blackScreens")) 
					{
						Log ("Exceptions for solving one of these modules.");
					}
					else
					{
						incorrect = true;
						Log ("No exceptions.");
					}
				}
				break;
			}
			if (incorrect) 
			{
				module.HandleStrike ();
				incorrect = false;
			}
			else
			{
				correct.Play ();
				KeypadUnlocker3 = true;
			}
		}
	}

	void Log(string message)
	{
		Debug.LogFormat("[Black Screens #{0}] {1}", ModuleId, message);
	}

	void moveAfter()
	{
		Vector3 a = mover.position;
		Vector3 b = target.position;
		mover.position = Vector3.Lerp (a, b, t);
		Invoke ("moveAfter", 0);
	}
}
