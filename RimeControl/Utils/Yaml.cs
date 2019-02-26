using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace RimeControl.Utils
{
    public class YAML
    {
        // 所有行
        private string[] lines;
        // 格式化为节点
        public List<Node> nodeList { get; set; }
        // 文件所在地址
        private readonly string path;

        /// <summary>
        /// Yaml
        /// </summary>
        /// <param name="path">yaml文件路径</param>
        public YAML(string path)
        {
            nodeList = new List<Node>();
            this.path = path;
            try
            {
                this.lines = File.ReadAllLines(path);
                Console.WriteLine(lines);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Trim() == "")
                    {
                        //Console.WriteLine("空白行，行号：" + (i + 1));
                        continue;
                    }
                    else if (line.Trim().Substring(0, 1) == "#")
                    {
                        //Console.WriteLine("注释行，行号：" + (i + 1));
                        Node nodeC = new Node
                        {
                            isComment = true,
                            name = line.Trim(),
                            space = FindPreSpace(line)
                        };
                        this.nodeList.Add(nodeC);
                        continue;
                    }

                    //在Rime的配置中参数存在奇怪的单独处理
                    //":": {commit: "："}
                    Node node = new Node
                    {
                        space = FindPreSpace(line)
                    };

                    Match match = Regex.Match(line, ":.*?\\\".*?:");
                    if (match.Success)
                    {
                        //匹配成功，表示是这种特殊情况
                        int index = match.Index + match.Length;
                        node.name = line.Substring(0, index - 1).Trim();
                        node.value = line.Substring(index).Trim();
                    }
                    else
                    {
                        //处理 raim 中 输入法方案 描述中的网址--以 https?: 开头
                        //https://zh-yue.wikipedia.org/wiki/%E9%A6%99%E6%B8%AF%E8%AA%9E%E8%A8%80%E5%AD%B8%E5%AD%B8%E6%9C%83%E7%B2%B5%E8%AA%9E%E6%8B%BC%E9%9F%B3%E6%96%B9%E6%A1%88
                        match = Regex.Match(line.Trim(), "^https?:");
                        if (match.Success)
                        {
                            //匹配成功，表示是这种特殊情况，对地址进行解码
                            node.name = System.Web.HttpUtility.UrlDecode(line.Trim());
                        }
                        else
                        {
                            string[] kv = Regex.Split(line, ":", RegexOptions.IgnoreCase);
                            // findPreSpace(line);
                            node.name = kv[0].Trim();

                            // 去除前后空白符
                            string fline = line.Trim();
                            int first = fline.IndexOf(':');
                            //冒号不存在的情况-----数组元素
                            if (first > -1)
                            {
                                node.value = first == fline.Length - 1 ? null : fline.Substring(first + 2, fline.Length - first - 2);
                            }
                            else
                            {
                                node.isArrayEl = true;
                            }
                        }
                        
                    }

                    if (node.name== "schema_id")
                    {

                    }
                    //设置父节点
                    node.parent = FindParent(node.space);
                    nodeList.Add(node);
                }

                this.Formatting();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public List<Node> getCloneNodeList()
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this.nodeList);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (List<Node>)formatter.Deserialize(objectStream);
            }
        }

        /// <summary>
        /// 添加值 允许key为多级 例如：spring.datasource.url
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isArrayEl"></param>
        /// <returns></returns>
        public bool Add(string key, string value, bool isArrayEl)
        {
            //为空情况下加注释
            if (this.nodeList.Count == 0)
            {
                Node ComNode = new Node
                {
                    isComment = true,
                    name = "#Custom File Create By RimeControl On " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                nodeList.Add(ComNode);

                ComNode = new Node
                {
                    isComment = true,
                    name = "#如果修改，请认真阅读 Rime定製指南 https://github.com/rime/home/wiki/CustomizationGuide 的要求！！"
                };
                nodeList.Add(ComNode);
            }

            Node node = null;//节点
            string tempKey = key;//存放临时key值
            bool isTop = false;//是否顶层
            while ((node = FindNodeByKey(tempKey)) == null)
            {
                int index = tempKey.LastIndexOf(".");
                if (index == -1)
                {
                    key = "." + key;
                    isTop = true;
                    break;
                }
                tempKey = tempKey.Substring(0, index);
            }
            string removeKey;//获取不存在的key
            if (isTop)
            {
                removeKey = key;
            }
            else
            {
                removeKey = key.Replace(tempKey, "");
            }
            //添加的key已经存在，返回false
            if (string.IsNullOrEmpty(removeKey))
            {
                return false;
            }
            //添加的key不存在，层级添加
            removeKey = removeKey.Substring(1);//去掉前的.
            string[] rKeys = removeKey.Split('.');//根据.分割
            Node pNode = node;
            for (int i = 0; i < rKeys.Count(); i++)
            {
                Node newNode = new Node
                {
                    name = rKeys[i],
                };
                if (i != 0 || !isTop)//不是顶层节点
                {
                    newNode.parent = pNode;
                    newNode.space = pNode.space + 2;
                    newNode.tier = pNode.tier + 1;
                }
                //最后
                if (i == rKeys.Count() - 1)
                {
                    newNode.isArrayEl = isArrayEl;
                    newNode.value = value;
                }
                this.nodeList.Add(newNode);
                pNode = newNode;
            }
            this.Formatting();
            return true;
        }

        /// <summary>
        /// 修改 允许key为多级 例如：spring.datasource.url
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Modify(string key, string value)
        {
            Node node = FindNodeByKey(key);
            if (node != null)
            {
                node.value = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除 允许key为多级 例如：spring.datasource.url
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            Node node = FindNodeByKey(key);
            if (node != null)
            {
                this.nodeList.Remove(node);
                return true;
            }
            return false;
        }

        // 读取值
        public string Read(string key)
        {
            Node node = FindNodeByKey(key);
            if (node != null)
            {
                return node.value;
            }
            return null;
        }

        // 保存到文件中
        public void Save()
        {
            string[] strContent = new string[this.nodeList.Count];
            int contentIndex = 0;

            //StreamWriter stream = File.CreateText(this.path);
            for (int i = 0; i < nodeList.Count; i++)
            {
                Node node = nodeList[i];
                StringBuilder sb = new StringBuilder();
                if (node.isComment)
                {
                    sb.Append(node.name);
                }
                else
                {
                    // 放入前置空格
                    for (int j = 0; j < node.tier; j++)
                    {
                        sb.Append("  ");
                    }
                    sb.Append(node.name);
                    if (!node.isArrayEl)
                    {
                        sb.Append(": ");
                        if (node.value != null)
                        {
                            sb.Append(node.value);
                        }
                    }
                }
                strContent[contentIndex] = sb.ToString();
                contentIndex++;
                //stream.WriteLine(sb.ToString());
            }
            File.WriteAllLines(this.path, strContent);
            //stream.Flush();
            //stream.Close();
        }


        // 根据key找节点
        public Node FindNodeByKey(string key)
        {
            string[] ks = key.Split('.');
            for (int i = 0; i < nodeList.Count; i++)
            {
                Node node = nodeList[i];
                if (node.name == ks[ks.Length - 1])
                {
                    // 判断父级
                    Node tem = node;
                    // 统计匹配到的次数
                    int count = 1;
                    for (int j = ks.Length - 2; j >= 0; j--)
                    {
                        try
                        {
                            if (tem.parent.name == ks[j])
                            {
                                count++;
                                // 继续检查父级
                                tem = tem.parent;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    if (count == ks.Length)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 格式化
        /// </summary>
        private void Formatting()
        {
            // 先找出根节点
            List<Node> parentNode = new List<Node>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                Node node = nodeList[i];
                if (node.parent == null)
                {
                    parentNode.Add(node);
                }
            }

            List<Node> fNodeList = new List<Node>();
            // 遍历根节点
            for (int i = 0; i < parentNode.Count; i++)
            {
                Node node = parentNode[i];
                fNodeList.Add(node);
                FindChildren(node, fNodeList);
            }

            Console.WriteLine("完成");

            // 指针指向格式化后的
            nodeList = fNodeList;
        }


        // 层级
        int tier = 0;
        // 查找孩子并进行分层
        private void FindChildren(Node node, List<Node> fNodeList)
        {
            // 当前层 默认第一级，根在外层进行操作
            tier++;

            for (int i = 0; i < nodeList.Count; i++)
            {
                Node item = nodeList[i];
                if (item.parent == node)
                {
                    item.tier = tier;
                    fNodeList.Add(item);
                    FindChildren(item, fNodeList);
                }
            }

            // 走出一层
            tier--;
        }

        //查找前缀空格数量
        private int FindPreSpace(string str)
        {
            List<char> chars = str.ToList();
            int count = 0;
            foreach (char c in chars)
            {
                if (c == ' ')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        // 根据缩进找上级
        private Node FindParent(int space)
        {

            if (nodeList.Count == 0)
            {
                return null;
            }
            else
            {
                // 倒着找上级
                for (int i = nodeList.Count - 1; i >= 0; i--)
                {
                    Node node = nodeList[i];
                    if (node.space < space)
                    {
                        return node;
                    }
                }
                // 如果没有找到 返回null
                return null;
            }
        }

        // 节点类
        public class Node
        {
            // 名称
            public string name { get; set; }
            // 值
            public string value { get; set; }
            // 父级
            public Node parent { get; set; }
            // 前缀空格
            public int space { get; set; }
            // 所属层级
            public int tier { get; set; }
            //是否注释行
            public bool isComment { get; set; }
            //是否数组元素
            public bool isArrayEl { get; set; }
        }
    }

}
