using UnityEngine;
using System.Collections;

public class GPSController : MonoBehaviour {

    public MainController GMS;

    void Start()
    {

        GameObject GM = GameObject.Find("MainController");
        GMS = GM.GetComponent<MainController>();

        //StartCoroutine(init_gps());
        // Stop service if there is no need to query location updates continuously
        //Input.location.Stop();
    }

    private IEnumerator refreshGps() {
        yield return new WaitForSeconds(3);
        if (GMS.gps_active) {
            if (Input.location.lastData.latitude != 0f) {
                GMS.userLat = Input.location.lastData.latitude.ToString();
                GMS.userLng = Input.location.lastData.longitude.ToString();
            }
        }

        StartCoroutine(refreshGps());
    }

    private IEnumerator init_gps()
    {
        yield return new WaitForSeconds(2);

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS deshabilitado");

            if (!GMS.popup.activeSelf)
            {
                NPBinding.UI.ShowAlertDialogWithSingleButton("Alerta!", "GPS deshabilitado, por favor habilitelo para continuar", "Aceptar", (string _buttonPressed) => {
                    if (_buttonPressed == "Aceptar")
                    {
                        StartCoroutine(init_gps());
                    }
                });
            }
            
            // remind user to enable GPS
            // As far as I know, there is no way to forward user to GPS setting menu in Unity
        }
        else {
            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
                Debug.Log("veces reintentando: " + maxWait);
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                Debug.Log("error");
                GMS.gps_active = false;

                StartCoroutine(init_gps());
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("conexion fallida");
                GMS.gps_active = false;
                StartCoroutine(init_gps());
            }

            // Access granted and location value could be retrieved
            else {
                Debug.Log("ubicacion: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
                
                //Debug.Log("cargar lat y lng de GPS para clima...");
                GMS.userLat = Input.location.lastData.latitude.ToString();
                GMS.userLng = Input.location.lastData.longitude.ToString();
                
            }

            StartCoroutine(refreshGps());
        }
    }
}
