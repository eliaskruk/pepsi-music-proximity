using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class menu : MonoBehaviour {

    private MainController GMS;
    public GameObject itemDefault;
    public GameObject itemDefaultB;

    Dictionary<string, string> dictionarySubItemsCat;
    Dictionary<string, GameObject> dictionarySubItems;

    //public GameObject PanelScrollRectB;

    private string actOption = "";

    // Use this for initialization
    void Start () {

        dictionarySubItemsCat = new Dictionary<string, string>();
        dictionarySubItems = new Dictionary<string, GameObject>();

        GameObject GM = GameObject.Find("MainController");
        GMS = GM.GetComponent<MainController>();

        string query_artistas = GMS.userData.query_get_artists(); //count_contenidos, ca.id, ca.artista

        GMS.db.OpenDB(GMS.dbName);
        ArrayList result = GMS.db.BasicQueryArray(query_artistas);
        GMS.db.CloseDB();

        foreach (string[] row_ in result)
        {
            //Debug.Log(row_[1]);
            GameObject clone;
            clone = Instantiate(itemDefault, itemDefault.transform.position, itemDefault.transform.rotation) as GameObject;
            clone.transform.SetParent(itemDefault.transform.parent);
            clone.transform.localScale = new Vector3(1, 1, 1);
            //clone.transform.Find("Cant").GetComponent<Text>().text = row_[0];
            clone.transform.Find("Item").GetComponent<Text>().text = row_[1];
            clone.name = "opcion-" + row_[0];

            prechargeContents(row_[0]);
        }

        itemDefault.SetActive(false);
        itemDefaultB.SetActive(false);

        hideContents();
    }

    private void hideContents() {
        GameObject[] contentsMenuI = GameObject.FindGameObjectsWithTag("menuItemsB");
        foreach (GameObject cim in contentsMenuI)
        {
            cim.SetActive(false);
        }
    }

    private void prechargeContents(string artistas_id) {

        GMS.db.OpenDB(GMS.dbName);
        string qctn = GMS.userData.query_get_contents(artistas_id);
        //Debug.Log(qctn);
        ArrayList result = GMS.db.BasicQueryArray(qctn); //id, titulo, contenido_tipo

        //itemDefaultB;

        GMS.db.CloseDB();

        foreach (string[] row_ in result)
        {

            //Debug.Log(row_[1]);
            GameObject clone;
            clone = Instantiate(itemDefaultB, itemDefaultB.transform.position, itemDefaultB.transform.rotation) as GameObject;
            clone.transform.SetParent(itemDefaultB.transform.parent);
            clone.transform.localScale = new Vector3(1, 1, 1);
            //clone.transform.Find("Cant").GetComponent<Text>().text = row_[2];
            Debug.Log("row_[3]: " + row_[3]);
            clone.transform.Find("ico").GetComponent<Image>().sprite = Resources.Load<Sprite>("icomenu_" + row_[2]);
            clone.transform.Find("Item").GetComponent<Text>().text = row_[1];
            clone.name = "opcionC-" + row_[0];

            if (row_[3] == "null" || row_[3] == "") {
                clone.GetComponent<Button>().enabled = false;

                /*clone.transform.Find("Item").GetComponent<Text>().text = clone.transform.Find("Item").GetComponent<Text>().text + Environment.NewLine +
                    "DISPONIBLE 26/09/2016";*/

                clone.transform.Find("Item").GetComponent<Text>().color = new Color(128.0f / 255.0f, 128.0f / 255.0f, 128.0f / 255.0f);

                clone.transform.Find("ico").GetComponent<Image>().sprite = Resources.Load<Sprite>("icomenu_" + row_[2] + "_disable");

                //clone.GetComponent<Text>().color = new Color(77.0f / 255.0f, 77.0f / 255.0f, 77.0f / 255.0f);
            }

            /*if (row_[4] > GMS.getActualDate2()) {

            }*/
            //GMS.getActualDate2();
            dictionarySubItems.Add(row_[0], clone);
            dictionarySubItemsCat.Add(row_[0], artistas_id);
        }
    }

    public void showContent(GameObject optionBtn) {

        //hideContents();

        string[] idOpcion = optionBtn.name.Split('-');
        Debug.Log("id opcion: " + idOpcion[1]);

        foreach (KeyValuePair<string, string> pair in dictionarySubItemsCat)
        {
            Debug.Log("id Item: " + pair.Key + " | id artista: " + pair.Value);
            if (pair.Value == idOpcion[1]) {
                GameObject myValue;
                if (dictionarySubItems.TryGetValue(pair.Key, out myValue)) {
                    if (myValue.activeSelf)
                    {
                        myValue.SetActive(false);
                    }
                    else {
                        myValue.SetActive(true);
                    }
                }
            }
        }
    }

    /*public void selectArtist(GameObject optionBtn) {
        GameObject[] contentsMenuI = GameObject.FindGameObjectsWithTag("menuItemsB");
        foreach (GameObject cim in contentsMenuI) {
            if (cim.name != "List Item")
            {
                Destroy(cim);
            }
        }
        itemDefaultB.SetActive(true);

        string[] idOpcion = optionBtn.name.Split('-');

        Debug.Log("id opcion: " + idOpcion[1]);

        GMS.db.OpenDB(GMS.dbName);
        ArrayList result = GMS.db.BasicQueryArray( GMS.userData.query_get_contents(idOpcion[1]) ); //id, titulo, contenido_tipo

        //itemDefaultB;

        GMS.db.CloseDB();

        foreach (string[] row_ in result)
        {
            Debug.Log(row_[1]);
            GameObject clone;
            clone = Instantiate(itemDefaultB, itemDefaultB.transform.position, itemDefaultB.transform.rotation) as GameObject;
            clone.transform.SetParent(itemDefaultB.transform.parent);
            clone.transform.localScale = new Vector3(1, 1, 1);
            //clone.transform.Find("Cant").GetComponent<Text>().text = row_[2];
            clone.transform.Find("ico").GetComponent<Image>().sprite = Resources.Load<Sprite>("icomenu_" + row_[2]);
            clone.transform.Find("Item").GetComponent<Text>().text = row_[1];
            clone.name = "opcion-" + row_[0];

        }
        itemDefaultB.SetActive(false);

        //mover menu contenidos
        PanelScrollRectB.GetComponent<Animation>().Play();
    }*/

    public void selectContent(GameObject optionBtn) {
        string[] idOpcion = optionBtn.name.Split('-');
        Debug.Log("id opcion: " + idOpcion[1]);

        PlayerPrefs.SetString("contenidos_id", idOpcion[1]);
        Application.LoadLevel("content");
    }

    public void gotohome() {
        Application.LoadLevel("home");
    }
}
