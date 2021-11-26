using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : Singleton<OptionManager>
{
	Dictionary<KeyOptionType, KeyCode> _KeyCode;
	public KeyCode GetKeyCode(KeyOptionType key)
	{
		return _KeyCode[key];
	}
	public void SetKeyCode(KeyOptionType key,KeyCode code)
	{
		_KeyCode[key] = code;
	}
	
	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
		_KeyCode = new Dictionary<KeyOptionType, KeyCode>();
	}

}
