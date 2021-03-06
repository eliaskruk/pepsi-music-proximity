﻿using UnityEngine;
using System.Collections;

//Add these Namespaces
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility.Demo;

public class notifications : MonoBehaviour {

	private MainController GMS;

	[SerializeField, EnumMaskField(typeof(NotificationType))]
	private NotificationType	m_notificationType;
	
	
	void Start()
	{
		Debug.Log ("start notif controller");
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

#if UNITY_EDITOR
        GMS.userData.plataforma = "Android";
        GMS.userData.reg_id = "11111111111";
#endif

        string config_alerts = PlayerPrefs.GetString ("config_alerts");
		Debug.Log ("notificaciones activadas: " + config_alerts);
		NPBinding.NotificationService.RegisterNotificationTypes (m_notificationType);
		//if (config_alerts == "true") {
			NPBinding.NotificationService.RegisterForRemoteNotifications ();
		//}
	}

	public void disableNotifs(){
		NPBinding.NotificationService.UnregisterForRemoteNotifications ();
	}

	public void enableNotifs(){
		NPBinding.NotificationService.RegisterForRemoteNotifications ();
	}

	void OnEnable ()
	{
		Debug.Log ("enable notif");
		// Register RemoteNotificated related callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	+= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			+= DidReceiveRemoteNotificationEvent;
		
		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			+= DidReceiveLocalNotificationEvent;
		
	}
	
	void OnDisable ()
	{
		// Un-Register from callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	-= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			-= DidReceiveRemoteNotificationEvent;
		
		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			-= DidReceiveLocalNotificationEvent;
		
	}
	
	
	#region API Callbacks
	
	private void DidReceiveLocalNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Received DidReceiveLocalNotificationEvent : " + _notification.ToString());
	}
	
	private void DidReceiveRemoteNotificationEvent (CrossPlatformNotification _notification)
	{
		IDictionary _userInfo 			= _notification.UserInfo;
		if (_userInfo != null) {
			//(string)_userInfo["notifType"] == "chat"
		}

		string[] m_buttons = new string[] { "Cerrar", "Ver" };
		NPBinding.UI.ShowAlertDialogWithMultipleButtons ("Tienes una nueva notificación", _notification.AlertBody, m_buttons, (string _buttonPressed)=>{
			if(_buttonPressed == "Ver"){

				//Application.LoadLevel ("notificaciones");
			}
		});
	}

    private void processRecivedNotif(CrossPlatformNotification _notification)
    {
    }

    private void DidLaunchWithRemoteNotificationEvent (CrossPlatformNotification _notification)
	{

	}

    

    private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
    {

        Debug.Log("DidFinishRegisterForRemoteNotificationEvent");
		if(string.IsNullOrEmpty(_error))
		{
			Debug.Log("Device Token : " + _deviceToken);
			GMS.userData.reg_id = _deviceToken;

            #if !UNITY_EDITOR
                #if UNITY_ANDROID
	                GMS.userData.plataforma = "Android";
                #else
	                GMS.userData.plataforma = "IOS";
                #endif
            #endif
        }
		else
		{
			Debug.Log("Error in registering for remote notifications : " + _deviceToken);
            #if UNITY_EDITOR
                GMS.userData.plataforma = "Android";
                GMS.userData.reg_id = "11111111111";
            #endif
        }
	}
	
#endregion
}
