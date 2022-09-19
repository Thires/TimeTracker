using System;

namespace TimeTracker
{
  internal class TimeCalc
  {
    private const long _katambaBegin = 49623621;
    private const long _xibarBegin = 49697781;
    private const long _yavashBegin = 49376779;
    private const long _katambaBaseline = 10603;
    private const long _xibarBaseline = 10483;
    private const long _yavashBaseline = 10621;
    private const long _katambaCycle = 21088;
    private const long _xibarCycle = 20848;
    private const long _yavashCycle = 21130;
    private DRDateTime _drDateTime;
    private TimeCalc.riseSetData[] _sunData;
    private string[] _anlasNames;
    private string[] _anduNames;
    private string[] _monthNames;
    private string[] _yearNames;
    private string[] _seasonNames;
    private string[] _partsOfDay;
    private bool _sunIsVisible = false;
    private bool _katambaIsVisible = false;
    private bool _xibarIsVisible = false;
    private bool _yavashIsVisible = false;
    private long _katambaStart;
    private long _xibarStart;
    private long _yavashStart;
    private long _katambaTime = 180;
    private long _xibarTime = 180;
    private long _yavashTime = 180;
    private long _sunTime = 360;
    private long _katambaOffset = 0;
    private long _xibarOffset = 0;
    private long _yavashOffset = 0;
    private long _sunOffset = 0;
    private bool _useGameTime = true;
    private long _gameTime = 0;
    private long _theSeconds;
    private long _totalSeconds;

    public TimeCalc(long sunOffset, long katambaOffset, long xibarOffset, long yavashOffset)
    {
      this._katambaOffset = katambaOffset;
      this._xibarOffset = xibarOffset;
      this._yavashOffset = yavashOffset;
      this._sunOffset = sunOffset;
      this.init();
      this._katambaStart += this._katambaOffset;
      this._xibarStart += this._xibarOffset;
      this._yavashStart += this._yavashOffset;
      this.CalculateTimes();
    }

    public TimeCalc() => this.init();

    public long SunOffset
    {
      get => this._sunOffset;
      set => this._sunOffset = value;
    }

    public long KatambaOffset
    {
      get => this._katambaOffset;
      set => this._katambaOffset = value;
    }

    public long XibarOffset
    {
      get => this._xibarOffset;
      set => this._xibarOffset = value;
    }

    public long YavashOffset
    {
      get => this._yavashOffset;
      set => this._yavashOffset = value;
    }

    public long SunTime => this._sunTime;

    public bool SunIsVisible => this._sunIsVisible;

    public long YavashTime => this._yavashTime;

    public bool YavashIsVisible => this._yavashIsVisible;

    public long XibarTime => this._xibarTime;

    public bool XibarIsVisible => this._xibarIsVisible;

    public long KatambaTime => this._katambaTime;

    public bool KatambaIsVisible => this._katambaIsVisible;

    public long Year => (long) this._drDateTime.Year;

    public string YearName => this._yearNames[this.getYearCycle((long) this._drDateTime.Year)];

    public long DaysSince => (long) this._drDateTime.DaysSince;

    public long Month => (long) this._drDateTime.Month;

    public string MonthName => this._monthNames[this._drDateTime.Month];

    public long Day => (long) this._drDateTime.Day;

    public long Anlas => (long) this._drDateTime.Anlas;

    public string AnlasName => this._anlasNames[this._drDateTime.Anlas];

    public long Rois => (long) this._drDateTime.Rois;

    public long GameTime
    {
      get => this._gameTime;
      set => this._gameTime = this.GameTime;
    }

    public string Season
    {
      get
      {
        if (this._drDateTime.DaysSince < 50)
          return this._seasonNames[4];
        if (this._drDateTime.DaysSince < 150)
          return this._seasonNames[1];
        if (this._drDateTime.DaysSince < 250)
          return this._seasonNames[2];
        return this._drDateTime.DaysSince < 350 ? this._seasonNames[3] : this._seasonNames[4];
      }
    }

    public string TimeOfDay
    {
      get
      {
        int index = 1;
        int num1 = (int) (this._sunData[this._drDateTime.DaysSince].setSeconds - this._sunData[this._drDateTime.DaysSince].riseSeconds + 1L);
        int num2 = 21600 - num1;
        if (this._theSeconds >= this._sunData[this._drDateTime.DaysSince].riseSeconds - (long) (num2 / 8))
          index = 2;
        if (this._theSeconds >= this._sunData[this._drDateTime.DaysSince].riseSeconds)
          index = 3;
        if (this._theSeconds >= this._sunData[this._drDateTime.DaysSince].riseSeconds + (long) (num2 / 8))
          index = 4;
        if (this._theSeconds >= (long) (10800 - num1 / 4))
          index = 5;
        if (this._theSeconds >= (long) (10800 - num1 / 8))
          index = 6;
        if (this._theSeconds >= 10800L)
          index = 7;
        if (this._theSeconds >= (long) (10800 + num1 / 8))
          index = 8;
        if (this._theSeconds >= (long) (10800 + num1 / 4))
          index = 9;
        if (this._theSeconds >= this._sunData[this._drDateTime.DaysSince].setSeconds - (long) (num1 / 8))
          index = 10;
        if (this._theSeconds >= this._sunData[this._drDateTime.DaysSince].setSeconds - (long) (num1 / 9))
          index = 11;
        if (this._theSeconds >= this._sunData[this._drDateTime.DaysSince].setSeconds)
          index = 12;
        if (this._theSeconds >= this._sunData[this._drDateTime.DaysSince].setSeconds + (long) (num2 / 9))
          index = 13;
        if (this._theSeconds >= (long) (21600 - num2 / 4))
          index = 14;
        if (this._theSeconds >= (long) (21600 - num2 / 8))
          index = 15;
        return this._partsOfDay[index];
      }
    }

    public bool UseGameTime
    {
      get => this._useGameTime;
      set => this._useGameTime = this.UseGameTime;
    }

    public string DescriptiveText => string.Format("It is the {0}, {1} years since the Victory of Lanival the Redeemer. ", (object) this._yearNames[this.getYearCycle((long) this._drDateTime.Year)], (object) this._drDateTime.Year) + string.Format("It is the {0}{1} day and the {2}{3} andu of {4} in ", (object) this._drDateTime.Day, (object) this.getNumberSuffix((long) this._drDateTime.Day), (object) this._drDateTime.Andu, (object) this.getNumberSuffix((long) this._drDateTime.Andu), (object) this._anduNames[this._drDateTime.Andu]) + string.Format("the {0}{1} month of {2}. ", (object) this._drDateTime.Month, (object) this.getNumberSuffix((long) this._drDateTime.Month), (object) this._monthNames[this._drDateTime.Month]) + string.Format("It is now the {0}{1} roisan of the {2}{3} anlas of {4}. ", (object) this._drDateTime.Rois, (object) this.getNumberSuffix((long) this._drDateTime.Rois), (object) this._drDateTime.Anlas, (object) this.getNumberSuffix((long) this._drDateTime.Anlas), (object) this._anlasNames[this._drDateTime.Anlas]) + string.Format("It is currently {0} and {1}. ", (object) this.Season, (object) this.TimeOfDay);

    public void CalculateTimes()
    {
      this.drSecondsFromReferenceDate();
      if (this._theSeconds < this._sunData[this._drDateTime.DaysSince].riseSeconds || this._theSeconds >= this._sunData[this._drDateTime.DaysSince].setSeconds)
      {
        this._sunIsVisible = false;
        this._sunTime = this.getSunHidden();
      }
      else
      {
        this._sunIsVisible = true;
        this._sunTime = this.getSunVisible();
      }
      if (this._yavashStart < 0L)
      {
        while (this._yavashStart < 0L)
          this._yavashStart += 21130L;
      }
      if (this._yavashStart >= 21130L)
      {
        while (this._yavashStart >= 21130L)
          this._yavashStart -= 21130L;
      }
      if (this._xibarStart < 0L)
      {
        while (this._xibarStart < 0L)
          this._xibarStart += 20848L;
      }
      if (this._xibarStart >= 20848L)
      {
        while (this._xibarStart >= 20848L)
          this._xibarStart -= 20848L;
      }
      if (this._katambaStart < 0L)
      {
        while (this._katambaStart < 0L)
          this._katambaStart += 21088L;
      }
      if (this._katambaStart >= 21088L)
      {
        while (this._katambaStart >= 21088L)
          this._katambaStart -= 21088L;
      }
      long num1 = this._totalSeconds - this._yavashStart - 49376779L;
      if (num1 > 21130L)
      {
        while (num1 > 21130L)
          num1 -= 21130L;
      }
      if (num1 < 10621L)
      {
        this._yavashIsVisible = true;
        this._yavashTime = this.getYavashVisible();
      }
      else
      {
        this._yavashIsVisible = false;
        this._yavashTime = this.getYavashHidden();
      }
      long num2 = this._totalSeconds - this._xibarStart - 49697781L;
      if (num2 > 20848L)
      {
        while (num2 > 20848L)
          num2 -= 20848L;
      }
      if (num2 < 10483L)
      {
        this._xibarIsVisible = true;
        this._xibarTime = this.getXibarVisible();
      }
      else
      {
        this._xibarIsVisible = false;
        this._xibarTime = this.getXibarHidden();
      }
      long num3 = this._totalSeconds - this._katambaStart - 49623621L;
      if (num3 > 21088L)
      {
        while (num3 > 21088L)
          num3 -= 21088L;
      }
      if (num3 < 10603L)
      {
        this._katambaIsVisible = true;
        this._katambaTime = this.getKatambaVisible();
      }
      else
      {
        this._katambaIsVisible = false;
        this._katambaTime = this.getKatambaHidden();
      }
    }

    public long SunJustSet()
    {
      long sunTime = this._sunTime;
      this._sunTime = this.reCalcSunHidden();
      this._sunOffset = this._sunTime - sunTime;
      this._sunIsVisible = false;
      return this._sunOffset;
    }

    public long SunJustRose()
    {
      long sunTime = this._sunTime;
      this._sunTime = this.reCalcSunVisible();
      this._sunOffset = this._sunTime - sunTime;
      this._sunIsVisible = true;
      return this._sunOffset;
    }

    public long KatambaJustSet()
    {
      long katambaTime = this._katambaTime;
      if (katambaTime > 2L)
      {
        if (!this._katambaIsVisible)
          katambaTime -= 10485L;
        long num = -katambaTime;
        this._katambaStart += num;
        this._katambaOffset = num;
        this.CalculateTimes();
      }
      return this._katambaOffset;
    }

    public long KatambaJustRose()
    {
      long katambaTime = this._katambaTime;
      if (katambaTime > 2L)
      {
        if (this._katambaIsVisible)
          katambaTime -= 10603L;
        long num = -katambaTime;
        this._katambaStart += num;
        this._katambaOffset = num;
        this.CalculateTimes();
      }
      return this._katambaOffset;
    }

    public long XibarJustSet()
    {
      long xibarTime = this._xibarTime;
      if (xibarTime > 2L)
      {
        if (!this._xibarIsVisible)
          xibarTime -= 10365L;
        long num = -xibarTime;
        this._xibarStart += num;
        this._xibarOffset = num;
        this.CalculateTimes();
      }
      return this._xibarOffset;
    }

    public long XibarJustRose()
    {
      long xibarTime = this._xibarTime;
      if (xibarTime > 2L)
      {
        if (this._xibarIsVisible)
          xibarTime -= 10483L;
        long num = -xibarTime;
        this._xibarStart += num;
        this._xibarOffset = num;
        this.CalculateTimes();
      }
      return this._xibarOffset;
    }

    public long YavashJustSet()
    {
      long yavashTime = this._yavashTime;
      if (yavashTime > 2L)
      {
        if (!this._yavashIsVisible)
          yavashTime -= 10509L;
        long num = -yavashTime;
        this._yavashStart += num;
        this._yavashOffset = num;
        this.CalculateTimes();
      }
      return this._yavashOffset;
    }

    public long YavashJustRose()
    {
      long yavashTime = this._yavashTime;
      if (yavashTime > 2L)
      {
        if (this._yavashIsVisible)
          yavashTime -= 10621L;
        long num = -yavashTime;
        this._yavashStart += num;
        this._yavashOffset = num;
        this.CalculateTimes();
      }
      return this._yavashOffset;
    }

    public DRDateTime DRDateDurationFromDateTime(DateTime theDate)
    {
      double num = 63297143100.0;
      this.DRDateFromDateTime(DateTime.UtcNow);
      return this.returnOffsetFromSeconds((double) ((((long) TimeSpan.FromTicks(theDate.Ticks).Days * 24L + (long) theDate.Hour) * 60L + (long) theDate.Minute) * 60L + (long) theDate.Second) - num - (double) this._totalSeconds);
    }

    public void DRDateFromDateTime(DateTime theDate)
    {
      double num1 = 63297143100.0;
      double num2 = (double) ((((long) TimeSpan.FromTicks(theDate.Ticks).Days * 24L + (long) theDate.Hour) * 60L + (long) theDate.Minute) * 60L + (long) theDate.Second) - num1;
      this._totalSeconds = (long) Math.Floor(num2);
      this._theSeconds = this.drDateTimeFromSeconds(num2);
    }

    public DateTime DateTimeFromDRDate(DRDateTime date)
    {
      double num = 63297143100.0;
      return new DateTime(TimeSpan.FromSeconds(((((double) (date.Year - 385) * 400.0 + (double) date.DaysSince) * 12.0 + (double) date.Anlas) * 30.0 + (double) date.Rois) * 60.0 + num).Ticks).ToLocalTime();
    }

    private void init()
    {
      this._sunData = new TimeCalc.riseSetData[401];
      this._anlasNames = new string[13];
      this._anduNames = new string[11];
      this._monthNames = new string[11];
      this._yearNames = new string[8];
      this._seasonNames = new string[5];
      this._partsOfDay = new string[16];
      this.initRiseSetData();
      this.initNames();
      this._yavashIsVisible = false;
      this._xibarIsVisible = false;
      this._katambaIsVisible = false;
      this._sunIsVisible = true;
      this._katambaTime = 180L;
      this._xibarTime = 180L;
      this._yavashTime = 180L;
      this._sunTime = 360L;
      this._katambaStart = 0L;
      this._xibarStart = 0L;
      this._yavashStart = 0L;
      this._drDateTime = new DRDateTime(0, 1, 1);
      this._theSeconds = 0L;
      this._gameTime = 0L;
      this.CalculateTimes();
    }

    private void initRiseSetData()
    {
      for (int index = 0; index < 400; ++index)
      {
        double num = (double) (index + 98);
        this._sunData[index].setSeconds = (long) Math.Floor(10800.0 - Math.Sin(num / 400.0 * (2.0 * Math.PI)) * 1800.0 + 5400.0);
        this._sunData[index].riseSeconds = 21600L - this._sunData[index].setSeconds;
      }
    }

    private void initNames()
    {
      this._anlasNames[0] = "Anduwen";
      this._anlasNames[1] = "Starwatch";
      this._anlasNames[2] = "Asketi's Hunt";
      this._anlasNames[3] = "Berengaria's Touch";
      this._anlasNames[4] = "Hodierna's Blessing";
      this._anlasNames[5] = "Peri'el's Watch";
      this._anlasNames[6] = "Dergati's Bane";
      this._anlasNames[7] = "Firulf's Flame";
      this._anlasNames[8] = "Tamsine's Toil";
      this._anlasNames[9] = "Meraud's Cloak";
      this._anlasNames[10] = "Phelim's Vigil";
      this._anlasNames[11] = "Revelfae";
      this._anduNames[1] = "Kertandu";
      this._anduNames[2] = "Hodandu";
      this._anduNames[3] = "Evandu";
      this._anduNames[4] = "Truffandu";
      this._anduNames[5] = "Havrandu";
      this._anduNames[6] = "Elandu";
      this._anduNames[7] = "Chandu";
      this._anduNames[8] = "Glythandu";
      this._anduNames[9] = "Faenandu";
      this._anduNames[10] = "Tamsandu";
      this._monthNames[1] = "Akroeg, the Ram";
      this._monthNames[2] = "Ka'len, the Sea Drake";
      this._monthNames[3] = "Lirisa, the Archer";
      this._monthNames[4] = "Shorka, the Cobra";
      this._monthNames[5] = "Uthmor, the Giant";
      this._monthNames[6] = "Arhat, the Fire Lion";
      this._monthNames[7] = "Moliko, the Balance";
      this._monthNames[8] = "Skullcleaver, the Dwarven Axe";
      this._monthNames[9] = "Dolefaren, the Brigantine";
      this._monthNames[10] = "Nissa, the Maiden";
      this._yearNames[1] = "Year of the Bronze Wyvern";
      this._yearNames[2] = "Year of the Golden Panther";
      this._yearNames[3] = "Year of the Amber Phoenix";
      this._yearNames[4] = "Year of the Iron Toad";
      this._yearNames[5] = "Year of the Emerald Dolphin";
      this._yearNames[6] = "Year of the Crystal Snow Hare";
      this._yearNames[7] = "Year of the Silver Unicorn";
      this._seasonNames[1] = "spring";
      this._seasonNames[2] = "summer";
      this._seasonNames[3] = "fall";
      this._seasonNames[4] = "winter";
      this._partsOfDay[1] = "night";
      this._partsOfDay[2] = "approaching sunrise";
      this._partsOfDay[3] = "dawn";
      this._partsOfDay[4] = "early morning";
      this._partsOfDay[5] = "mid-morning";
      this._partsOfDay[6] = "late morning";
      this._partsOfDay[7] = "midday";
      this._partsOfDay[8] = "early afternoon";
      this._partsOfDay[9] = "mid-afternoon";
      this._partsOfDay[10] = "late afternoon";
      this._partsOfDay[11] = "dusk";
      this._partsOfDay[12] = "sunset";
      this._partsOfDay[13] = "early evening";
      this._partsOfDay[14] = "evening";
      this._partsOfDay[15] = "late evening";
    }

    private DRDateTime returnOffsetFromSeconds(double drBase)
    {
      double num1 = 0.0;
      double d1 = drBase / 60.0;
      double d2 = d1 / 30.0;
      double d3 = d2 / 12.0;
      double d4 = d3 / 400.0;
      double d5 = Math.Truncate(d3) - Math.Truncate(d4) * 400.0;
      double d6 = Math.Truncate(d5 / 40.0);
      double num2 = (double) (int) (Math.Truncate(d5) - Math.Truncate(d6) * 40.0);
      double d7 = Math.Truncate(d2) - (Math.Truncate(d4) * 400.0 + Math.Truncate(d5)) * 12.0;
      double d8 = Math.Truncate(d1) - ((Math.Truncate(d4) * 400.0 + Math.Truncate(d5)) * 12.0 + Math.Truncate(d7)) * 30.0;
      num1 = Math.Truncate(drBase) - (((Math.Truncate(d4) * 400.0 + Math.Truncate(d5)) * 12.0 + Math.Truncate(d7)) * 30.0 + Math.Truncate(d8)) * 60.0;
      return new DRDateTime().Range((int) Math.Abs(d4), (int) Math.Abs(d6), (int) Math.Abs(num2), (int) Math.Abs(d7), (int) Math.Abs(d8));
    }

    private long drDateTimeFromSeconds(double drBase)
    {
      double num1 = 0.0;
      double d1 = drBase / 60.0;
      double d2 = d1 / 30.0;
      double d3 = d2 / 12.0;
      double d4 = d3 / 400.0;
      double d5 = Math.Floor(d3) - Math.Floor(d4) * 400.0;
      double d6 = d5 / 40.0;
      double num2 = (double) (int) (Math.Floor(d5) - Math.Floor(d6) * 40.0 + 1.0);
      double d7 = Math.Floor(d2) - (Math.Floor(d4) * 400.0 + Math.Floor(d5)) * 12.0;
      double d8 = Math.Floor(d1) - ((Math.Floor(d4) * 400.0 + Math.Floor(d5)) * 12.0 + Math.Floor(d7)) * 30.0;
      num1 = Math.Floor(drBase) - (((Math.Floor(d4) * 400.0 + Math.Floor(d5)) * 12.0 + Math.Floor(d7)) * 30.0 + Math.Floor(d8)) * 60.0;
      this._drDateTime = new DRDateTime((int) Math.Floor(d4 + 385.0), (int) d6 + 1, (int) num2, (int) d7, (int) d8);
      return (long) (Math.Floor(drBase) - (Math.Floor(d4) * 400.0 + Math.Floor(d5)) * 12.0 * 30.0 * 60.0);
    }

    private void drSecondsFromReferenceDate()
    {
      double num1 = 63297143100.0;
      double num2 = 1161546300.0;
      DateTime utcNow = DateTime.UtcNow;
      double num3 = this._gameTime == 0L || !this._useGameTime ? (double) ((((long) TimeSpan.FromTicks(utcNow.Ticks).Days * 24L + (long) utcNow.Hour) * 60L + (long) utcNow.Minute) * 60L + (long) utcNow.Second) - num1 : (double) ((((long) TimeSpan.FromSeconds((double) this._gameTime).Days * 24L + (long) utcNow.Hour) * 60L + (long) utcNow.Minute) * 60L + (long) utcNow.Second) - num2;
      this._totalSeconds = (long) Math.Floor(num3);
      this._theSeconds = this.drDateTimeFromSeconds(num3);
    }

    private long reCalcSunHidden()
    {
      this.drSecondsFromReferenceDate();
      long daysSince = (long) this._drDateTime.DaysSince;
      long num1 = this._theSeconds;
      if (!this._sunIsVisible)
        this._sunTime = 0L;
      if (num1 < this._sunData[daysSince].riseSeconds)
        num1 = this._sunData[daysSince].riseSeconds - num1;
      else if (num1 >= this._sunData[daysSince].setSeconds)
      {
        long num2 = 21600L - num1;
        long index = daysSince + 1L;
        if (index > 400L)
          index = 1L;
        num1 = num2 + this._sunData[index].riseSeconds;
      }
      return this._sunTime + num1;
    }

    private long reCalcSunVisible()
    {
      if (this._sunIsVisible)
        this._sunTime = 0L;
      this.drSecondsFromReferenceDate();
      return this._sunTime + (this._sunData[this._drDateTime.DaysSince].setSeconds - this._theSeconds);
    }

    private long getSunHidden()
    {
      long daysSince = (long) this._drDateTime.DaysSince;
      long num1 = this._theSeconds;
      if (num1 < this._sunData[daysSince].riseSeconds)
        num1 = this._sunData[daysSince].riseSeconds - num1;
      else if (num1 >= this._sunData[daysSince].setSeconds)
      {
        long num2 = 21600L - num1;
        long index = daysSince + 1L;
        if (index > 400L)
          index = 1L;
        num1 = num2 + this._sunData[index].riseSeconds;
      }
      return num1;
    }

    private long getKatambaHidden()
    {
      long num1 = this._totalSeconds - this._katambaStart - 49623621L;
      long num2 = 0;
      if (num1 > 21088L)
      {
        while (num1 > 21088L)
          num1 -= 21088L;
      }
      if (num1 >= 10603L)
        num2 = 21086L - num1;
      return num2;
    }

    private long getXibarHidden()
    {
      long num1 = this._totalSeconds - this._xibarStart - 49697781L;
      long num2 = 0;
      if (num1 > 20848L)
      {
        while (num1 > 20848L)
          num1 -= 20848L;
      }
      if (num1 >= 10483L)
        num2 = 20846L - num1;
      return num2;
    }

    private long getYavashHidden()
    {
      long num1 = this._totalSeconds - this._yavashStart - 49376779L;
      long num2 = 0;
      if (num1 > 21130L)
      {
        while (num1 > 21130L)
          num1 -= 21130L;
      }
      if (num1 >= 10621L)
        num2 = 21128L - num1;
      return num2;
    }

    private long getSunVisible() => this._sunData[this._drDateTime.DaysSince].setSeconds - this._theSeconds;

    private long getKatambaVisible()
    {
      long num1 = this._totalSeconds - this._katambaStart - 49623621L;
      long num2 = 0;
      if (num1 > 21088L)
      {
        while (num1 > 21088L)
          num1 -= 21088L;
      }
      if (num1 < 10603L)
        num2 = 10601L - num1;
      return num2;
    }

    private long getXibarVisible()
    {
      long num1 = this._totalSeconds - this._xibarStart - 49697781L;
      long num2 = 0;
      if (num1 > 20848L)
      {
        while (num1 > 20848L)
          num1 -= 20848L;
      }
      if (num1 < 10483L)
        num2 = 10481L - num1;
      return num2;
    }

    private long getYavashVisible()
    {
      long num1 = this._totalSeconds - this._yavashStart - 49376779L;
      long num2 = 0;
      if (num1 > 21130L)
      {
        while (num1 > 21130L)
          num1 -= 21130L;
      }
      if (num1 < 10621L)
        num2 = 10619L - num1;
      return num2;
    }

    private int getYearCycle(long year)
    {
      int num = (int) (year % 7L);
      return num == 0 ? 7 : num;
    }

    private string getNumberSuffix(long Value)
    {
      int num = (int) (Value % 100L);
      if (num > 10 && num < 20)
        return "th";
      switch (Value % 10L)
      {
        case 1:
          return "st";
        case 2:
          return "nd";
        case 3:
          return "rd";
        default:
          return "th";
      }
    }

    private struct riseSetData
    {
      public long setSeconds;
      public long riseSeconds;
    }
  }
}
