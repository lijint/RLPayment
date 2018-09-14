using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Xml;

namespace Landi.FrameWorks
{
    /// <summary>
    /// 注册表操作类
    /// </summary>
    public class RegHandler
    {
        /// <summary>
        /// 注册表域
        /// </summary>
        public enum RegDomain
        {
            /// <summary>
            /// 定义文档的类型（或类）以及与那些类型关联的属性。该字段读取 Windows 注册表基项 HKEY_CLASSES_ROOT。
            /// </summary>
            ClassesRoot = 0,
            /// <summary>
            /// 包含有关非用户特定的硬件的配置信息。该字段读取 Windows 注册表基项 HKEY_CURRENT_CONFIG。
            /// </summary>
            CurrentConfig = 1,
            /// <summary>
            /// 包含有关当前用户首选项的信息。该字段读取 Windows 注册表基项 HKEY_CURRENT_USER
            /// </summary>
            CurrentUser = 2,
            /// <summary>
            /// 包含动态注册表数据。该字段读取 Windows 注册表基项 HKEY_DYN_DATA。
            /// </summary>
            DynData = 3,
            /// <summary>
            /// 包含本地计算机的配置数据。该字段读取 Windows 注册表基项 HKEY_LOCAL_MACHINE。
            /// </summary>
            LocalMachine = 4,
            /// <summary>
            /// 包含软件组件的性能信息。该字段读取 Windows 注册表基项 HKEY_PERFORMANCE_DATA。
            /// </summary>
            PerformanceData = 5,
            /// <summary>
            /// 包含有关默认用户配置的信息。该字段读取 Windows 注册表基项 HKEY_USERS。
            /// </summary>
            Users = 6,
        }

        public class SubKeyInfo
        {
            public class InnerInfo
            {
                public string SubKeyPath;
                public string SubKeyName;
                public Dictionary<string, string> ValuePairs;

                public void AddValuePair(string key, string value)
                {
                    ValuePairs.Add(key, value);
                }

                public string GetValue(string key)
                {
                    return ValuePairs[key];
                }

                public void SetValue(string key, string value)
                {
                    ValuePairs[key] = value;
                }

                public void ClearValues()
                {
                    foreach (string i in ValuePairs.Keys)
                    {
                        ValuePairs[i] = "";
                    }
                }

                public void ClearValuePair()
                {
                    ValuePairs.Clear();
                }

                public InnerInfo()
                {
                    ValuePairs = new Dictionary<string, string>();
                }

            }

            private List<InnerInfo> innerInfos;

            public SubKeyInfo()
            {
                innerInfos = new List<InnerInfo>();
            }

            public void AddInnerInfo(InnerInfo info)
            {
                if (info != null)
                    innerInfos.Add(info);
            }

            public void ClearInnerInfos()
            {
                innerInfos.Clear();
            }

            public InnerInfo GetInnerInfoByName(string subKeyName)
            {
                for (int i = 0; i < innerInfos.Count; i++)
                {
                    if (subKeyName == innerInfos[i].SubKeyName)
                        return innerInfos[i];
                }
                return null;
            }

            public InnerInfo GetInnerInfoByPath(string subKeyPath)
            {
                for (int i = 0; i < innerInfos.Count; i++)
                {
                    if (subKeyPath == innerInfos[i].SubKeyPath)
                        return innerInfos[i];
                }
                return null;
            }

            public InnerInfo GetInnerInfoByIndex(int index)
            {
                if (index < GetInnerInfoCount())
                    return innerInfos[index];
                return null;
            }

            public void ClearValuePair()
            {
                for (int i = 0; i < innerInfos.Count; i++)
                {
                    innerInfos[i].ClearValuePair();
                }
            }

            public void ClearValues()
            {
                for (int i = 0; i < innerInfos.Count; i++)
                {
                    innerInfos[i].ClearValues();
                }
            }

            public int GetInnerInfoCount()
            {
                return innerInfos.Count;
            }
        }

        private RegistryKey entryKey;
        private string treeFile;
        private XmlNode rootNode;
        private bool isCreateRegTree = false;

        public RegHandler(RegDomain domain, string subKey, string regtreeFile)
            : this(domain, subKey)
        {
            treeFile = regtreeFile;
        }

        public RegHandler(RegDomain domain, string subKey)
        {
            switch (domain)
            {
                case RegDomain.LocalMachine:
                    entryKey = Registry.LocalMachine;
                    break;
                case RegDomain.ClassesRoot:
                    entryKey = Registry.ClassesRoot;
                    break;
                case RegDomain.CurrentConfig:
                    entryKey = Registry.CurrentConfig;
                    break;
                case RegDomain.CurrentUser:
                    entryKey = Registry.CurrentUser;
                    break;
                case RegDomain.DynData:
                    entryKey = Registry.DynData;
                    break;
                case RegDomain.PerformanceData:
                    entryKey = Registry.PerformanceData;
                    break;
                case RegDomain.Users:
                    entryKey = Registry.Users;
                    break;
                default:
                    entryKey = Registry.LocalMachine;
                    break;
            }
            entryKey = entryKey.OpenSubKey(subKey, true);
        }

        public RegHandler()
        {
            entryKey = Registry.LocalMachine;
            entryKey = entryKey.OpenSubKey("SOFTWARE", true);
        }

        public RegHandler(string regtreeFile)
            :this()
        {
            treeFile = regtreeFile;
        }

        private XmlNode getRootNode()
        {
            if (rootNode == null)
            {
                if (string.IsNullOrEmpty(treeFile) || !File.Exists(treeFile) || Path.GetExtension(treeFile) != ".xml")
                    return rootNode;
                XmlTextReader reader = new XmlTextReader(treeFile);
                XmlDocument document = new XmlDocument();
                document.Load(reader);
                rootNode = document.DocumentElement;
                return rootNode;
            }
            else
                return rootNode;
        }

        private bool validate()
        {
            if (entryKey == null)
                return false;
            else
                return true;
        }

        public bool CreateSubKey(string keyPath, Dictionary<string, object> valueInfo)
        {
            if (!validate())
                return false;
            if (string.IsNullOrEmpty(keyPath))
                return true;
            string[] paths = keyPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries); ;
            RegistryKey tmp = entryKey;
            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].Length > 0)
                    {
                        tmp = tmp.CreateSubKey(paths[i]);
                        if (tmp == null)
                            return false;
                    }
                }
            }
            if (tmp != null && valueInfo != null)
            {
                foreach (KeyValuePair<string, object> pair in valueInfo)
                {
                    tmp.SetValue(pair.Key.ToString(), pair.Value);
                }
            }
            tmp.Close();
            return true;
        }

        public bool CreateSubKey(string keyPath)
        {
            return CreateSubKey(keyPath, null);
        }

        public bool DeleteSubKey(string keyPath)
        {
            if (!validate())
                return false;
            if (string.IsNullOrEmpty(keyPath))
                return true;
            string[] paths = keyPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            RegistryKey tmp = entryKey;
            int i = 0;
            if (paths.Length > 0)
            {
                for (; i < paths.Length - 1; i++)
                {
                    if (paths[i].Length > 0)
                    {
                        tmp = tmp.OpenSubKey(paths[i], true);
                        if (tmp == null)
                            return false;
                    }
                }
            }
            tmp.DeleteSubKeyTree(paths[i]);
            tmp.Close();
            return true;
        }

        public bool SubKeyExist(string keyPath)
        {
            if (!validate())
                return false;
            if (string.IsNullOrEmpty(keyPath))
                return true;
            string[] paths = keyPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries); ;
            RegistryKey tmp = entryKey;
            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].Length > 0)
                    {
                        tmp = tmp.OpenSubKey(paths[i]);
                        if (tmp == null)
                            return false;
                    }
                }
            }
            return true;
        }

        public bool ValueExist(string keyPath, string key)
        {
            if (!validate())
                return false;
            if (string.IsNullOrEmpty(keyPath))
                return true;
            string[] paths = keyPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries); ;
            RegistryKey tmp = entryKey;
            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].Length > 0)
                    {
                        tmp = tmp.OpenSubKey(paths[i]);
                        if (tmp == null)
                            return false;
                    }
                }
            }
            if (tmp.GetValue(key, null) == null)
                return false;
            else
                return true;
        }

        public bool CreateRegTree(string fileName)
        {
            if (!validate())
                return false;
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName) || Path.GetExtension(fileName) != ".xml")
                return false;
            XmlTextReader reader = new XmlTextReader(fileName);
            XmlDocument document = new XmlDocument();
            document.Load(reader);

            if (document.DocumentElement == null)
                return false;
            isCreateRegTree = true;
            handleRegTree(document.DocumentElement, entryKey, true, null, null);
            isCreateRegTree = false;
            return true;
        }

        public bool CreateRegTree()
        {
            if (!validate())
                return false;
            if (getRootNode() == null)
                return false;

            isCreateRegTree = true;
            handleRegTree(getRootNode(), entryKey, true, null, null);
            isCreateRegTree = false;
            return true;
        }

        public bool WriteRegTree(string fileName)
        {
            if (!validate())
                return false;
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName) || Path.GetExtension(fileName) != ".xml")
                return false;
            XmlTextReader reader = new XmlTextReader(fileName);
            XmlDocument document = new XmlDocument();
            document.Load(reader);

            if (document.DocumentElement == null)
                return false;

            handleRegTree(document.DocumentElement, entryKey, true, null, null);
            return true;
        }

        public bool WriteRegTree()
        {
            if (!validate())
                return false;
            if (getRootNode() == null)
                return false;
            handleRegTree(getRootNode(), entryKey, true, null, null);
            return true;
        }

        public bool WriteRegTree(SubKeyInfo valueInfo)
        {
            if (!validate())
                return false;
            if (getRootNode() == null)
                return false;
            handleRegTree(getRootNode(), entryKey, true, valueInfo, null);
            return true;
        }

        public bool ReadRegTree(ref SubKeyInfo valueInfo)
        {
            if (!validate())
                return false;
            if (getRootNode() == null)
                return false;
            handleRegTree(getRootNode(), entryKey, false, null, valueInfo);
            if (valueInfo.GetInnerInfoCount() == 0)
            {
                valueInfo = null;
                return false;
            }
            else
                return true;
        }

        public bool ExistRegTree()
        {
            if (!validate())
                return false;
            if (getRootNode() == null)
                return false;
            SubKeyInfo info = new SubKeyInfo();
            return ReadRegTree(ref info);
        }

        public SubKeyInfo GetSubKeyInfosWithNoValue()
        {
            SubKeyInfo info = new SubKeyInfo();

            ReadRegTree(ref info);

            if (info != null)
                info.ClearValuePair();
            return info;
        }

        private void handleRegTree(XmlNode rootNode, RegistryKey currentKey, bool writeOrRead, SubKeyInfo writeValueInfo, SubKeyInfo readValueInfo)
        {
            SubKeyInfo.InnerInfo info = null;
            for (int i = 0; i < rootNode.ChildNodes.Count; i++)
            {
                XmlNode tmpNode = rootNode.ChildNodes.Item(i);
                if (tmpNode.Name == "ValuePair")
                {
                    string key = "";
                    string value = "";
                    bool read = true;

                    for (int j = 0; j < tmpNode.Attributes.Count; j++)
                    {
                        XmlAttribute attr = tmpNode.Attributes[j];
                        if (attr.Name == "Key")
                            key = attr.Value;
                        else if (attr.Name == "Value")
                            value = attr.Value;
                        else if (attr.Name == "Read")
                        {
                            if (int.Parse(attr.Value) == 0)
                                read = false;
                        }
                    }

                    if (isCreateRegTree)
                    {
                        if (currentKey.GetValue(key, null) == null)
                            currentKey.SetValue(key, value);
                    }
                    else
                    {
                        if (writeOrRead)
                        {
                            if (currentKey.GetValue(key, null) != null)
                            {
                                if (writeValueInfo != null)
                                {
                                    SubKeyInfo.InnerInfo innerInfo = writeValueInfo.GetInnerInfoByPath(currentKey.Name);
                                    if (innerInfo != null)
                                    {
                                        if (innerInfo.ValuePairs.ContainsKey(key))
                                            currentKey.SetValue(key, innerInfo.ValuePairs[key]);
                                    }
                                }
                                else
                                {
                                    currentKey.SetValue(key, value);
                                }
                            }
                        }
                        else
                        {
                            if (read && currentKey.GetValue(key, null) != null)
                            {
                                if (info == null)
                                {
                                    info = new SubKeyInfo.InnerInfo();
                                    info.SubKeyPath = currentKey.Name;
                                    info.SubKeyName = currentKey.Name.Substring(currentKey.Name.LastIndexOf('\\') + 1);
                                }
                                info.AddValuePair(key, currentKey.GetValue(key).ToString());
                            }
                        }
                    }
                }
                else if (tmpNode.Name == "SubKey")
                {
                    string path = "";
                    int loopTime = 1;
                    for (int j = 0; j < tmpNode.Attributes.Count; j++)
                    {
                        XmlAttribute attr = tmpNode.Attributes[j];
                        if (attr.Name == "Name")
                            path = attr.Value;
                        else if (attr.Name == "LoopTime")
                        {
                            if (int.TryParse(attr.Value, out loopTime) && loopTime <= 0)
                                loopTime = 1;
                        }

                    }

                    if (isCreateRegTree)
                    {
                        if (loopTime == 1)
                            handleRegTree(tmpNode, currentKey.CreateSubKey(path), writeOrRead, writeValueInfo, readValueInfo);
                        else
                            for (int k = 0; k < loopTime; k++)
                                handleRegTree(tmpNode, currentKey.CreateSubKey(path + k.ToString()), writeOrRead, writeValueInfo, readValueInfo);

                    }
                    else
                    {
                        if (loopTime == 1)
                        {
                            if (containsSubKey(currentKey, path))
                                handleRegTree(tmpNode, currentKey.OpenSubKey(path, true), writeOrRead, writeValueInfo, readValueInfo);
                        }
                        else
                        {
                            for (int k = 0; k < loopTime; k++)
                                if (containsSubKey(currentKey, path + k.ToString()))
                                    handleRegTree(tmpNode, currentKey.OpenSubKey(path + k.ToString(), true), writeOrRead, writeValueInfo, readValueInfo);
                        }
                    }
                }
            }
            if (info != null && readValueInfo != null)
                readValueInfo.AddInnerInfo(info);
        }

        private bool containsSubKey(RegistryKey key, string subKeyName)
        {
            if (key == null || string.IsNullOrEmpty(subKeyName))
                return false;
            string[] names = key.GetSubKeyNames();
            for (int i = 0; i < names.Length; i++)
                if (subKeyName == names[i])
                    return true;
            return false;
        }

        public RegistryKey OpenSubKey(string keyPath)
        {
            if (!validate())
                return null;
            RegistryKey tmp = entryKey;
            if (string.IsNullOrEmpty(keyPath))
                return tmp;
            string[] paths = keyPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].Length > 0)
                    {
                        tmp = tmp.OpenSubKey(paths[i], true);
                        if (tmp == null)
                            break;
                    }
                }
            }
            return tmp;
        }

        public bool SetValue(string keyPath, string key, object value)
        {
            RegistryKey tmp = OpenSubKey(keyPath);
            if (tmp != null)
            {
                tmp.SetValue(key, value);
                tmp.Close();
                return true;
            }
            else
                return false;
        }

        public object GetValue(string keyPath, string key)
        {
            RegistryKey tmp = OpenSubKey(keyPath);
            if (tmp != null)
            {
                return tmp.GetValue(key);
            }
            else
                return null;
        }

    }
}
