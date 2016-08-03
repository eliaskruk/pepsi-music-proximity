using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Eg_NFC;

public class Example_EgNFC_Demo : MonoBehaviour {

	// NFC Android Plugin
	private Eg_NFC_DLL mNFC_Android;
	private string ReceivingFunName = "OnReceivingMsg"; //  Receive function Name

	// UI
	private Text mText_ReceivingMsg;
	private Text mText_Status;
	private Text mText_ID;
	private Text mText_Tag;
	private Button mBtn_Write;
	private Button mBtn_Read;
	private Button mBtn_Start;
	private InputField mInput_Info;

	void Awake ()   {   
		mText_ReceivingMsg = transform.Find("Panel_ReceivingMsg/Text").GetComponent<Text>();
		mText_Status = transform.Find("Panel_Status/Text_Status").GetComponent<Text>();
		mText_ID = transform.Find("Panel_Info/Txt1").GetComponent<Text>();
		mText_Tag = transform.Find("Panel_Info/Txt2").GetComponent<Text>();
		mBtn_Write = transform.Find("Btn_Read").GetComponent<Button>();
		mBtn_Read = transform.Find("Btn_Write").GetComponent<Button>();
		mInput_Info = transform.Find("InputField").GetComponent<InputField>();
	} 

	void Start() {
		if(Application.isEditor)
			Debug.LogWarning("[Warning] You have to be tested on the entity");
	}

	public void OnClick_Init() {
		mNFC_Android = new Eg_NFC_DLL();
		mNFC_Android.SetCodingType("UTF-8");
		mNFC_Android.SetListener(gameObject, ReceivingFunName);
		// Default Status
		mText_Status.text = "Read";
	}
	
	public void OnClick_Read() {
		mNFC_Android.SetStatus(0);
		mText_Status.text = "Read";
	}

	public void OnClick_Write() {
		mNFC_Android.SetStatus(1);
		mNFC_Android.Write(mInput_Info.text);
		mText_Status.text = "Write";
	}

	public void OnClick_Clear() {
		mNFC_Android.SetStatus(2);
		mText_Status.text = "Clear";
	}

	public void OnClick_CodeType1() {
		mNFC_Android.SetCodingType("UTF-8");
	}

	public void OnClick_CodeType2() {
		mNFC_Android.SetCodingType("US-ASCII");
	}

	/// <summary>
	/// 註冊送出後回傳成功訊息
	/// Listen Regist cell back.
	/// </summary>
	private void OnReceivingMsg(string str) {
		Debug.Log("ReceivingMsg: " + str);
		mText_ReceivingMsg.text = "ReceivingMsg: " + str;
		mText_ID.text = mNFC_Android.GetID();
		mText_Tag.text = mNFC_Android.GetTagData();

		string[] strs= str.Split('_');

		switch(strs[0])
		{
		case Eg_NFC_Def.JarPushType.readID:
			//	do something
			break;
		case Eg_NFC_Def.JarPushType.readTag:
			//	do something
			break;
		}
	}
}