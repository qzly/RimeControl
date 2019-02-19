using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace RimeControl.Utils
{
    public class YamlUtil
    {

        #region 序列化操作  
        
        public static void Serializer(string filePath, Object obj)
        {
            StreamWriter writer = File.CreateText(filePath);
            Serializer yamlSerializer = new Serializer();
            yamlSerializer.Serialize(writer, obj);
            writer.Close();
        }

        public static void Serializer<T>(string filePath, T obj)
        {
            StreamWriter writer = File.CreateText(filePath);
            Serializer yamlSerializer = new Serializer();
            yamlSerializer.Serialize(writer, obj);
            writer.Close();
        }

        #endregion


        #region 反序列化操作  

        public static YamlMappingNode DeserializerToNode(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            StreamReader reader = File.OpenText(filePath);
            // Load the stream
            var yaml = new YamlStream();
            yaml.Load(reader);

            // Examine the stream
            YamlMappingNode mapping =
                (YamlMappingNode)yaml.Documents[0].RootNode;

            return mapping;
        }

        public static Object Deserializer(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            StreamReader reader = File.OpenText(filePath);
            Deserializer yamlDeserializer = new Deserializer();

            //读取持久化对象  
            Object obj = yamlDeserializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        public static T Deserializer<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            StreamReader reader = File.OpenText(filePath);
            Deserializer yamlDeserializer = new Deserializer();

            //读取持久化对象  
            T info = yamlDeserializer.Deserialize<T>(reader);
            reader.Close();
            return info;
        }

        #endregion
    }
}
