using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using Mkey;

public class Connect : Singleton<Connect>
{
	[SerializeField]
	SocketIOComponent socket;

	Action<JSONObject> LoginCallback;
	Action<JSONObject> GetPatternCallback;
	[Serializable]
	public struct Balance
	{
		public string balance;
	}

	private void Start()
	{
		socket.Connect();
		socket.On("open", TestOpen);
		socket.On("error", TestError);
		socket.On("close", TestClose);
		socket.On("Login", Login);
		socket.On("KickPlayer", KickPlayer);
		socket.On("UpdateBalance", UpdateBalance);
		socket.On("GetSpinResult", GetPattern);
	}

	bool isWaitingGetPattern;

	public IEnumerator EmitGetPattern(JSONObject json, Action<JSONObject> callback, Action contLoop)
	{

		GetPatternCallback = callback;
		isWaitingGetPattern = true;
		socket.Emit("GetSpinResult", json);

		while (isWaitingGetPattern)
		{

			contLoop.Invoke();
			yield return null;
		}

	}


	public void EmitLogin(JSONObject json, Action<JSONObject> callback)
	{
		LoginCallback = callback;
		socket.Emit("Login", json);
	}
	
	public void EmitLogout()
	{
		Debug.Log(">>>>>>> Loging out");
		socket.Emit("Logout");
	}

	public void Kick()
	{
		JSONObject j = new JSONObject();

		j.AddField("username", "wyne969");
		socket.Emit("KickPlayer",j);
	}

	void Login(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: e.name " + e.name + " e.data " + e.data);
		if(LoginCallback != null)
		{
			LoginCallback.Invoke(e.data);
		}
	}

	void GetPattern(SocketIOEvent e)
	{
		Debug.Log("Getpatterm: " + e.name + " e.data " + e.data);
		isWaitingGetPattern = false;
		if (GetPatternCallback != null)
		{
			GetPatternCallback.Invoke(e.data);
		}
	}

	void UpdateBalance(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: e.name " + e.name + " e.data " + e.data);
		int balance = int.Parse(accessData(e.data, "balance"));

		SlotPlayer.Instance.SetCoinsCount(balance);


	}

	void KickPlayer(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: e.name " + e.name + " e.data " + e.data);
		Debug.Log("Kicked Player");
		Application.Quit();
	}

	void TestOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: e.name " + e.name + " e.data " + e.data);
	}


	void TestError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}

	void TestClose(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}

	string accessData(JSONObject obj, string keyValue)
	{
		if(obj.type == JSONObject.Type.OBJECT)
		{
			for (int i = 0; i < obj.list.Count; i++)
			{
				string key = (string)obj.keys[i];
				JSONObject j = (JSONObject)obj.list[i];
				if (keyValue == key)
				{
					return accessData(j);
				}

			}
		}
		return "NULL";
	}

	string accessData(JSONObject obj)
	{
		switch (obj.type)
		{
			case JSONObject.Type.ARRAY:
				foreach (JSONObject j in obj.list)
				{
					return accessData(j);
				}
				break;
			case JSONObject.Type.STRING:
				Debug.Log(obj.str);
				return obj.str;

			case JSONObject.Type.NUMBER:
				Debug.Log(obj.n);
				return obj.n.ToString();
			case JSONObject.Type.BOOL:
				Debug.Log(obj.b);
				return obj.b.ToString();
			case JSONObject.Type.NULL:
				Debug.Log("NULL");
				return "NULL";

		}
		return "NULL";
	}
}
