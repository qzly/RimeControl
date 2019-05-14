using RimeControl.Entitys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace RimeControl.Utils
{
    public class Tools
    {
        public static bool IsNumeric(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        public static bool IsInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        public static bool IsUnsign(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }

        public static bool IsTel(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return Regex.IsMatch(value, @"\d{3}-\d{8}|\d{4}-\d{7}");
        }

        /// <summary>
        /// 移除首尾的"或'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveFirstLastQuotationMarks(string value)
        {
            value = value.Trim();
            if (value.IndexOf('"') == 0)
            {
                value = value.Substring(1);
            }
            if (value.IndexOf('\'') == 0)
            {
                value = value.Substring(1);
            }
            if (value.LastIndexOf('"') == (value.Count() - 1))
            {
                value = value.Substring(0, value.Count() - 1);
            }
            if (value.LastIndexOf('\'') == (value.Count() - 1))
            {
                value = value.Substring(0, value.Count() - 1);
            }
            return value;
        }

        /// <summary>
        /// Rime颜色转换为标准颜色
        /// BGR转RGB
        /// </summary>
        /// <param name="color"> Rime颜色BGR </param>
        /// <returns></returns>
        public static System.Windows.Media.Color Bgr2Rgb(string color)
        {
            string[] s = color.Split('x');
            char[] chars = s[1].ToCharArray();
            Char t = chars[0];
            chars[0] = chars[4];
            chars[4] = t;

            t = chars[1];
            chars[1] = chars[5];
            chars[5] = t;

            string returnColor = new string(chars);

            Color drawColor = ColorTranslator.FromHtml("#" + returnColor);

            System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B);
            return mediaColor;
        }

        /// <summary>
        /// 标准颜色转换为Rime颜色
        /// RGB转BGR
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string Rgb2Bgr(System.Windows.Media.Color color)
        {
            string strColor = color.ToString();
            string[] s = strColor.Split('#');
            char[] charss = s[1].ToCharArray();
            char[] chars = { charss[2], charss[3], charss[4], charss[5], charss[6], charss[7] };


            char t = chars[0];
            chars[0] = chars[4];
            chars[4] = t;

            t = chars[1];
            chars[1] = chars[5];
            chars[5] = t;

            string returnColor = new string(chars);
            returnColor = "0x" + returnColor;
            return returnColor;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="yaml">YAML对象</param>
        /// <param name="key">key值</param>
        /// <returns>获取到的值或者null</returns>
        public static string GetYamlValue(Yaml yaml, string key)
        {
            //没有从 x.yaml 加载
            return yaml.Read(key);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="theYaml">YAML对象</param>
        /// <param name="key">key值</param>
        /// <param name="value">value值</param>
        /// <returns>是否成功</returns>
        public static bool SetYamlValue(Yaml theYaml, string key, string value)
        {
            key = "patch." + key;
            bool bolR;
            if (theYaml.FindNodeByKey(key) != null)
            {
                //如果存在，修改
                bolR = theYaml.Modify(key, value);
            }
            else
            {
                //如果不存在，添加
                bolR = theYaml.Add(key, value, false);
            }
            return bolR;
        }

        /// <summary>
        /// 获取所有currentSchema依赖或间接依赖的Schema
        /// </summary>
        /// <param name="listSchemaList">所有Schema列表</param>
        /// <param name="ylSchemas"></param>
        /// <param name="currentSchema">当前Schema</param>
        /// <returns></returns>
        public static void GetAllSchemaYl(ObservableCollection<Schema> listSchemaList, List<Schema> ylSchemas, Schema currentSchema)
        {
            //查询出所有 currentSchema 依赖的 Schema
            List<Schema> ylSchemasT = listSchemaList.Where(sc => currentSchema.Dependencies != null && currentSchema.Dependencies.Contains(sc.SchemaId)).ToList();
            //遍历
            foreach (Schema ylSchema in ylSchemasT)
            {
                if (ylSchemas.IndexOf(ylSchema) < 0)
                {
                    //添加到依赖列表
                    ylSchemas.Add(ylSchema);
                    //递归检查上级依赖
                    GetAllSchemaYl(listSchemaList, ylSchemas, ylSchema);
                }
            }
        }

        /// <summary>
        /// 获取所有依赖于或间接依赖于currentSchema的Schema
        /// </summary>
        /// <param name="listSchemaList">所有Schema列表</param>
        /// <param name="bylSchemas"></param>
        /// <param name="currentSchema">当前Schema</param>
        /// <returns></returns>
        public static void GetAllSchemaByl(ObservableCollection<Schema> listSchemaList, List<Schema> bylSchemas, Schema currentSchema)
        {
            //查询出所有 依赖于 currentSchma的 Schema
            List<Schema> bylSchemasT = listSchemaList.Where(sc => sc.Dependencies != null && sc.Dependencies.Contains(currentSchema.SchemaId)).ToList();
            //遍历
            foreach (Schema bylSchema in bylSchemasT)
            {
                if (bylSchemas.IndexOf(bylSchema) < 0)
                {
                    //添加到被依赖列表
                    bylSchemas.Add(bylSchema);
                    //递归检查下级被依赖
                    GetAllSchemaByl(listSchemaList, bylSchemas, bylSchema);
                }
            }
        }
    }
}
