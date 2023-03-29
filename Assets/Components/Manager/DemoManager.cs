using GPUInstancer;
using SimpleFileBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public List<GPUInstancerPrefab> particlePrefab;
    public GPUInstancerPrefabManager previewPrefabManager;
    public GPUInstancerPrefabManager headsetPrefabManager;

    public ParticleManager HeadsetManager;
    public ParticleManager PreviewManager;
    public ParticleUIManager UIManager;

    public GameObject particlePreviewer;
    public GameObject particleWindow;

    public TMPro.TextMeshProUGUI Error;
    public Button Dismiss;

    public Button SendToHeadset;
    public Button LoadSettings;
    public Button SaveSettings;

    public Button QuitApplication;

    private string debugPath = "./SendToCooperIfAnythingGoesWrong.log";
    StreamWriter writer;

    static SerialPort _serialPort;

    // Start is called before the first frame update
    void Awake()
    {
        _serialPort = new SerialPort();
        _serialPort.PortName = "COM3";//Set your board COM
        _serialPort.BaudRate = 9600;
        _serialPort.Open();

        writer =  new StreamWriter(debugPath, false);
        Application.logMessageReceived += (string logString, string stackTrace, LogType logType) => {
 
            writer.WriteLine(logString);
            if (logType == LogType.Error || logType == LogType.Exception)
                writer.WriteLine(stackTrace);

            writer.Flush();
        };

        Dismiss.onClick.AddListener(() =>
        {
            Error.gameObject.transform.parent.gameObject.SetActive(false);
        });

        Error.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void Start()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Tests", ".test"));
        SaveSettings.onClick.AddListener(onSaveSettingsClicked);
        LoadSettings.onClick.AddListener(onLoadSettingsClicked);
        QuitApplication.onClick.AddListener(onQuitClicked);

        SendToHeadset.onClick.AddListener(() =>
        {
            _serialPort.WriteLine("Hello");

            HeadsetManager.Reset(UIManager.numberOfParticles, UIManager.particleRadius,
                UIManager.generationRadius, UIManager.generationLength, UIManager.exclusionRadius,
                UIManager.velocity, UIManager.travelLength, UIManager.fixCamera, UIManager.showFloor, 
                UIManager.showCrosshairs, UIManager.maskCoV, UIManager.maskRadius, UIManager.invertMask);
        });

        UIManager.loadedEvent.AddListener(() =>
        {
            PreviewManager.Reset(UIManager.numberOfParticles, UIManager.particleRadius,
                UIManager.generationRadius, UIManager.generationLength, UIManager.exclusionRadius,
                UIManager.velocity, UIManager.travelLength, UIManager.fixCamera, UIManager.showFloor,
                UIManager.showCrosshairs, UIManager.maskCoV, UIManager.maskRadius, UIManager.invertMask);
        });

        UIManager.numberOfParticlesChangedEvent.AddListener((int num) =>
        {
            PreviewManager.SetNumParticles(num);
        });

        UIManager.particleRadiusChangedEvent.AddListener((float rad) =>
        {
            PreviewManager.SetParticleRadius(rad);
        });

        UIManager.generationRadiusChangedEvent.AddListener((float rad) =>
        {
            PreviewManager.SetGenerationRadius(rad);
        });

        UIManager.generationLengthChangedEvent.AddListener((float len) =>
        {
            PreviewManager.SetGenerationLength(len);
        });

        UIManager.exclusionRadiusChangedEvent.AddListener((float rad) =>
        {
            PreviewManager.SetExclusionRadius(rad);
        });

        UIManager.velocityChangedEvent.AddListener((float vel) =>
        {
            PreviewManager.SetVelocity(vel);
        });

        UIManager.travelLengthChangedEvent.AddListener((float len) =>
        {
            PreviewManager.SetTravelLength(len);
        });

        UIManager.fixCameraChangedEvent.AddListener((bool fix) =>
        {
            PreviewManager.SetFixCamera(fix);
        });

        UIManager.showFloorChangedEvent.AddListener((bool show) =>
        {
            PreviewManager.SetShowFloor(show);
        });

        UIManager.showCrosshairsChangedEvent.AddListener((bool show) =>
        {
            PreviewManager.SetShowCrosshairs(show);
        });

        UIManager.maskCoVChangedEvent.AddListener((bool show) =>
        {
            PreviewManager.SetShowMask(show);
        });

        UIManager.maskRadiusChangedEvent.AddListener((float rad) =>
        {
            PreviewManager.SetMaskRadius(rad);
        });

        UIManager.invertMaskChangedEvent.AddListener((bool inv) =>
        {
            PreviewManager.SetInvertMask(inv);
        });

        UIManager.disablePreviewChangedEvent.AddListener((bool toggle) =>
        {
            particlePreviewer.SetActive(!toggle);
            particleWindow.SetActive(!toggle);
        });

        if (previewPrefabManager != null && previewPrefabManager.gameObject.activeSelf && previewPrefabManager.enabled)
        {
            GPUInstancerAPI.InitializeGPUInstancer(previewPrefabManager);
        }
        if (headsetPrefabManager != null && headsetPrefabManager.gameObject.activeSelf && headsetPrefabManager.enabled)
        {
            GPUInstancerAPI.InitializeGPUInstancer(headsetPrefabManager);

        }
        HeadsetManager.Reset(UIManager.numberOfParticles, UIManager.particleRadius,
                UIManager.generationRadius, UIManager.generationLength, UIManager.exclusionRadius,
                UIManager.velocity, UIManager.travelLength, UIManager.fixCamera, UIManager.showFloor,
                UIManager.showCrosshairs, UIManager.maskCoV, UIManager.maskRadius, UIManager.invertMask);
    }

    void onLoadSettingsClicked()
    {
        FileBrowser.ShowLoadDialog(
            onFileLoaded,
            () => { },
            FileBrowser.PickMode.Files, false, null, null, "Load previous test", "Select");
    }

    void onSaveSettingsClicked()
    {
        FileBrowser.ShowSaveDialog(
            onFilePicked,
            () => { },
            FileBrowser.PickMode.Files, false, null, null, "Load previous test", "Select");
    }

    void onQuitClicked()
    {
        Application.Quit();
    }

    void onFileLoaded(string[] paths)
    {
        string text = File.ReadAllText(paths[0]);
        string[] split = text.Split(',');

        try
        {
            UIManager.deserialize(text);
        }
        catch (Exception e)
        {
            Error.gameObject.transform.parent.gameObject.SetActive(true);
            Error.text = "Issue reading test file: \n\n" + e.Message;
        }
    }

    void onFilePicked(string[] paths)
    {
        File.WriteAllText(paths[0], UIManager.serialize());
    }
}
