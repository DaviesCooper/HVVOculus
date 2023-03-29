using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SerialManager : MonoBehaviour
{

    static SerialPort _serialPort;

    public TMPro.TMP_Dropdown Dropdown;
    public Button connectButton;


    // Start is called before the first frame update
    void Start()
    {
        string[] portNames = SerialPort.GetPortNames();
        Dropdown.ClearOptions();
        //creates a new list
        List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < portNames.Length; i++)
        {
            //create a new item for list
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
            newData.text = portNames[i];
            data.Add(newData);
        }
        //populate TMPro dropdown with  List
        Dropdown.AddOptions(data);

        connectButton.onClick.AddListener(connect);
    }

    public void connect()
    {
        if (Dropdown.options.Count > 0)
        {
            try
            {
                _serialPort = new SerialPort();
                string name = Dropdown.options[Dropdown.value].text;
                _serialPort.PortName = name;
                _serialPort.BaudRate = 9600;
            }
            catch { Debug.LogError("Unable to connect to arduino using " + Dropdown.options[Dropdown.value].text + "."); }
        }
    }

    public void sendSignal()
    {
        try
        {
            _serialPort.Open();
            _serialPort.WriteLine("A");
            _serialPort.Close();
        }
        catch
        {
            Debug.LogError("Unable to send signal to arduino.");
        }
    }
}
