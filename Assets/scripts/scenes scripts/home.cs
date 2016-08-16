using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Eg_NFC;

public class home : MonoBehaviour {

    private string nfc_text = "";
    // NFC Android Plugin
    private Eg_NFC_DLL mNFC_Android;
    private string ReceivingFunName = "OnReceivingMsg"; //  Receive function Name

    private MainController GMS;
    public InputField Codigo;
    public GameObject buttonSubmit;
    public GameObject Downloading;

    public GameObject ContenidoDesbloqueado;

    // Use this for initialization
    void Start () {
        GameObject GM = GameObject.Find("MainController");
        GMS = GM.GetComponent<MainController>();

#if !UNITY_EDITOR
        initNFC();
#endif
    }

    // Update is called once per frame
    void Update () {
        if (Codigo.text != "" )
        {
            if (!buttonSubmit.activeSelf)
            {
                buttonSubmit.SetActive(true);
            }
        }
        else {
            if (buttonSubmit.activeSelf)
            {
                buttonSubmit.SetActive(false);
            }
        }
    }

    public void submit()
    {
        GMS.db.OpenDB(GMS.dbName);
        ArrayList resultC = GMS.db.BasicQueryArray("select id from codigos where codigo = '" + Codigo.text + "' ");
        GMS.db.CloseDB();

        if (resultC.Count > 0)
        {
            GMS.db.OpenDB(GMS.dbName);
            ArrayList result = GMS.db.BasicQueryArray("select id from contenidos_usuarios where usuarios_id = '" + GMS.userData.id + "' and codigos_id = '" + ((string[])resultC[0])[0] + "' ");
            GMS.db.CloseDB();

            if (result.Count > 0)
            {
                GMS.errorPopup("El Código ya ha sido utilizado");
            }
            else
            {
                getRandContent(((string[])resultC[0])[0]);
            }
        }
        else{
            GMS.errorPopup("Codigo incorrecto");
        }

    }
    

    private void getRandContent(string codigo_id) {
        ArrayList resContenido;

        string fecha_entrada = GMS.getActualDate();

        //verificar si tengo el primero contenido y cargarlo
        GMS.db.OpenDB(GMS.dbName);
        ArrayList result = GMS.db.BasicQueryArray("SELECT contenidos_id FROM contenidos_usuarios WHERE usuarios_id = '" + GMS.userData.id + "' AND contenidos_id = '1' ");
        GMS.db.CloseDB();

        if (result.Count == 0)
        {
            //Obtengo el primer contenido
            GMS.db.OpenDB(GMS.dbName);
            resContenido = GMS.db.BasicQueryArray("SELECT * FROM contenidos WHERE id = 1");
            GMS.db.CloseDB();
            this.guardarContenidoDesbloq(resContenido, codigo_id, fecha_entrada);
        }
        else
        {
            //Obtener el contenido del dia si no lo tiene
            GMS.db.OpenDB(GMS.dbName);
            string queryAux = GMS.userData.query_get_day_contenido(fecha_entrada);
            Debug.Log(queryAux);
            resContenido = GMS.db.BasicQueryArray(queryAux);
            GMS.db.CloseDB();

            if (resContenido.Count > 0)
            {
                this.guardarContenidoDesbloq(resContenido, codigo_id, fecha_entrada);
            }
            else
            {

                //Obtener contenido mas antiguo
                GMS.db.OpenDB(GMS.dbName);
                string randContentQ = GMS.userData.query_get_contenido_old(fecha_entrada);
                Debug.Log(randContentQ);
                resContenido = GMS.db.BasicQueryArray(randContentQ); //id, titulo, contenido_tipo, contenido, imagen
                GMS.db.CloseDB();

                if (resContenido.Count > 0)
                {
                    this.guardarContenidoDesbloq(resContenido, codigo_id, fecha_entrada);
                }
                else
                {
                    GMS.errorPopup("Ya no hay mas contenidos..");
                }
            }
        }
    }

    public void guardarContenidoDesbloq(ArrayList result, string codigo_id, string fecha_entrada) {
        //cargar contenido usuario
        string newId = GMS.getNewId("contenidos_usuarios");
        string[] cols = new string[] { "id", "usuarios_id", "codigos_id", "contenidos_id", "fecha_entrada" };
        string[] colsVals = new string[] { newId, GMS.userData.id.ToString(), codigo_id, ((string[])result[0])[0], fecha_entrada };

        GMS.db.OpenDB(GMS.dbName);
        GMS.db.InsertIntoSpecific("contenidos_usuarios", cols, colsVals);
        GMS.db.CloseDB();

        //insertar para sync
        string[] fields = { "usuarios_id", "contenidos_id", "fecha_entrada", "codigos_id", "actLat", "actLng" };
        string[] values = { GMS.userData.id.ToString(), ((string[])result[0])[0], fecha_entrada, codigo_id, GMS.userLat, GMS.userLng };
        GMS.insert_sync(fields, values, "codigos_usuarios");

        PlayerPrefs.SetString("contenidos_id", ((string[])result[0])[0]);
        ContenidoDesbloqueado.SetActive(true);
        StartCoroutine(GMS.redirect("content", 4f));
    }

    private void submitNFC(string codigos_id) {
        getRandContent(codigos_id);
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 32;
        //GUI.Label(new Rect(0, Screen.height * 0.5f, Screen.width, 40), "NFC mgs: " + nfc_text);
    }

    //NFC
    private void initNFC() {
        nfc_text = "initNFC";

        mNFC_Android = new Eg_NFC_DLL();
        mNFC_Android.SetCodingType("UTF-8");
        mNFC_Android.SetListener(gameObject, ReceivingFunName);
        // Default Status
        nfc_text = "Reading NFC";

        mNFC_Android.SetStatus(0);
    }


    private void OnReceivingMsg(string str)
    {
        Debug.Log("ReceivingMsg: " + str);
        nfc_text = mNFC_Android.GetID();
        nfc_text += ": ";
        nfc_text += mNFC_Android.GetTagData();

        submitNFC(nfc_text);

        nfc_text += " | ";
        nfc_text += str;

        string[] strs = str.Split('_');

        switch (strs[0])
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
