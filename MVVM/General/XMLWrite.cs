using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MVVM.Models;
namespace MVVM.General
{
    class XMLWrite
    {
        public class saveData
        {
            public string title;
            public LayerVM[] layerArray;
        }

        public static void WriteXML(string _fileName, LayerVM[] _layerArray)
        {
            saveData overview = new saveData();
            overview.title = _fileName;
            overview.layerArray = _layerArray;
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(saveData));

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, overview);
            file.Close();
        }
    }
}
