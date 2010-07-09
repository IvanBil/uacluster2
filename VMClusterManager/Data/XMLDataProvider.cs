using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace VMClusterManager.Data
{
    public class XMLDataProvider : IDataProvider
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public XMLDataProvider(string path)
        {
            filePath = path;
        }

        #region IDataProvider Members

        public VMHostGroup GetHostTree(string path)
        {
            //XmlDocument doc = new XmlDocument();
            //XmlReader reader;
            //XElement el = new XElement(); 
            return null;
        }

  
        #endregion

        #region IDataProvider Members


        public void Save(object o)
        {
            XmlSerializer s = new XmlSerializer(typeof(VMGroup));
            TextWriter w = new StreamWriter(this.FilePath);
            s.Serialize(w, o);
            w.Close();
        }

        #endregion
    }
}
