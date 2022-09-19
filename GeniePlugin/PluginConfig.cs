using GeniePlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace GeniePlugin
{
  public class PluginConfig
  {
    private XmlDocument _configFile;
    private IHost _host;
    private IPlugin _plugin;
    private string _configName;
    private string _fileName;
    private string _filePath;
    private string _fullPath;

    public PluginConfig(IHost Host, IPlugin PlugIn)
    {
      _host = Host;
      this._plugin = PlugIn;
      this._configFile = new XmlDocument();
      this._configName = this._plugin.Name.Replace(" ", "_");
      this._fileName = this._configName + ".xml";
      this._filePath = _host.get_Variable("PluginPath");
      if (this._filePath != "\\")
        this._filePath += "\\";
      this._fullPath = this._filePath + this._fileName;
    }

    public void SaveConfig()
    {
      if (this._configFile == null)
        return;
      try
      {
        this._configFile.Save(this._fullPath);
      }
      catch (Exception ex)
      {
        _host.EchoText(this._plugin.Name + ".SaveConfig: " + ex.Message + "\n");
      }
    }

    public void OpenConfig()
    {
      if (this._configFile == null)
        this._configFile = new XmlDocument();
      try
      {
        this._configFile.Load(this._fullPath);
      }
      catch
      {
        if (Directory.Exists(this._filePath))
        {
          this._configFile = new XmlDocument();
          this._configFile.AppendChild((XmlNode) this._configFile.CreateXmlDeclaration("1.0", (string) null, (string) null));
          this._configFile.AppendChild((XmlNode) this._configFile.CreateElement(this._configName));
        }
        else
          _host.EchoText(this._plugin.Name + ".OpenConfig: " + this._filePath + " is not a valid path.\n");
      }
    }

    public Dictionary<string, string> GetSectionList() => this.GetSectionList("");

    public Dictionary<string, string> GetSectionList(string sectionName)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (this._configFile == null)
        return (Dictionary<string, string>) null;
      sectionName = !(sectionName != string.Empty) ? "/" + this._configName + "/" : sectionName + "/";
      try
      {
        XPathNodeIterator xpathNodeIterator = this._configFile.CreateNavigator().Select(sectionName + "*");
        while (xpathNodeIterator.MoveNext())
        {
          if (xpathNodeIterator.Current.IsNode)
            dictionary.Add(xpathNodeIterator.Current.Name, sectionName + xpathNodeIterator.Current.Name);
        }
      }
      catch (Exception ex)
      {
        _host.EchoText(this._plugin.Name + ".GetSectionList: " + ex.Message + "\n");
        return (Dictionary<string, string>) null;
      }
      return dictionary;
    }

    public XmlNode GetSection(string sectionName) => this.GetKey("/" + this._configName + "/" + sectionName);

    public XmlNode GetKey(string sectionName, string keyName) => this.GetKey(this.GetSection(sectionName), keyName);

    public XmlNode GetKey(string keyName) => this._configFile == null ? (XmlNode) null : this._configFile.SelectSingleNode(keyName);

    public XmlNode GetKey(XmlNode node, string keyName) => node.SelectSingleNode(keyName);

    public string GetKeyValue(string sectionName, string keyName, string defValue)
    {
      XmlNode key = this.GetKey(sectionName, keyName);
      return key != null ? this.GetKeyValue(key, defValue) : defValue;
    }

    public string GetKeyValue(XmlNode Key, string defValue)
    {
      try
      {
        return Key.InnerText;
      }
      catch
      {
        return defValue;
      }
    }

    public string GetKeyValue(XmlNode Section, string keyName, string defValue)
    {
      try
      {
        return Section.SelectSingleNode(keyName).InnerText;
      }
      catch
      {
        return defValue;
      }
    }

    public string GetKeyValue(string Key, string defValue)
    {
      try
      {
        return this._configFile.SelectSingleNode(Key).InnerText;
      }
      catch
      {
        return defValue;
      }
    }

    public void SetKeyValue(XmlNode Section, string key, string Value)
    {
      try
      {
        Section.SelectSingleNode(key).InnerText = Value;
      }
      catch
      {
        if (this._configFile == null)
          return;
        XmlElement element = this._configFile.CreateElement(key);
        Section.AppendChild((XmlNode) element);
        element.AppendChild((XmlNode) this._configFile.CreateTextNode(Value));
      }
    }

    public void SetKeyValue(string Key, string Value)
    {
      try
      {
        this._configFile.SelectSingleNode(Key).InnerText = Value;
      }
      catch
      {
        if (this._configFile == null)
          return;
        this._configFile.CreateElement(Key).AppendChild((XmlNode) this._configFile.CreateTextNode(Value));
      }
    }

    public void SetAttribute(XmlNode node, string attribute, string value)
    {
      XmlAttribute attribute1 = this._configFile.CreateAttribute(attribute);
      attribute1.Value = value;
      ((XmlElement) node).SetAttributeNode(attribute1);
    }

    public string GetAttribute(XmlNode node, string attribute) => node.Attributes[attribute].Value;

    public XmlNode AddSection(string sectionName)
    {
      XmlNode element = (XmlNode) this._configFile.CreateElement(sectionName);
      this._configFile.SelectSingleNode(this._configName).AppendChild(element);
      return element;
    }

    public XmlNode GetComplexKey(XmlNode Section, string key, string name) => Section.SelectSingleNode(key + "[@name='" + name + "']");

    public XmlNode AddComplexKey(
      XmlNode Section,
      string key,
      string name,
      Dictionary<string, string> attributes)
    {
      XmlNode element = (XmlNode) this._configFile.CreateElement(key);
      XmlAttribute attribute1 = this._configFile.CreateAttribute(nameof (name));
      attribute1.Value = name;
      ((XmlElement) element).SetAttributeNode(attribute1);
      foreach (string key1 in attributes.Keys)
      {
        XmlAttribute attribute2 = this._configFile.CreateAttribute(key1);
        attribute2.Value = attributes[key1];
        ((XmlElement) element).SetAttributeNode(attribute2);
      }
      Section.AppendChild(element);
      return element;
    }
  }
}
