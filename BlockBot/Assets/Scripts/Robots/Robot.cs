using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Robot : MonoBehaviour
{
    public string robotName;
    public List<Axis> axes = new List<Axis>();


    public void loadXMLData() {
        string fileName = "D:\\studia\\PracaInzynierska\\BlockBot\\Assets\\Robots\\kuka_kr_60\\kuka_kr_60.xml";
        XmlDocument Xdoc = new XmlDocument();
        Xdoc.Load(fileName);

        axes.Clear();
        
        foreach(XmlNode node in Xdoc.DocumentElement) {
            float offset = float.Parse(node["offset"].InnerText);
            float minAngle = float.Parse(node["min"].InnerText);
            float maxAngle = float.Parse(node["max"].InnerText);

            axes.Add(new Axis(offset, minAngle, maxAngle));
        }
    }

    public void saveXML() {
        string fileName = "D:\\studia\\PracaInzynierska\\BlockBot\\Assets\\Robots\\kuka_kr_60\\kuka_kr_60.xml";

        XmlDocument Xdoc = new XmlDocument();
        Xdoc.Load(fileName);

        int axisId = 0;
        foreach(XmlNode node in Xdoc.DocumentElement) {
            node["offset"].InnerText = axes[axisId].offset.ToString();
            node["min"].InnerText = axes[axisId].minAngle.ToString();
            node["max"].InnerText = axes[axisId].maxAngle.ToString();
            axisId++;
        }
        Xdoc.Save(fileName);
    }

    public class Axis {
        public Axis(float _offset, float _minAngle, float _maxAngle) {
            offset = _offset;
            minAngle = _minAngle;
            maxAngle = _maxAngle;
        }
        public float offset;
        public float minAngle;
        public float maxAngle;
    }
}