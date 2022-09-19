using System;

namespace TimeTracker
{
  public struct DRDateTime
  {
    private int _year;
    private int _month;
    private int _andu;
    private int _day;
    private int _daysSince;
    private int _anlas;
    private int _rois;
    private bool _range;

    public int Year => this._year;

    public int Month => this._month;

    public int Andu => this._andu;

    public int Day => this._day;

    public int DaysSince => this._daysSince;

    public int Anlas => this._anlas;

    public int Rois => this._rois;

    public DRDateTime(int year, int month, int day)
      : this(year, month, day, 0, 0)
    {
    }

    public DRDateTime(int year, int month, int day, int anlas, int rois)
    {
      this._year = year >= 0 ? year : throw new Exception("DRDateTime Year must be greater than 0.");
      this._month = month >= 1 && month <= 10 ? month : throw new Exception("DRDateTIme Month must be between 1 and 10.");
      this._day = day >= 1 && day <= 40 ? day : throw new Exception("DRDateTime Day must be between 1 and 40.");
      this._andu = (this._day - 1) / 4 + 1;
      this._daysSince = this._day + (this._month - 1) * 40 - 1;
      this._anlas = anlas >= 0 && anlas <= 11 ? anlas : throw new Exception("DRDateTime Anlas must be between 0 and 11.");
      this._rois = rois >= 0 && rois <= 29 ? rois : throw new Exception("DRDateTime Rois must be between 0 and 29.");
      this._range = false;
    }

    public DRDateTime(string date)
    {
      try
      {
        date = date.Trim();
        this._year = int.Parse(date.Substring(0, 3));
        if (this._year < 0)
          throw new Exception("DRDateTime Year must be greater than 0.");
        date = date.Substring(4);
        this._month = int.Parse(date.Substring(0, 2));
        if (this._month < 1 || this._month > 10)
          throw new Exception("DRDateTIme Month must be between 1 and 10.");
        date = date.Substring(3);
        this._day = int.Parse(date.Substring(0, 2));
        if (this._day < 1 || this._day > 40)
          throw new Exception("DRDateTime Day must be between 1 and 40.");
        this._andu = (this._day - 1) / 4 + 1;
        this._daysSince = this._day + (this._month - 1) * 40 - 1;
        if (date.Trim().Length > 2)
        {
          date = date.Substring(3).Trim();
          this._anlas = int.Parse(date.Substring(0, 2));
          if (this._anlas < 0 || this._anlas > 11)
            throw new Exception("DRDateTime Anlas must be between 0 and 11.");
          date = date.Substring(3);
          this._rois = int.Parse(date.Substring(0, 2));
          if (this._rois < 0 || this._rois > 29)
            throw new Exception("DRDateTime Rois must be between 0 and 29.");
        }
        else
        {
          this._anlas = 0;
          this._rois = 0;
        }
        this._range = false;
      }
      catch
      {
        throw new Exception("Invalid DRDateTime format.");
      }
    }

    public override string ToString()
    {
      if (this._range)
        return (this._year != 0 ? this._year.ToString() + " years " : "") + (this._month != 0 ? this._month.ToString() + " months " : "") + (this._day != 0 ? this._day.ToString() + " days " : "") + (this._anlas != 0 ? this._anlas.ToString() + " anlaen " : "") + (this._rois != 0 ? this._rois.ToString() + " roisen " : "");
      return string.Format("{0:00}-{1:00}-{2:00} {3:00}:{4:00}", (object) this.Year, (object) this.Month, (object) this.Day, (object) this.Anlas, (object) this.Rois);
    }

    public DRDateTime Range(int year, int month, int day, int anlas, int rois)
    {
      this._year = year;
      this._month = month;
      this._day = day;
      this._andu = (this._day - 1) / 4 + 1;
      this._daysSince = this._day + (this._month - 1) * 40 - 1;
      this._anlas = anlas;
      this._rois = rois;
      this._range = true;
      return this;
    }

    public bool IsRange() => this._range;
  }
}
