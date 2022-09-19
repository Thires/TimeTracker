using GeniePlugin.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;

namespace GeniePlugin
{
  public class PluginLog
  {
    private IHost _host;
    private IPlugin _plugin;
    private TextWriter _logWriter;
    private string _logName;
    private string _fileName;
    private string _filePath;
    private string _fullPath;
    private bool _logGameTime;
    private bool _clearLogAtStart;
    private bool _newLogAtStart;
    private bool _useTitleSeparator;
    private string _logTitle;
    private string _titleSeparator;
    private bool _logStarted;

    public PluginLog(IHost Host, IPlugin PlugIn)
    {
      _host = Host;
      _plugin = PlugIn;
      this._logName = _plugin.Name.Replace(" ", "_");
      this._fileName = this._logName + ".log";
      this._filePath = Application.StartupPath;
      if (this._filePath != "\\")
        this._filePath += "\\";
      this._filePath += "Logs\\";
      this._fullPath = this._filePath + this._fileName;
      this._logGameTime = false;
      this._clearLogAtStart = false;
      this._newLogAtStart = false;
      this._useTitleSeparator = true;
      this._titleSeparator = new string('=', 70);
      this._logStarted = false;
    }

    public bool LogGameTime
    {
      get => this._logGameTime;
      set => this._logGameTime = value;
    }

    public bool ClearLogAtStart
    {
      get => this._clearLogAtStart;
      set => this._clearLogAtStart = value;
    }

    public bool NewLogAtStart
    {
      get => this._newLogAtStart;
      set => this._newLogAtStart = value;
    }

    public void StartLog(string Title)
    {
      int num = 1;
      try
      {
        if (this._newLogAtStart)
        {
          while (File.Exists(this._fullPath))
          {
            this._fullPath = this._filePath + this._logName + "(" + num.ToString() + ").log";
            ++num;
          }
        }
        this._logWriter = (TextWriter) new StreamWriter(this._fullPath, !this._clearLogAtStart);
        this._logTitle = Title;
        if (this._logTitle.Length > 0 && this._useTitleSeparator)
          this._logWriter.WriteLine(this._titleSeparator);
        if (this._logTitle.Length > 0)
          this._logWriter.WriteLine(string.Format("{0} started: {1}", (object) this._logTitle, (object) DateTime.Now));
        if (this._logTitle.Length > 0 && this._useTitleSeparator)
          this._logWriter.WriteLine(this._titleSeparator);
        this._logWriter.Close();
        this._logStarted = true;
      }
      catch (Exception ex)
      {
        _host.EchoText(this._plugin.Name + ".StartLog: " + ex.Message);
      }
    }

    public void Log(string Text)
    {
      try
      {
        if (!this._logStarted)
          this.StartLog("");
        this._logWriter = (TextWriter) new StreamWriter(this._fullPath, true);
        string str = string.Format("[{0:MM/dd/yyyy HH:mm:ss}]\t", (object) DateTime.Now);
        if (this._logGameTime)
          //str += string.Format("({0})\t", (object) _host["gametime"]);
        this._logWriter.WriteLine(str + Text);
        this._logWriter.Close();
      }
      catch (Exception ex)
      {
        _host.EchoText(this._plugin.Name + ".Log: " + ex.Message);
      }
    }

    public void EndLog()
    {
      try
      {
        this._logWriter = (TextWriter) new StreamWriter(this._fullPath, true);
        if (this._logTitle.Length > 0 && this._useTitleSeparator)
          this._logWriter.WriteLine(this._titleSeparator);
        if (this._logTitle.Length > 0)
          this._logWriter.WriteLine(string.Format("{0} ended: {1}", (object) this._logTitle, (object) DateTime.Now));
        this._logWriter.Close();
        this._logStarted = false;
      }
      catch (Exception ex)
      {
        _host.EchoText(this._plugin.Name + ".EndLog: " + ex.Message);
      }
    }
  }
}
