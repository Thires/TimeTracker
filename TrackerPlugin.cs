using GeniePlugin;
using GeniePlugin.Interfaces;
using System;
using System.Timers;
using System.Xml;

namespace TimeTracker
{
  public class TrackerPlugin : IPlugin
  {
    private IHost _host;
    private PluginConfig _config;
    private PluginLog _log;
    private TimeCalc _calc;
    private Timer _timer;
    private FormOptions _form;
    private bool _enabled = true;
    private bool _elanthiaTime;
    private bool _longNames;
    private bool _useGameTime;
    private bool _includeAnlasName;
    private bool _includeTimeOfDay;
    private long _sunOffset;
    private long _yavashOffset;
    private long _xibarOffset;
    private long _katambaOffset;

    public string Name => "Time Tracker";

    public string Version => "1.9.0";

    public string Description => "Tracks the moons of Elanthia, displays information about IC time and provides commands to convert between actual time and IC time.";

    public string Author => "Barnacus";

    public bool Enabled
    {
      set
      {
        this._enabled = value;
        if (this._enabled)
          this._timer.Start();
        else
          this._timer.Stop();
      }
      get => this._enabled;
    }

    public void Initialize(IHost host)
    {
      _host = host;
      this._config = new PluginConfig(_host, (IPlugin) this);
      this._log = new PluginLog(_host, (IPlugin) this);
      this._timer = new Timer();
      this._elanthiaTime = false;
      this._longNames = false;
      this._useGameTime = true;
      this._includeAnlasName = false;
      this._includeTimeOfDay = false;
      this._sunOffset = 0L;
      this._xibarOffset = 0L;
      this._yavashOffset = 0L;
      this._katambaOffset = 0L;
      this.LoadConfig();
      this._log.LogGameTime = true;
      this._calc = new TimeCalc(this._sunOffset, this._katambaOffset, this._xibarOffset, this._yavashOffset);
      this.updateTimeInfo("Time");
      this._timer.Interval = 30000.0;
      this._timer.Elapsed += new ElapsedEventHandler(this.RefreshTime);
      this._timer.Start();
    }

    public void Show()
    {
      if (this._form == null)
      {
        this._form = new FormOptions(_host, this._config, this);
        if (_host.ParentForm != null)
          this._form.MdiParent = _host.ParentForm;
      }
      if (!this._form.Visible)
        this._form.Show();
      this._form.BringToFront();
      this.updateTimeInfo("Time");
    }

    public void ParentClosing() => this._timer.Stop();

    public string ParseText(string Text, string Window) => Window.Trim().ToLower() == "main" || Window.Trim() == string.Empty ? this.ParseText(Text) : Text;

    public string ParseText(string Text)
    {
      if (!this._enabled)
        return Text;
      bool flag = false;
      if (Text.Contains("heralding another fine day") || Text.Contains("rises to create the new day") || Text.Contains("as the sun rises, hidden") || Text.Contains("as the sun rises behind it") || Text.Contains("faintest hint of the rising sun") || Text.Contains("The rising sun slowly"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._sunOffset = this._calc.SunJustRose();
        flag = true;
      }
      if (Text.Contains("The sun sinks below the horizon,") || Text.Contains("night slowly drapes its starry banner") || Text.Contains("sun slowly sinks behind the scattered clouds and vanishes") || Text.Contains("grey light fades into a heavy mantle of black"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._sunOffset = this._calc.SunJustSet();
        flag = true;
      }
      if (Text.Contains("Katamba sets") && !Text.Contains("moonbeam"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._katambaOffset += this._calc.KatambaJustSet();
        flag = true;
      }
      if (Text.Contains("Katamba slowly rises"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._katambaOffset += this._calc.KatambaJustRose();
        flag = true;
      }
      if (Text.Contains("Xibar sets") && !Text.Contains("moonbeam"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._xibarOffset += this._calc.XibarJustSet();
        flag = true;
      }
      if (Text.Contains("Xibar slowly rises"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._xibarOffset += this._calc.XibarJustRose();
        flag = true;
      }
      if (Text.Contains("Yavash sets") && !Text.Contains("moonbeam"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._yavashOffset += this._calc.YavashJustSet();
        flag = true;
      }
      if (Text.Contains("Yavash slowly rises"))
      {
        this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
        this._yavashOffset += this._calc.YavashJustRose();
        flag = true;
      }
      if (flag)
      {
        this.updateTimeInfo("Time");
        this.SaveConfig();
      }
      return Text;
    }

    public string ParseInput(string Text)
    {
      if (!this._enabled)
        return Text;
      if (Text.StartsWith("/timetracker", StringComparison.CurrentCultureIgnoreCase))
      {
        if (Text.Length > 12)
          return Text;
        _host.EchoText("Date Difference Usage:");
        _host.EchoText("*  Local to Elanthian:");
        _host.EchoText("*   /timediff YYYY-MM-DD HH:mm:ss returns Elanthian time difference");
        _host.EchoText("*  Elanthian to Local:");
        _host.EchoText("*   /timediff YYY-MM-DD AA:RR     returns Local time difference");
        _host.EchoText("*  Key:");
        _host.EchoText("*   (AA = Anlas,   RR = Roisen,       YYY = Year since victory of lanival");
        _host.EchoText("*    MM = Month,   DD = Day of Month, HH = Hour on 24-Hour Clock");
        _host.EchoText("*    mm = minutes, ss = seconds,      YYYY = Gregorian Year)");
        _host.EchoText("Date Conversion Usage:");
        _host.EchoText("*  Local to Elanthian:");
        _host.EchoText("*   /time YYYY-MM-DD HH:mm:ss returns Elanthian YYY-MM-DD AA:RR");
        _host.EchoText("*    use /time YYYY-MM-DD HH:mm:ss -long to return the long format.");
        _host.EchoText("*  Elanthian to Local:");
        _host.EchoText("*   /time YYY-MM-DD AA:RR     returns Local YYYY-MM-DD HH:mm:ss");
        _host.EchoText("*  Key:");
        _host.EchoText("*   (AA = Anlas,   RR = Roisen,       YYY = Year since victory of lanival");
        _host.EchoText("*    MM = Month,   DD = Day of Month, HH = Hour on 24-Hour Clock");
        _host.EchoText("*    mm = minutes, ss = seconds,      YYYY = Gregorian Year)");
        _host.EchoText("Current time Usage:");
        _host.EchoText("* /now");
        return string.Empty;
      }
      if (Text.StartsWith("/timediff", StringComparison.CurrentCultureIgnoreCase))
      {
        string str1 = Text.Trim();
        if (str1.Length == 9)
        {
          _host.EchoText("Date Difference Usage:");
          _host.EchoText("*  Local to Elanthian:");
          _host.EchoText("*   /timediff YYYY-MM-DD HH:mm:ss returns Elanthian time difference");
          _host.EchoText("*  Elanthian to Local:");
          _host.EchoText("*   /timediff YYY-MM-DD AA:RR     returns Local time difference");
          _host.EchoText("*  Key:");
          _host.EchoText("*   (AA = Anlas,   RR = Roisen,       YYY = Year since victory of lanival");
          _host.EchoText("*    MM = Month,   DD = Day of Month, HH = Hour on 24-Hour Clock");
          _host.EchoText("*    mm = minutes, ss = seconds,      YYYY = Gregorian Year)");
        }
        else
        {
          if (Text.Length > 9 && Text.Substring(9, 1) != " ")
            return Text;
          try
          {
            str1 = str1.Substring(9).Trim();
            if (str1.Substring(4, 1) == "-")
            {
              DateTime universalTime = DateTime.Parse(str1).ToUniversalTime();
              string str2 = new TimeCalc().DRDateDurationFromDateTime(universalTime).ToString();
              _host.EchoText(str2.Length <= 0 ? str1 + " is right now" : str1 + (universalTime >= DateTime.UtcNow ? " is " + str2 + "from now." : " was " + str2 + "before now."));
            }
            else
            {
              try
              {
                DateTime dateTime = new TimeCalc().DateTimeFromDRDate(new DRDateTime(str1));
                DateTime now = DateTime.Now;
                TimeSpan timeSpan = dateTime - now;
                timeSpan = timeSpan.Duration();
                int num;
                string str3;
                if (timeSpan.Days == 0)
                {
                  str3 = "";
                }
                else
                {
                  num = timeSpan.Days;
                  str3 = num.ToString() + " days ";
                }
                string str4 = str3;
                string str5;
                if (timeSpan.Hours == 0)
                {
                  str5 = "";
                }
                else
                {
                  num = timeSpan.Hours;
                  str5 = num.ToString() + " hours ";
                }
                string str6 = str4 + str5;
                string str7;
                if (timeSpan.Minutes == 0)
                {
                  str7 = "";
                }
                else
                {
                  num = timeSpan.Minutes;
                  str7 = num.ToString() + " minutes ";
                }
                string str8 = str6 + str7;
                _host.EchoText(str8.Length <= 0 ? str1 + " is right now" : str1 + " is " + str8 + (dateTime >= now ? "from now." : "before now."));
              }
              catch
              {
                _host.EchoText("*** " + str1 + " is not a valid Elanthian date/time format.");
              }
            }
          }
          catch
          {
            _host.EchoText("*** " + str1 + " is not a valid value.");
          }
        }
        return string.Empty;
      }
      if (Text.StartsWith("/time", StringComparison.CurrentCultureIgnoreCase))
      {
        string str = Text.Trim();
        bool flag = false;
        if (str.Length == 5)
        {
          _host.EchoText("Date Conversion Usage:");
          _host.EchoText("*  Local to Elanthian:");
          _host.EchoText("*   /time YYYY-MM-DD HH:mm:ss returns Elanthian YYY-MM-DD AA:RR");
          _host.EchoText("*    use /time YYYY-MM-DD HH:mm:ss -long to return the long format.");
          _host.EchoText("*  Elanthian to Local:");
          _host.EchoText("*   /time YYY-MM-DD AA:RR     returns Local YYYY-MM-DD HH:mm:ss");
          _host.EchoText("*  Key:");
          _host.EchoText("*   (AA = Anlas,   RR = Roisen,       YYY = Year since victory of lanival");
          _host.EchoText("*    MM = Month,   DD = Day of Month, HH = Hour on 24-Hour Clock");
          _host.EchoText("*    mm = minutes, ss = seconds,      YYYY = Gregorian Year)");
        }
        else
        {
          if (Text.Length > 5 && Text.Substring(5, 1) != " ")
            return Text;
          try
          {
            if (str.EndsWith("-long", StringComparison.CurrentCultureIgnoreCase))
            {
              flag = true;
              str = str.Substring(0, str.Length - 5).Trim();
            }
            str = str.Substring(5).Trim();
            if (str.Substring(4, 1) == "-")
            {
              try
              {
                DateTime universalTime = DateTime.Parse(str).ToUniversalTime();
                TimeCalc timeCalc = new TimeCalc();
                timeCalc.DRDateFromDateTime(universalTime);
                _host.EchoText(str + " is " + string.Format("{0:00}-{1:00}-{2:00} {3:00}:{4:00} {5}", (object) timeCalc.Year, (object) timeCalc.Month, (object) timeCalc.Day, (object) timeCalc.Anlas, (object) timeCalc.Rois, (object) timeCalc.AnlasName));
                if (flag)
                  _host.EchoText("This is " + timeCalc.DescriptiveText);
              }
              catch
              {
                _host.EchoText("*** " + str + " is not a valid Local date/time format.");
              }
            }
            else
            {
              try
              {
                DRDateTime date = new DRDateTime(str);
                TimeCalc timeCalc = new TimeCalc();
                _host.EchoText(str + " is " + timeCalc.DateTimeFromDRDate(date).ToString("yyyy-MM-dd HH:mm:ss"));
              }
              catch
              {
                _host.EchoText("*** " + str + " is not a valid Elanthian date/time format.");
              }
            }
          }
          catch
          {
            _host.EchoText("*** " + str + " is not a valid value.");
          }
        }
        return string.Empty;
      }
      if (!Text.StartsWith("/now", StringComparison.CurrentCultureIgnoreCase) || Text.Length > 4 && Text.Substring(4, 1) != " ")
        return Text;
      this.updateTimeInfo(string.Empty);
      return string.Empty;
    }

    public void ParseXML(string XML)
    {
    }

    public void VariableChanged(string Variable)
    {
    }

    public bool ElanthiaTime
    {
      get => this._elanthiaTime;
      set => this._elanthiaTime = value;
    }

    public bool LongNames
    {
      get => this._longNames;
      set => this._longNames = value;
    }

    public bool UseGameTime
    {
      get => this._useGameTime;
      set => this._useGameTime = value;
    }

    public bool IncludeAnlasName
    {
      get => this._includeAnlasName;
      set => this._includeAnlasName = value;
    }

    public bool IncludeTimeOfDay
    {
      get => this._includeTimeOfDay;
      set => this._includeTimeOfDay = value;
    }

    public void LoadConfig()
    {
      this._elanthiaTime = false;
      this._longNames = false;
      try
      {
        this._config.OpenConfig();
        XmlNode section1 = this._config.GetSection("Options");
        if (section1 != null)
        {
          this._elanthiaTime = bool.Parse(this._config.GetKeyValue(section1, "ShowElanthiaTime", "false"));
          this._longNames = bool.Parse(this._config.GetKeyValue(section1, "ShowLongNames", "false"));
          this._useGameTime = bool.Parse(this._config.GetKeyValue(section1, "UseGameTime", "true"));
          this._includeAnlasName = bool.Parse(this._config.GetKeyValue(section1, "IncludeAnlasName", "false"));
          this._includeTimeOfDay = bool.Parse(this._config.GetKeyValue(section1, "IncludeTimeOfDay", "false"));
        }
        else
        {
          XmlNode Section = this._config.AddSection("Options");
          this._config.SetKeyValue(Section, "ShowElanthiaTime", this._elanthiaTime.ToString());
          this._config.SetKeyValue(Section, "ShowLongNames", this._longNames.ToString());
          this._config.SetKeyValue(Section, "UseGameTime", this._useGameTime.ToString());
          this._config.SetKeyValue(Section, "IncludeAnlasName", this._includeAnlasName.ToString());
          this._config.SetKeyValue(Section, "IncludeTimeOfDay", this._includeTimeOfDay.ToString());
        }
        XmlNode section2 = this._config.GetSection("Calculations");
        if (section2 != null)
        {
          this._sunOffset = long.Parse(this._config.GetKeyValue(section2, "SunOffset", "0"));
          this._katambaOffset = long.Parse(this._config.GetKeyValue(section2, "KatambaOffset", "0"));
          this._xibarOffset = long.Parse(this._config.GetKeyValue(section2, "XibarOffset", "0"));
          this._yavashOffset = long.Parse(this._config.GetKeyValue(section2, "YavashOffset", "0"));
        }
        else
        {
          XmlNode Section = this._config.AddSection("Calculations");
          this._config.SetKeyValue(Section, "SunOffset", this._sunOffset.ToString());
          this._config.SetKeyValue(Section, "KatambaOffset", this._katambaOffset.ToString());
          this._config.SetKeyValue(Section, "XibarOffset", this._xibarOffset.ToString());
          this._config.SetKeyValue(Section, "YavashOffset", this._yavashOffset.ToString());
        }
      }
      catch (Exception ex)
      {
        _host.EchoText("Could not load config file " + ex.Message + "\n");
      }
    }

    public void SaveConfig()
    {
      try
      {
        XmlNode section1 = this._config.GetSection("Options");
        this._config.SetKeyValue(section1, "ShowElanthiaTime", this._elanthiaTime.ToString());
        this._config.SetKeyValue(section1, "ShowLongNames", this._longNames.ToString());
        this._config.SetKeyValue(section1, "UseGameTime", this._useGameTime.ToString());
        this._config.SetKeyValue(section1, "IncludeAnlasName", this._includeAnlasName.ToString());
        this._config.SetKeyValue(section1, "IncludeTimeOfDay", this._includeTimeOfDay.ToString());
        XmlNode section2 = this._config.GetSection("Calculations");
        this._config.SetKeyValue(section2, "SunOffset", this._sunOffset.ToString());
        this._config.SetKeyValue(section2, "KatambaOffset", this._katambaOffset.ToString());
        this._config.SetKeyValue(section2, "XibarOffset", this._xibarOffset.ToString());
        this._config.SetKeyValue(section2, "YavashOffset", this._yavashOffset.ToString());
        this._config.SaveConfig();
      }
      catch (Exception ex)
      {
        _host.EchoText("Could not save config file " + ex.Message + "\n");
      }
    }

    public void RefreshTime(object source, EventArgs e) => this.RefreshTimeTracker();

    public void RefreshTimeTracker()
    {
      this._calc.GameTime = long.Parse(_host.get_Variable("gametime"));
      this._calc.CalculateTimes();
      this.updateTimeInfo("Time");
    }

    private void updateTimeInfo(string Window)
    {
      string str1 = "RISE";
      string str2 = "SET ";
      if (Window != string.Empty)
      {
        if (!Window.StartsWith(">"))
          Window = ">" + Window;
        Window = Window.Trim() + " ";
        _host.SendText("#echo " + Window + "@suspend@");
      }
      string str3 = "\"Katamba " + (this._calc.KatambaIsVisible ? str2 : str1) + " in ";
      TimeSpan timeSpan1 = TimeSpan.FromSeconds((double) this._calc.KatambaTime);
      string str4 = str3 + string.Format("{0:0}:{1:00}", (object) timeSpan1.Hours, (object) timeSpan1.Minutes) + "\"";
      string str5 = (this._calc.KatambaTime < 360L ? "Red " : "") + str4;
      _host.SendText("#echo " + Window + str5);
      string str6 = "\"Xibar   " + (this._calc.XibarIsVisible ? str2 : str1) + " in ";
      TimeSpan timeSpan2 = TimeSpan.FromSeconds((double) this._calc.XibarTime);
      string str7 = str6 + string.Format("{0:0}:{1:00}", (object) timeSpan2.Hours, (object) timeSpan2.Minutes) + "\"";
      string str8 = (this._calc.XibarTime < 360L ? "Red " : "") + str7;
      _host.SendText("#echo " + Window + str8);
      string str9 = "\"Yavash  " + (this._calc.YavashIsVisible ? str2 : str1) + " in ";
      TimeSpan timeSpan3 = TimeSpan.FromSeconds((double) this._calc.YavashTime);
      string str10 = str9 + string.Format("{0:0}:{1:00}", (object) timeSpan3.Hours, (object) timeSpan3.Minutes) + "\"";
      string str11 = (this._calc.YavashTime < 360L ? "Red " : "") + str10;
      _host.SendText("#echo " + Window + str11);
      string str12 = "\"Sun     " + (this._calc.SunIsVisible ? str2 : str1) + " in ";
      TimeSpan timeSpan4 = TimeSpan.FromSeconds((double) this._calc.SunTime);
      string str13 = str12 + string.Format("{0:0}:{1:00}", (object) timeSpan4.Hours, (object) timeSpan4.Minutes) + "\"";
      _host.SendText("#echo " + Window + str13);
      if (this._elanthiaTime)
      {
        string str14;
        if (this._includeAnlasName)
          str14 = string.Format("{0:00}-{1:00}-{2:00} {3:00}:{4:00} {5}", (object) this._calc.Year, (object) this._calc.Month, (object) this._calc.Day, (object) this._calc.Anlas, (object) this._calc.Rois, (object) this._calc.AnlasName);
        else
          str14 = string.Format("{0:00}-{1:00}-{2:00} {3:00}:{4:00}", (object) this._calc.Year, (object) this._calc.Month, (object) this._calc.Day, (object) this._calc.Anlas, (object) this._calc.Rois);
        _host.SendText("#echo " + Window + str14);
      }
      if (this._longNames)
        _host.SendText("#echo " + Window + this._calc.DescriptiveText);
      else if (this._includeTimeOfDay)
      {
        string str15 = string.Format("It is currently {0} and {1}. ", (object) this._calc.Season, (object) this._calc.TimeOfDay);
        _host.SendText("#echo " + Window + str15);
      }
      if (Window != string.Empty)
        _host.SendText("#echo " + Window + "@resume@");
      _host.SendText(string.Format("#var Time.isKatambaUp {0}", (object) (this._calc.KatambaIsVisible ? 1 : 0)));
      _host.SendText(string.Format("#var Time.isXibarUp {0}", (object) (this._calc.XibarIsVisible ? 1 : 0)));
      _host.SendText(string.Format("#var Time.isYavashUp {0}", (object) (this._calc.YavashIsVisible ? 1 : 0)));
      _host.SendText(string.Format("#var Time.isDay {0}", (object) (this._calc.SunIsVisible ? 1 : 0)));
      _host.SendText(string.Format("#var Time.timeOfDay {0}", (object) this._calc.TimeOfDay));
      _host.SendText(string.Format("#var Time.season {0}", (object) this._calc.Season));
      _host.SendText(string.Format("#var Time.katambaSeconds {0}", (object) this._calc.KatambaTime));
      _host.SendText(string.Format("#var Time.xibarSeconds {0}", (object) this._calc.XibarTime));
      _host.SendText(string.Format("#var Time.yavashSeconds {0}", (object) this._calc.YavashTime));
      _host.SendText(string.Format("#var Time.sunSeconds {0}", (object) this._calc.SunTime));
    }
  }
}
