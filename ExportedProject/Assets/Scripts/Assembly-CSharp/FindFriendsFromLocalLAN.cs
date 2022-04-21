using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Rilisoft.MiniJson;
using UnityEngine;

public class FindFriendsFromLocalLAN : MonoBehaviour
{
	public static bool isFindLocalFriends = false;

	private string ipaddress;

	public static List<string> lanPlayerInfo = new List<string>();

	public static Action lanPlayerInfoUpdate = null;

	private UdpClient objUDPClient;

	private float periodSendMyInfo = 30f;

	private float timeSendMyInfo;

	private bool isGetMessage;

	private bool isActiveFriends;

	private List<string> idsForInfo = new List<string>();

	private void Start()
	{
		ipaddress = Network.player.ipAddress.ToString();
		StartBroadcastingSession();
	}

	private void StartBroadcastingSession()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		objUDPClient = new UdpClient(22044);
		objUDPClient.set_EnableBroadcast(true);
		BeginAsyncReceive();
	}

	public void StopBroadCasting()
	{
		if (objUDPClient != null)
		{
			UdpClient val = objUDPClient;
			objUDPClient = null;
			val.Close();
		}
	}

	private void BeginAsyncReceive()
	{
		if (objUDPClient != null)
		{
			objUDPClient.BeginReceive((AsyncCallback)GetAsyncReceive, (object)null);
		}
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		if (pause)
		{
			StopBroadCasting();
			yield break;
		}
		yield return null;
		yield return null;
		yield return null;
		StartBroadcastingSession();
	}

	private void GetAsyncReceive(IAsyncResult objResult)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		if (objUDPClient == null)
		{
			return;
		}
		IPEndPoint val = new IPEndPoint(IPAddress.Any, 0);
		byte[] array = objUDPClient.EndReceive(objResult, ref val);
		if (array.Length > 0 && !val.get_Address().ToString().Equals(ipaddress))
		{
			string @string = Encoding.Unicode.GetString(array);
			List<object> list = Json.Deserialize(@string) as List<object>;
			string text = string.Empty;
			if (list != null && list.Count == 1)
			{
				text = Convert.ToString(list[0]);
			}
			if (!string.IsNullOrEmpty(text) && !lanPlayerInfo.Contains(text) && !FriendsController.sharedController.friends.Contains(text))
			{
				lanPlayerInfo.Add(text);
				if (isActiveFriends)
				{
					idsForInfo.Add(text);
				}
			}
			isGetMessage = true;
		}
		BeginAsyncReceive();
	}

	private void SendMyInfo()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		if (string.IsNullOrEmpty(FriendsController.sharedController.id))
		{
			return;
		}
		timeSendMyInfo = Time.time;
		List<string> list = new List<string>();
		list.Add(FriendsController.sharedController.id);
		string s = Json.Serialize(list);
		byte[] bytes = Encoding.Unicode.GetBytes(s);
		if (objUDPClient != null)
		{
			try
			{
				objUDPClient.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, 22044));
				return;
			}
			catch (Exception ex)
			{
				Debug.Log("soccet close " + ex);
				return;
			}
		}
		Debug.Log("objUDPClient=NULL");
	}

	private void Update()
	{
		isActiveFriends = FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled;
		if (idsForInfo.Count > 0)
		{
			FriendsController.sharedController.GetInfoAboutPlayers(idsForInfo);
			idsForInfo.Clear();
		}
		if ((isActiveFriends || isGetMessage) && Time.time - timeSendMyInfo > periodSendMyInfo)
		{
			isGetMessage = false;
			SendMyInfo();
		}
	}
}
