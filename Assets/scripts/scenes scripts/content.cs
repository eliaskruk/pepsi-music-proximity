using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using NAudio;
using NAudio.Wave;
using System.IO;
using System;

public class content : MonoBehaviour {

    public string filepathOk;
    private bool isCharged = false;

    public MediaPlayerCtrl scrMedia;

    private MainController GMS;
    private string content_id;

    public Text Titulo;
    public Text Subtitulo;
    public Text ContenidoText;

    public Text TextAbout;
    public Text TextDescription;

    public GameObject contentVideo;
    public GameObject contentText;
    public GameObject contentAudio;
    public GameObject contentImage;

    private string ActualCtn;

    public Image BkcImage;

    public AudioSource audio;

    public Image panelImage;

    //video
    public Image VideoButton;

    //audio
    private IWavePlayer mWaveOutDevice;
    private WaveStream mMainOutputStream;
    private WaveChannel32 mVolumeStream;

    private string debugAudio = "";
    private string debugTimer = "";

    public GameObject AudioSlider;
    public Image AudioButton;

    public int audioTime;
    private float partialTime;
    public int actSeconds = 0;

    private int SSeconds = 0;
    private float tomoveinSecs = 0f;
    private bool isPlayingAudio = false;
    float actPos = 0f;

    public Animation footerDescription;
    private bool isFooterOpen = false;

    private void hideContentsPanels() {
        contentVideo.SetActive(false);
        contentText.SetActive(false);
        contentAudio.SetActive(false);
        contentImage.SetActive(false);
    }

    private void showContentPanel() {
        if (GMS.ContentData.contenido_tipo == "texto")
        {
            contentText.SetActive(true);
        }
        else if (GMS.ContentData.contenido_tipo == "video")
        {
            contentVideo.SetActive(true);
        }
        else if (GMS.ContentData.contenido_tipo == "audio")
        {
            contentAudio.SetActive(true);
        }
        else if (GMS.ContentData.contenido_tipo == "imagen")
        {
            contentImage.SetActive(true);
        }
    }

    // Use this for initialization
    void Start() {

        hideContentsPanels();

        GameObject GM = GameObject.Find("MainController");
        GMS = GM.GetComponent<MainController>();

        GMS.isDonwloading = true;

        content_id = PlayerPrefs.GetString("contenidos_id");

        GMS.db.OpenDB(GMS.dbName);
        ArrayList result = GMS.db.BasicQueryArray(GMS.ContentData.query_get_contenido(content_id));
        GMS.db.CloseDB();

        //populate contenido
        GMS.ContentData.populateContent((string[])result[0]);

        Titulo.text = GMS.ContentData.titulo;
        Subtitulo.text = GMS.ContentData.subtitulo;

        TextAbout.text = "About " + GMS.ContentData.artista;
        TextDescription.text = GMS.ContentData.artista_descripcion;

        if (GMS.ContentData.contenido_tipo != "imagen")
        {
            //verificar si tiene imagen de contenido
            string ff = GMS.checkFileExist2(GMS.ContentData.imagen);
            if (ff != "")
            {
                BkcImage.sprite = GMS.spriteFromFileP(GMS.ContentData.artista_imagen); ;
            }
            else {
                //verificar si tiene imagen de artista
                ff = GMS.checkFileExist2(GMS.ContentData.artista_imagen);
                if (ff != "")
                {
                    BkcImage.sprite = GMS.spriteFromFileP(GMS.ContentData.artista_imagen); ;
                }
            }
            Debug.Log("archivo imagen: " + ff);
        }

        /*if (GMS.checkFileExist(GMS.ContentData.artista_imagen)) {
            BkcImage.sprite = GMS.spriteFromFile(GMS.ContentData.artista_imagen);
        }*/

        showContentPanel();

        if (GMS.ContentData.contenido_tipo == "texto")
        {
            ContenidoText.text = GMS.ContentData.contenido;
            GMS.isDonwloading = false;
        }
        else if (GMS.ContentData.contenido_tipo == "video")
        {
            Debug.Log("Entra a checkContenidoFiles");
            checkContenidoFiles("video");
            //loadVideo();
        }
        else if (GMS.ContentData.contenido_tipo == "audio")
        {
            //getDataFromAudio();
            //LoadAudio2();
            //StartCoroutine(loadAudio3());
            checkContenidoFiles("audio");
        }
        else if (GMS.ContentData.contenido_tipo == "imagen")
        {
            //verificar q el contenido este precargado
            checkContenidoFiles("imagen");
            //panelImage.sprite = GMS.spriteFromFile(filepath);
        }
        
    }

    
    private void checkContenidoFiles(string tipoFile = "") {

        filepathOk = Application.persistentDataPath + "/" + GMS.ContentData.contenido;

        Debug.Log("filepathOk: " + filepathOk);
        if (!File.Exists(filepathOk))
        {

            if (GMS.ContentData.precargado == "1")
            {
                filepathOk = GMS.ContentData.contenido;
                GMS.isDonwloading = false;
            }
            else
            {
                //descargar contenido
                if (!GMS.haveInet)
                {
                    GMS.errorPopup("Necesitas internet para poder descargar el contenido desbloqueado");
                }
                else
                {
                    Debug.Log("como no existe el archivo lo descarga!!");
                    GMS.isDonwloading = true;
                    GMS.Downloading.SetActive(true);
                    GMS.downloadFile(GMS.ContentData.contenido, true);
                    GMS.showLoading(false);
                }
            }
        }
        else
        {
            if (tipoFile == "video")
            {
                filepathOk = "file://" + filepathOk;
            }
            GMS.isDonwloading = false;
        }
        
    }

    public IEnumerator moveFileToPersistentDataPath() {

        string filepath = Application.persistentDataPath + "/" + GMS.ContentData.contenido;

        if (!File.Exists(filepath))
        {
            WWW AFile = new WWW(Application.streamingAssetsPath + "/" + GMS.ContentData.contenido);  // this is the path to your StreamingAssets in android

            while (!AFile.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check

            File.WriteAllBytes(filepath, AFile.bytes);

            yield return AFile;
        }
    }

    void Update()
    {
        updateAudioSlider();

        //if (!GMS.isDonwloading && !isCharged) {
        if (!isCharged)
        {
            isCharged = true;
            if (GMS.ContentData.contenido_tipo == "imagen") {
                panelImage.sprite = GMS.spriteFromFileP(filepathOk);
            }
            if (GMS.ContentData.contenido_tipo == "audio")
            {
                StartCoroutine(loadAudio4());
            }
            if (GMS.ContentData.contenido_tipo == "video")
            {
                loadVideo();
                
            }
        }
        

        //debugTimer = " time: " + audio.timeSamples;
    }

    void loadVideo()
    {
        scrMedia.Load(filepathOk);

        StartCoroutine(playVideoE());
    }

    IEnumerator playVideoE() {
        yield return new WaitForSeconds(1);
        playVideo();
    }

    public void playVideo()
    {

        if (isPlayingAudio)
        {
            isPlayingAudio = false;
            scrMedia.Pause();
            VideoButton.sprite = Resources.Load<Sprite>("btnPlay");

            Debug.Log("Pause video");
        }
        else {
            isPlayingAudio = true;
            scrMedia.Play();
            VideoButton.sprite = Resources.Load<Sprite>("btnPause");

            Debug.Log("Play video");
        }

        //Debug.Log("Play video");
        //yield return new WaitForSeconds(1);
        //scrMedia.Play();
    }
    /*
    IEnumerator loadAudio()
    {
        string filepath = Application.persistentDataPath + "/" + GMS.ContentData.contenido;
        Debug.Log(filepath);
        WWW www = new WWW("file://" + filepath);  // start a download of the given URL
        yield return www;
        AudioClip clip = www.GetAudioClip(false, true); // 2D, streaming

        audio.clip = clip;
        initSlide();
    }*/

    public int screenW;

    private IEnumerator initSlide()
    {
        yield return new WaitForSeconds(1);
        debugTimer = " time: " + audio.clip.length;
        audioTime = Mathf.CeilToInt(audio.clip.length);
        //int screenW = Screen.width;
        float ceroPos = AudioSlider.transform.localPosition.x;
        actPos = screenW * 1f;

        AudioSlider.transform.localPosition = new Vector3(actPos * -1f, AudioSlider.transform.localPosition.y, AudioSlider.transform.localPosition.z);

        Debug.Log("ceroPos: " + ceroPos + " actPos: " + actPos);

        tomoveinSecs = (actPos - ceroPos) / audioTime;
        Debug.Log("tomoveinSecs: " + tomoveinSecs);
        
    }

    private void updateAudioSlider() {
        if (isPlayingAudio && actSeconds < audioTime)
        {
            partialTime += Time.deltaTime;
            
            int roundedRestSeconds = Mathf.CeilToInt(partialTime);
            actSeconds = roundedRestSeconds % 60;
            if (actSeconds != SSeconds)
            {
                SSeconds = actSeconds;
                Debug.Log(actSeconds);
                actPos = actPos - tomoveinSecs;
                AudioSlider.transform.localPosition = new Vector3(actPos * -1, AudioSlider.transform.localPosition.y, AudioSlider.transform.localPosition.z);
            }
        }
    }

    public void playAudio()
    {
        if (isPlayingAudio)
        {
            isPlayingAudio = false;
            audio.Pause();
            AudioButton.sprite = Resources.Load<Sprite>("btnPlay");
            
            Debug.Log("Pause audio");
        }
        else {
            isPlayingAudio = true;
            audio.Play();
            AudioButton.sprite = Resources.Load<Sprite>("btnPause");
            
            Debug.Log("Play audio");
        }
        
        
    }


    public void OpenFooterDescription()
    {
        if (isFooterOpen)
        {
            showContentPanel();
            footerDescription["footerDescription"].speed = -1;
            isFooterOpen = false;
        }
        else {
            hideContentsPanels();
            footerDescription["footerDescription"].speed = 1;
            isFooterOpen = true;
        }
        footerDescription.Play();
    }


    //audio test functions

    IEnumerator loadAudio4()
    {
        debugAudio = "init LoadAudio4";
        Debug.Log("File: " + filepathOk);
        debugAudio = "file: " + filepathOk;

#if UNITY_EDITOR
            WWW www = new WWW("file:///" + filepathOk);  // start a download of the given URL
#else
            WWW www = new WWW("file://" + filepathOk);  // start a download of the given URL
#endif

        yield return www;
        AudioClip clip = www.GetAudioClip(false, true, AudioType.MPEG);

        audio.clip = clip;
        StartCoroutine(initSlide());
    }

    IEnumerator loadAudio3()
    {
        debugAudio = "init LoadAudio3";

        //verificar q el contenido este precargado
        string filepath = Application.streamingAssetsPath + "/" + GMS.ContentData.contenido;
        if (!File.Exists(filepath)){
            Debug.Log("contenido NO precargado");
            filepath = Application.persistentDataPath + "/" + GMS.ContentData.contenido;
        }


        Debug.Log(filepath);
        debugAudio = "file: " + filepath;

        WWW www = new WWW("file://" + filepath);  // start a download of the given URL
        yield return www;
        AudioClip clip = www.GetAudioClip(false, true, AudioType.MPEG);

        audio.clip = clip;
        StartCoroutine(initSlide());
    }

    private static float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 2];

        for (int i = 0; i < floatArr.Length; i++)
        {
            floatArr[i] = ((float)BitConverter.ToInt16(array, i * 2)) / 32768.0f;
        }

        return floatArr;
    }

    private void getDataFromAudio()
    {

        byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/content1465502810.wav");
        float[] f = ConvertByteToFloat(bytes);

        AudioClip audioClip = AudioClip.Create("testSound", f.Length, 2, 44100, false, false);
        audioClip.SetData(f, 0);

        audio.clip = audioClip;
        audio.Play();
    }

    private bool LoadAudioFromData(byte[] data)
    {
        debugAudio = "LoadAudioFromData";
        try
        {
            MemoryStream tmpStr = new MemoryStream(data);
            mMainOutputStream = new Mp3FileReader(tmpStr);
            mVolumeStream = new WaveChannel32(mMainOutputStream);

            mWaveOutDevice = new WaveOut();
            mWaveOutDevice.Init(mVolumeStream);

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error! " + ex.Message);
        }

        return false;
    }

    private string audioFile = "";
    private void LoadAudio2()
    {

        debugAudio = "init LoadAudio2";

        //string filepath = Application.persistentDataPath + "/" + GMS.ContentData.contenido;
        audioFile = Application.persistentDataPath + "/" + GMS.ContentData.contenido;

        WWW www = new WWW("file://" + audioFile);
        debugAudio = "call www";
        //Debug.Log("path = " + cLocalPath + ofd.FileName);
        while (!www.isDone) { };
        if (!string.IsNullOrEmpty(www.error))
        {
            debugAudio = "NO open file: " + audioFile + ", " + www.error;
            //System.Windows.Forms.MessageBox.Show("Error! Cannot open file: " + ofd.FileName + "; " + www.error);
            return;
        }

        byte[] imageData = www.bytes;

        if (!LoadAudioFromData(imageData))
        {
            debugAudio = "Cannot open mp3 file!";
            //System.Windows.Forms.MessageBox.Show("Cannot open mp3 file!");
            return;
        }

        debugAudio = "LoadAudioFromData";
        mWaveOutDevice.Play();

        Resources.UnloadUnusedAssets();
        
    }

    private void UnloadAudio()
    {
        if (mWaveOutDevice != null)
        {
            mWaveOutDevice.Stop();
        }
        if (mMainOutputStream != null)
        {
            // this one really closes the file and ACM conversion
            mVolumeStream.Close();
            mVolumeStream = null;

            // this one does the metering stream
            mMainOutputStream.Close();
            mMainOutputStream = null;
        }
        if (mWaveOutDevice != null)
        {
            mWaveOutDevice.Dispose();
            mWaveOutDevice = null;
        }
    }

    void OnGUI()
    {
        if (false)
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(0, Screen.height * 0.2f, Screen.width, 80), "Audio Debug: " + debugAudio);

            GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, 80), "timer: " + debugTimer);
        }
    }
}
