using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace VMClusterManager
{
    [System.Xml.Serialization.XmlRootAttribute()]
    public class Settings
    {
        //private static Settings instance;

        private string mainNodeName;

        [System.Xml.Serialization.XmlElementAttribute()]
        public string MainNodeName
        {
            get { return mainNodeName; }
            set { mainNodeName = value; }
        }



        [System.Xml.Serialization.XmlElementAttribute()]
        public static string SettingsFileName = "Settings.xml";
        

        public Settings()
            : this("localhost")
        {
        }

        public Settings(string _mainNodeName)
        {
            this.MainNodeName = _mainNodeName;
        }

        //public static Settings GetInstance()
        //{
        //    if (instance == null)
        //        instance = new Settings();
        //    return instance;
        //}

        public static Settings LoadFromFile()
        {
            return LoadFromFile(SettingsFileName);
        }

        public static Settings LoadFromFile(string FileName)
        {
            FileStream stream = new FileStream(FileName, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            Settings set = (Settings)serializer.Deserialize(stream);
            stream.Close();
            return set;
        }

        public void SaveToFile()
        {
            SaveToFile(SettingsFileName);
        }

        public void SaveToFile(string filename)
        {
            TextWriter tw = new StreamWriter(filename);
            XmlSerializer sr = new XmlSerializer(typeof(Settings));
            sr.Serialize(tw, this);
            tw.Close();
        }

        
    }
}
