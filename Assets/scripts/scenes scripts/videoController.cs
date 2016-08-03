using UnityEngine;
using System.Collections;
using System.IO;

public class videoController : MonoBehaviour {
    public MediaPlayerCtrl scrMedia;

    public AudioSource audio;

    
    // Use this for initialization
    void Start () {
        isDonwloading = true;
        //scrMedia.Play();
        startLoading();

        string filep = "contenido1.jpg";
        //checkContenidoFiles(filep);

        //scrMedia.Load("loading.mp4");
        //StartCoroutine(dd());
    }

    private IEnumerator dd() {
        yield return new WaitForSeconds(1f);
        scrMedia.Play();
        //scrMedia.SeekTo(1500);

        StartCoroutine(dd2());
        //scrMedia.SetVolume(0f);
        //scrMedia.Play();
    }
    private IEnumerator dd2(){
        Debug.Log("SeekTo");
        yield return new WaitForSeconds(1f);
        scrMedia.SeekTo(40000);
    }

    private void loadImage() {

    }

    string filepathOk;
    bool isDonwloading = false;

    private void checkContenidoFiles(string filep = "")
    {
        filepathOk = Application.streamingAssetsPath + "/" + filep;
        Debug.Log("streaming path: " + filepathOk);
        if (!File.Exists( filepathOk))
        {
            Debug.Log("contenido NO precargado");
            filepathOk = Application.persistentDataPath + "/" + filep;

            if (!File.Exists(filepathOk))
            {
                Debug.Log("necesitas descargar el contenido");
            }
            else {
                isDonwloading = false;
            }
        }
        else {
            Debug.Log("contenido SI precargado");
            isDonwloading = false;
        }
    }

    public void playVideo() {
        //scrMedia.Load();
        string nombre_ = "loading.mp4";
        string filepath = Application.persistentDataPath + "/" + nombre_;

        scrMedia.Load("file://" + filepath);

        //scrMedia.Play();
        StartCoroutine(startVideo());
        Debug.Log(filepath);
    }

    private float actTime = 5f;
    float newTime = 0.5f;

    private void startLoading()
    {
        //scrMedia.Load("file://" + "/loading.mp4");
        scrMedia.SetVolume(0f);
        StartCoroutine(simLoad(1f));
    }

    private IEnumerator simLoad(float simTime) {
        yield return new WaitForSeconds(simTime);
        
        actTime += simTime;
        
        newTime = 1f;
        if ( (actTime > 3 && actTime < 5) || (actTime > 8 && actTime < 10) ) {
            newTime = 2.3f;
        }
        seekToVideo();
        
    }

    public void seekToVideo()
    {
        int seekto = (int)Mathf.Round(actTime) * 1000;
        if (seekto != 0)
        {
            scrMedia.Play();
            Debug.Log("SeekTo: " + seekto);
            scrMedia.SeekTo(seekto);
            scrMedia.SetVolume(0f);
            StartCoroutine(stopSeekToVideo());
        }
    }

    private IEnumerator stopSeekToVideo()
    {
        yield return new WaitForSeconds(0.5f);
        scrMedia.Stop();
        StartCoroutine(simLoad(newTime));
    }


    private IEnumerator startVideo() {
        Debug.Log("Play video");
        yield return new WaitForSeconds(1);
        scrMedia.Play();
    }

    public void CalcVideoLength() {
        scrMedia.GetDuration();
    }

   

    public void playAudio()
    {
        //startAudio();

        string nombre_ = "musica1.mp3";
        string filepath = Application.persistentDataPath + "/" + nombre_;

        StartCoroutine(DownloadAndPlay("file://" + filepath));
    }

    private void startAudio() {

        string nombre_ = "musica1.mp3";
        string filepath = Application.persistentDataPath + "/" + nombre_;
        Debug.Log(filepath);
        WWW www = new WWW(filepath);  // start a download of the given URL
        AudioClip clip  = www.GetAudioClip(false, true); // 2D, streaming

        /*while (!clip.isReadyToPlay)
        {
            //progress = "downloaded " + (www.progress * 100).ToString() + "%...";
            //Debug.Log(progress);
            yield return null;
        }*/

        /*while (!clip.isReadyToPlay)
        {
            Debug.Log("Waiting");
            yield 0;
        }*/

        audio.clip = clip;
        audio.Play();
    }

    IEnumerator DownloadAndPlay(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        //AudioSource audio = GetComponent<AudioSource>();
        audio.clip = www.GetAudioClip(false, false, AudioType.MPEG);
        audio.name = url.Substring(url.LastIndexOf('/') + 1);

        audio.Play();
    }

    public GameObject AudioSlider;

    public int audioTime;
    private float partialTime;
    public int actSeconds = 0;

    private int SSeconds = 0;
    private float tomoveinSecs = 0f;
    private bool isInitAudio = false;
    float actPos = 0f;

    public void moveSlide() {
        int screenW = Screen.width;
        float ceroPos = AudioSlider.transform.localPosition.x;
        actPos = screenW * 1f;

        AudioSlider.transform.localPosition = new Vector3(actPos * -1f, AudioSlider.transform.localPosition.y, AudioSlider.transform.localPosition.z);
        
        //calculo movimiento por segundo
        
        

        Debug.Log("ceroPos: " + ceroPos + " actPos: " + actPos);

        tomoveinSecs = (actPos - ceroPos) / audioTime;
        Debug.Log("tomoveinSecs: " + tomoveinSecs);
        isInitAudio = true;
    }

    void Update() {
        if (isInitAudio && actSeconds < audioTime)
        {
            partialTime += Time.deltaTime;

            //calculos de paseo
            int roundedRestSeconds = Mathf.CeilToInt(partialTime);
            actSeconds = roundedRestSeconds % 60;
            if (actSeconds != SSeconds)
            {
                SSeconds = actSeconds;
                Debug.Log(actSeconds);
                actPos = actPos - tomoveinSecs;
                AudioSlider.transform.localPosition = new Vector3(actPos *-1, AudioSlider.transform.localPosition.y, AudioSlider.transform.localPosition.z);
            }
        }
    }
}
