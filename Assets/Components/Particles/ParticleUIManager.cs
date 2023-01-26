using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ParticleUIManager : MonoBehaviour
{
    public UnityEvent<int> numberOfParticlesChangedEvent;
    public UnityEvent<float> particleRadiusChangedEvent;
    public UnityEvent<float> generationRadiusChangedEvent;
    public UnityEvent<float> generationLengthChangedEvent;
    public UnityEvent<float> exclusionRadiusChangedEvent;

    public UnityEvent<float> velocityChangedEvent;
    public UnityEvent<float> travelLengthChangedEvent;
    public UnityEvent<bool> fixCameraChangedEvent;
    public UnityEvent<bool> showFloorChangedEvent;
    public UnityEvent<bool> showCrosshairsChangedEvent;
    public UnityEvent<bool> maskCoVChangedEvent;
    public UnityEvent<float> maskRadiusChangedEvent;
    public UnityEvent<bool> invertMaskChangedEvent;

    public UnityEvent<bool> disablePreviewChangedEvent;

    public UnityEvent loadedEvent;

    public int defaultNumberOfParticles;
    public float defaultParticleRadius;
    public float defaultGenerationRadius;
    public float defaultGenerationLength;
    public float defaultExclusionRadius;

    public float defaultVelocity;
    public float defaultTravelLength;
    public bool defaultFixCamera;
    public bool defaultShowFloor;
    public bool defaultShowCrosshairs;
    public bool defaultMaskCoV;
    public float defaultMaskRadius;
    public bool defaultInvertMask;

    public int numberOfParticles { private set; get; }
    public float particleRadius { private set; get; }
    public float generationRadius { private set; get; }
    public float generationLength { private set; get; }
    public float exclusionRadius { private set; get; }
    public float velocity { private set; get; }
    public float travelLength { private set; get; }
    public bool fixCamera { private set; get; }
    public bool showFloor { private set; get; }
    public bool showCrosshairs { private set; get; }
    public bool maskCoV { private set; get; }
    public float maskRadius { private set; get; }
    public bool invertMask { private set; get; }

    public TMPro.TMP_InputField NumberOfParticlesInputField;
    public TMPro.TMP_InputField ParticleRadiusInputField;
    public TMPro.TMP_InputField GenerationRadiusInputField;
    public TMPro.TMP_InputField GenerationLengthInputField;
    public TMPro.TMP_InputField ExclusionRadiusInputField;
    public TMPro.TMP_InputField VelocityInputField;
    public TMPro.TMP_InputField TravelLengthInputField;
    public Toggle FixCameraToggle;
    public Toggle ShowFloorToggle;
    public Toggle ShowCrosshairsToggle;
    public Toggle MaskCoVToggle;
    public TMPro.TMP_InputField MaskRadiusInputField;
    public Toggle InvertMaskToggle;

    public Toggle DisablePreviewToggle;


    public string serialize()
    {
        return numberOfParticles.ToString() + ","
            + particleRadius.ToString("0.000") + ","
            + generationRadius.ToString("0.000") + ","
            + generationLength.ToString("0.000") + ","
            + exclusionRadius.ToString("0.000") + ","
            + velocity.ToString("0.000") + ","
            + travelLength.ToString("0.000") + ","
            + (fixCamera ? "1" : "0") + ","
            + (showFloor ? "1" : "0") + ","
            + (showCrosshairs ? "1" : "0") + ","
            + (maskCoV ? "1" : "0") + ","
            + maskRadius.ToString("0.000") + ","
            + (invertMask ? "1" : "0");


    }

    public void deserialize(string values)
    {
        string[] separated = values.Split(',');
        if (separated.Length != 13)
            throw new Exception("Incorrect number of fields.");

        if (!int.TryParse(separated[0], out int numP))
            throw new Exception("Issue reading number of particles.");
        if (!float.TryParse(separated[1], out float pRad))
            throw new Exception("Issue reading particle Size.");
        if (!float.TryParse(separated[2], out float genRad))
            throw new Exception("Issue reading generation radius.");
        if (!float.TryParse(separated[3], out float genLen))
            throw new Exception("Issue reading generation length.");
        if (!float.TryParse(separated[4], out float exRad))
            throw new Exception("Issue reading exclusion radius.");
        if (!float.TryParse(separated[5], out float vel))
            throw new Exception("Issue reading velocity.");
        if (!float.TryParse(separated[6], out float tLen))
            throw new Exception("Issue reading travel length.");

        if (separated[7] != "0" && separated[7] != "1")
            throw new Exception("Issue reading fix camera.");
        bool fix = separated[7] == "1";

        if (separated[8] != "0" && separated[8] != "1")
            throw new Exception("Issue reading show floor.");
        bool floor = separated[8] == "1";

        if (separated[9] != "0" && separated[9] != "1")
            throw new Exception("Issue reading show crosshairs.");
        bool cross = separated[9] == "1";

        if (separated[10] != "0" && separated[10] != "1")
            throw new Exception("Issue reading mask CoV.");
        bool mask = separated[10] == "1";

        if (!float.TryParse(separated[11], out float mRad))
            throw new Exception("Issue reading mask radius.");

        if (separated[12] != "0" && separated[12] != "1")
            throw new Exception("Issue reading invert mask.");
        bool inv = separated[12] == "1";

        setFields(numP, pRad, genRad, genLen, exRad, vel, tLen, fix, floor, cross, mask, mRad, inv);
        loadedEvent?.Invoke();
    }

    void setFields(
        int numP, float pRad, float genRad, float genLen, float exRad,
        float vel, float tLen, bool fix, bool floor, bool cross,
        bool mask, float mRad, bool inv)
    {
        numberOfParticles = numP;
        NumberOfParticlesInputField.SetTextWithoutNotify(numberOfParticles.ToString());

        particleRadius = pRad;
        ParticleRadiusInputField.SetTextWithoutNotify(particleRadius.ToString("0.00"));

        generationRadius = genRad;
        GenerationRadiusInputField.SetTextWithoutNotify(generationRadius.ToString("0.00"));

        generationLength = genLen;
        GenerationLengthInputField.SetTextWithoutNotify(generationLength.ToString("0.00"));

        exclusionRadius = exRad;
        ExclusionRadiusInputField.SetTextWithoutNotify(exclusionRadius.ToString("0.00"));

        velocity = vel;
        VelocityInputField.SetTextWithoutNotify(velocity.ToString("0.00"));

        travelLength = tLen;
        TravelLengthInputField.SetTextWithoutNotify(travelLength.ToString("0.00"));

        fixCamera = fix;
        FixCameraToggle.SetIsOnWithoutNotify(fixCamera);

        showFloor = floor;
        ShowFloorToggle.SetIsOnWithoutNotify(showFloor);

        showCrosshairs = cross;
        ShowCrosshairsToggle.SetIsOnWithoutNotify(showCrosshairs);

        maskCoV = mask;
        MaskCoVToggle.SetIsOnWithoutNotify(maskCoV);

        maskRadius = mRad;
        MaskRadiusInputField.SetTextWithoutNotify(maskRadius.ToString("0.00"));

        invertMask = inv;
        InvertMaskToggle.SetIsOnWithoutNotify(invertMask);

        InvertMaskToggle.transform.parent.gameObject.SetActive(maskCoV);
        MaskRadiusInputField.transform.parent.gameObject.SetActive(maskCoV);

    }

    // Start is called before the first frame update
    void Start()
    {
        DisablePreviewToggle.SetIsOnWithoutNotify(false);

        NumberOfParticlesInputField.onValidateInput = OnValidatePositiveInteger;
        ParticleRadiusInputField.onValidateInput = OnValidateFloat;
        GenerationRadiusInputField.onValidateInput = OnValidateFloat;
        GenerationLengthInputField.onValidateInput = OnValidateFloat;
        ExclusionRadiusInputField.onValidateInput = OnValidateFloat;
        VelocityInputField.onValidateInput = OnValidateFloat;
        TravelLengthInputField.onValidateInput = OnValidateFloat;
        MaskRadiusInputField.onValidateInput = OnValidateFloat;


        NumberOfParticlesInputField.onEndEdit.AddListener((string value) =>
        {
            int asInt = int.Parse(value);
            numberOfParticles = asInt;
            numberOfParticlesChangedEvent?.Invoke(numberOfParticles);
        });
        ParticleRadiusInputField.onEndEdit.AddListener((string value) => {
            float asFloat = float.Parse(value);
            particleRadius = asFloat;
            particleRadiusChangedEvent?.Invoke(particleRadius);
        });
        GenerationRadiusInputField.onEndEdit.AddListener((string value) => {
            float asFloat = float.Parse(value);
            generationRadius = asFloat;
            generationRadiusChangedEvent?.Invoke(generationRadius);
        });
        GenerationLengthInputField.onEndEdit.AddListener((string value) => {
            float asFloat = float.Parse(value);
            generationLength = asFloat;
            generationLengthChangedEvent?.Invoke(generationLength);
        });
        ExclusionRadiusInputField.onEndEdit.AddListener((string value) => {
            float asFloat = float.Parse(value);
            exclusionRadius = asFloat;
            exclusionRadiusChangedEvent?.Invoke(exclusionRadius);
        });

        VelocityInputField.onEndEdit.AddListener((string value) => {
            float asFloat = float.Parse(value);
            velocity = asFloat;
            velocityChangedEvent?.Invoke(velocity);
        });
        TravelLengthInputField.onEndEdit.AddListener((string value) => {
            float asFloat = float.Parse(value);
            travelLength = asFloat;
            travelLengthChangedEvent?.Invoke(travelLength);
        });
        MaskRadiusInputField.onEndEdit.AddListener((string value) => {
            float asFloat = float.Parse(value);
            maskRadius = asFloat;
            maskRadiusChangedEvent?.Invoke(maskRadius);
        });

        FixCameraToggle.onValueChanged.AddListener((bool value) =>
        {
            fixCamera = value;
            fixCameraChangedEvent?.Invoke(fixCamera);
        });
        ShowFloorToggle.onValueChanged.AddListener((bool value) =>
        {
            showFloor = value;
            showFloorChangedEvent?.Invoke(showFloor);
        });
        ShowCrosshairsToggle.onValueChanged.AddListener((bool value) =>
        {
            showCrosshairs = value;
            showCrosshairsChangedEvent?.Invoke(showCrosshairs);
        });

        MaskCoVToggle.onValueChanged.AddListener((bool value) =>
        {
            maskCoV = value;
            maskCoVChangedEvent?.Invoke(maskCoV);
            InvertMaskToggle.transform.parent.gameObject.SetActive(maskCoV);
            MaskRadiusInputField.transform.parent.gameObject.SetActive(maskCoV);
        });
        InvertMaskToggle.onValueChanged.AddListener((bool value) =>
        {
            invertMask = value;
            invertMaskChangedEvent?.Invoke(invertMask);
        });

        DisablePreviewToggle.onValueChanged.AddListener((bool value) =>
        {
            disablePreviewChangedEvent?.Invoke(value);
        });

        setFields(defaultNumberOfParticles, 
            defaultParticleRadius, 
            defaultGenerationRadius,
            defaultGenerationLength,
            defaultExclusionRadius,
            defaultVelocity,
            defaultTravelLength,
            defaultFixCamera,
            defaultShowFloor,
            defaultShowCrosshairs,
            defaultMaskCoV,
            defaultMaskRadius,
            defaultInvertMask);
        Debug.Log(loadedEvent);
        loadedEvent?.Invoke();
    }

    private char OnValidatePositiveInteger(string text, int charIndex, char addedChar)
    {
        if (addedChar >= '0' && addedChar <= '9')
        {
            return addedChar;
        }
        return '\0';
    }

    private char OnValidateFloat(string text, int charIndex, char addedChar)
    {
        if (float.TryParse(text.Insert(charIndex, "" + addedChar), out float _))
        {
            return addedChar;
        }
        return '\0';
    }
}
