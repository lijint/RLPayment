using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Landi.FrameWorks
{
    /// <summary>
    /// 不自动保存文件,需要手动调用SaveXMLFile函数
    /// </summary>
    public class XMLConfig
    {
        private XmlDocument _xmlDoc;
        private XmlNode _rootNode;
        private string _fileName;

        public XMLConfig()
        {
            _xmlDoc = new XmlDocument();
        }

        public XMLConfig(string xmlFile)
        {
            _xmlDoc = new XmlDocument();
            _fileName = xmlFile;
            LoadXMLFile(_fileName);
        }

        public XMLConfig(Stream stream)
        {
            try
            {
                _xmlDoc.Load(stream);
            }
            catch(Exception e)
            {
                throw new Exception("xml stream load failed! " + e.Message);
            }
        }


        #region 初始化相关操作
        public Boolean LoadXMLFile(string xmlFile)
        {
            Boolean result = false;
            try
            {
                _xmlDoc.Load(xmlFile);
                _rootNode = _xmlDoc.DocumentElement;
                result = true;
            }
            catch(Exception e) 
            {
                throw new Exception("xml file parse failed! file[" + xmlFile + "]" + e.Message);
                
            }
            return result;
        }

        public Boolean loadXML(string xmlStr)
        {
            Boolean result = false;
            if (_rootNode != null)
            {
                _rootNode = null;
            }
            try
            {
                _xmlDoc.LoadXml(xmlStr);
                _rootNode = _xmlDoc.DocumentElement;
                result = true;
            }
            catch (System.Exception e)
            {
                throw new Exception("xml file parse failed! XML[" + xmlStr + "]" + e.Message);
            }
            return result;
        }

        public void SaveXMLFile(string xmlfile)
        {
            this._xmlDoc.Save(xmlfile);
        }

        public void SaveXMLFile()
        {
            this._xmlDoc.Save(_fileName);
        }

        public string GetXML()
        {
            return this._xmlDoc.OuterXml;
        }

        public XmlDocument GetXMLObj()
        {
            return this._xmlDoc;
        }

        public Boolean SetRootNodePath(string NodePath)
        {
            this._rootNode = this._xmlDoc.SelectSingleNode(NodePath);
            return this._rootNode != null;
        }

        #endregion

        #region 节点操作
        public XmlNode GetNode(string nodePath)
        {
            return _xmlDoc.SelectSingleNode(nodePath);
        }

        public int GetNodeElementCount(string nodePath)
        {
            XmlNode Node;
            Node = _xmlDoc.SelectSingleNode(nodePath);
            if (Node != null)
            {
                return Node.ChildNodes.Count;
            }
            else
            {
                throw new Exception("xml file parse failed! not found root node " + nodePath);
            }
        }

        public XmlNode GetNodeElementById(string nodePath, int itemId)
        {
            XmlNode rootNode;
            rootNode = _xmlDoc.SelectSingleNode(nodePath);
            if (rootNode == null)
            {
                throw new Exception("xml file parse failed! not found root node " + nodePath);
                //return null;
            }
            else if (this.GetNodeElementCount(nodePath) - 1 < itemId)
            {
                throw new Exception("xml node element itemid step over!");
                //return null;
            }
            return rootNode.ChildNodes.Item(itemId);
        }

        public XmlNode GetNodeElementByAttrValue(string nodePath, string attrName, string attrValue)
        {
            XmlNode rootNode, perNode, attrNode;
            bool isAttrExist = false;
            rootNode = _xmlDoc.SelectSingleNode(nodePath);
            if (rootNode == null)
            {
                throw new Exception("xml file parse failed! not found root node " + nodePath);
            }
            else 
            {
                for (int iPer = 0; iPer <= GetNodeElementCount(nodePath) - 1; iPer++ )
                {
                    perNode = GetNodeElementById(nodePath, iPer);
                    attrNode = perNode.Attributes.GetNamedItem(attrName);
                    if (attrNode != null)
                    {
                        isAttrExist = true;
                        if (attrNode.InnerText == attrValue) return perNode;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (isAttrExist == false)
                {
                    throw new Exception("xml file parse failed! not found attribute " + attrName);
                }
            }

            return null;
        }

        public Boolean HasNode(string nodePath)
        {
            return _xmlDoc.SelectSingleNode(nodePath) != null;
        }

        public Boolean CreateAttribute(string nodePath, string arrtName, string value)
        {
            XmlNode nodeIntf, newNodeIntf;
            if (!HasNode(nodePath))
            {
                return false;
            }
            nodeIntf = GetNode(nodePath);
            newNodeIntf = _xmlDoc.CreateNode(XmlNodeType.Attribute,arrtName,value);
            nodeIntf.Attributes.SetNamedItem(newNodeIntf);
            //SaveXMLFile(_fileName);
            return true;
        }

        public Boolean DeleteAttribute(string nodePath, string attrName)
        {
            XmlNode nodeIntf;
            if (!HasNode(nodePath))
            {
                return false;
            }
            nodeIntf = GetNode(nodePath);
            if (nodeIntf.Attributes.GetNamedItem(attrName) == null)
            {
                return false;
            }
            nodeIntf.Attributes.RemoveNamedItem(attrName);
            //SaveXMLFile(_fileName);
            return true;
        }

        public Boolean CreateNode(string nodePath, string nodeName, string value)
        {
            XmlNode nodeIntf, newNodeIntf;
            if (!HasNode(nodePath))
            {
                return false;
            }
            nodeIntf = GetNode(nodePath);
            newNodeIntf = _xmlDoc.CreateNode(XmlNodeType.Element, nodeName, "");
            newNodeIntf.InnerText = value;
            nodeIntf.AppendChild(newNodeIntf);
            //SaveXMLFile(_fileName);
            return true;
        }

        public XmlNode CreateNode(XmlNode parentNode, string nodeName, string value)
        {
            XmlNode result;
            result = _xmlDoc.CreateNode(XmlNodeType.Element, nodeName, "");
            result.InnerText = value;
            parentNode.AppendChild(result);
            //SaveXMLFile(_fileName);
            return result;
        }

        public Boolean DeleteNode(string nodePath, string nodeName)
        {
            XmlNode nodeIntf, childNodeIntf;
            if (!HasNode(nodePath))
            {
                return false;
            }
            //if (!HasNode(string.Format("%s/%s",nodePath,nodeName)))
            if (!HasNode(nodePath + "/" + nodeName))
            {
                return false;
            }
            nodeIntf = GetNode(nodePath);
            childNodeIntf = nodeIntf.SelectSingleNode(nodeName);
            nodeIntf.RemoveChild(childNodeIntf);
            //SaveXMLFile(_fileName);
            return true;
        }
        #endregion

        #region 值操作
        public string GetNodeValue(string nodePath)
        {
            XmlNode perNode;
            string result;
            perNode = GetNode(nodePath);
            if (perNode != null)
            {
                result = perNode.InnerText;
            }
            else
            {
                result = "";
            }
            perNode = null;
            return result;
        }

        public void SetNodeValue(string nodePath, string nodeValue)
        {
            XmlNode nodeIntf;
            if (!HasNode(nodePath))
            {
                return;
            }
            nodeIntf = GetNode(nodePath);
            nodeIntf.InnerText = nodeValue;
            //SaveXMLFile(_fileName);
        }

        public string GetAttributeValue(string nodePath, string attrName)
        {
            string result = "";
            XmlNode perNode, attrNode;
            perNode = GetNode(nodePath);
            if (perNode != null)
            {
                attrNode = perNode.Attributes.GetNamedItem(attrName);
                if (attrNode != null)
                {
                    result = attrNode.InnerText;
                    attrNode = null;
                }
                perNode = null;
            }
            return result;
        }

        public string GetAttributeValue(XmlNode xmlNode, string attrName)
        {
            string result = "";
            XmlNode attrNode;
            if (xmlNode != null)
            {
                attrNode = xmlNode.Attributes.GetNamedItem(attrName);
                if (attrNode != null)
                {
                    result = attrNode.InnerText;
                    attrNode = null;
                }
                xmlNode = null;
            }
            return result;
        }

        public void SetAttributeValue(string nodePath, string attrName, string attrValue)
        {
            XmlNode nodeIntf, attrNodeIntf;
            if (!HasNode(nodePath))
            {
                return;
            }
            nodeIntf = GetNode(nodePath);
            if (nodeIntf.Attributes.GetNamedItem(attrName) == null)
            {
                return;
            }
            attrNodeIntf = nodeIntf.Attributes.GetNamedItem(attrName);
            attrNodeIntf.InnerText  = attrValue;
            //SaveXMLFile(_fileName);
        }

        public void SetAttributeValue(XmlNode xmlNode, string attrName, string attrValue)
        {
            XmlNode attrNodeIntf;
            if (xmlNode.Attributes.GetNamedItem(attrName) == null)
            {
                return;
            }
            attrNodeIntf = xmlNode.Attributes.GetNamedItem(attrName);
            attrNodeIntf.InnerText = attrValue;
            //SaveXMLFile(_fileName);
        }

        public Boolean HasAttribute(string nodePath, string attributeName)
        {
            Boolean result = false;
            XmlNode nodeIntf;
            if (!HasNode(nodePath))
            {
                return false;
            }
            nodeIntf = GetNode(nodePath);
            result = nodeIntf.Attributes.GetNamedItem(attributeName) != null;
            return result;
        }
        #endregion

        #region 克隆复制
        public XMLConfig cloneXMLObject()
        {
            XMLConfig result;
            result = new XMLConfig();
            result.loadXML(this.GetXML());
            return result;
        }

        public XMLConfig cloneXMLObjectFromNode(string nodePath, Boolean deep)
        {
            XmlNode xmlNode;
            XMLConfig result = null;
            if (this.HasNode(nodePath))
            {
                result = new XMLConfig();
                xmlNode = GetNode(nodePath).CloneNode(deep);
                result.loadXML(xmlNode.OuterXml);
                xmlNode = null;
            }
            return result;
        }
        #endregion
    }
    

}
