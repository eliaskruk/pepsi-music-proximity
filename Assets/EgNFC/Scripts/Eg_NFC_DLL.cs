using UnityEngine;
using System.Collections;

namespace Eg_NFC
{
	public class Eg_NFC_DLL
	{
		private const string UnityActivityClassName	= "com.unity3d.player.UnityPlayer";
		private AndroidJavaClass EgNfCJavaClass;
		private AndroidJavaObject EgNfCJavaObject;
		private AndroidJavaObject mNFC_Mgr = null;
		private object[] arglist;
		private AndroidJavaObject jo;
		
		public Eg_NFC_DLL()
		{
			AndroidJavaClass jc = new AndroidJavaClass(UnityActivityClassName); 
			EgNfCJavaObject = jc.GetStatic<AndroidJavaObject>("currentActivity");
		}

		public int GetNFC_DriveStatus() {
			int _DriveStatus = EgNfCJavaObject.Call<int>("GetNFC_DriveStatus");
			string JavaLog = "";
			if(_DriveStatus == 0) {
				JavaLog = "This device doesn't support NFC.";
			}else if(_DriveStatus == 1) {
				JavaLog = "NFC Off.";
			}else if(_DriveStatus == 2) {
				JavaLog = "NFC On.";
			}
			Debug.Log("GetNFC_DriveStatus JavaLog: " + JavaLog);
			return _DriveStatus;
		}

		public string GetID() {
			string sReadID = EgNfCJavaObject.Call<string>("GetReadID");
			Debug.Log("GetID JavaLog: " + sReadID);
			return sReadID;
		}

		public string GetTagData() {
			string sReadTag = EgNfCJavaObject.Call<string>("GetReadTag");
			Debug.Log("GetTagData JavaLog: " + sReadTag);
			return sReadTag;
		}

		public void SetStatus(int _nStatus) {
			EgNfCJavaObject.Call("SetStatus", ToObjects(_nStatus));
		}

		public void Write(string NFC_Info) {
			EgNfCJavaObject.Call("SetWriteTag", ToObjects(NFC_Info));
		}

		/// <summary>
		/// Sets the type of the coding.
		/// _CodingType ex: "UTF-8", "US-ASCII" ..etc
		/// </summary>
		public void SetCodingType(string _CodingType) {
			EgNfCJavaObject.Call("SetCodingType", ToObjects(_CodingType));
		}
		/// <summary>
		/// 設定監聽的GameObject.
		/// Set Listener cell Gameobject.
		/// </summary>
		public void SetListener(GameObject obj, string _ReceivingFunName) {
			EgNfCJavaObject.Call("SetlistenerGameObjectName", ToObjects(obj.name, _ReceivingFunName));
		}

		private object[] ToObjects(object arg1, object arg2 = null, object arg3 = null) {
			int nCount = 0;

			if(arg1 != null) nCount++;
			if(arg2 != null) nCount++;
			if(arg3 != null) nCount++;

			object[] m_objects = new object[nCount];

			if(arg1 != null) m_objects[0] = arg1;
			if(arg2 != null) m_objects[1] = arg2;
			if(arg3 != null) m_objects[2] = arg3;

			return m_objects;
		}
	}
}