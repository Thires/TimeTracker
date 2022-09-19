using GeniePlugin;
using GeniePlugin.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeTracker
{
  public class FormOptions : Form
  {
    private IHost _host;
    private TrackerPlugin _plugin;
    private PluginConfig _config;
    private IContainer components = (IContainer) null;
    private CheckBox cbxElanthiaTime;
    private CheckBox cbxNames;
    private Button cmdOK;
    private Button cmdCancel;
    private CheckBox cbxGameTime;
    private CheckBox cbxIncludeAnlas;
    private CheckBox cbxIncludeTimeOfDay;

    public FormOptions(IHost Host, PluginConfig Config, TrackerPlugin Plugin)
    {
      this.InitializeComponent();
      _host = Host;
      this._config = Config;
      this._plugin = Plugin;
    }

    private void FormOptions_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.Hide();
      e.Cancel = true;
    }

    private void cmdOK_Click(object sender, EventArgs e)
    {
      this._plugin.ElanthiaTime = this.cbxElanthiaTime.Checked;
      this._plugin.LongNames = this.cbxNames.Checked;
      this._plugin.UseGameTime = this.cbxGameTime.Checked;
      this._plugin.IncludeAnlasName = this.cbxIncludeAnlas.Checked;
      this._plugin.IncludeTimeOfDay = this.cbxIncludeTimeOfDay.Checked;
      this._plugin.SaveConfig();
      this._plugin.RefreshTimeTracker();
      this.Hide();
    }

    private void cmdCancel_Click(object sender, EventArgs e) => this.Hide();

    private void FormOptions_VisibleChanged(object sender, EventArgs e)
    {
      if (!this.Visible)
        return;
      this._plugin.LoadConfig();
      this.cbxElanthiaTime.Checked = this._plugin.ElanthiaTime;
      this.cbxNames.Checked = this._plugin.LongNames;
      this.cbxGameTime.Checked = this._plugin.UseGameTime;
      this.cbxIncludeAnlas.Checked = this._plugin.IncludeAnlasName;
      this.cbxIncludeTimeOfDay.Checked = this._plugin.IncludeTimeOfDay;
    }

    private void FormOptions_HelpButtonClicked(object sender, CancelEventArgs e)
    {
      _host.EchoText("/timetracker");
      this._plugin.ParseText("/timetracker");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.cbxElanthiaTime = new CheckBox();
      this.cbxNames = new CheckBox();
      this.cmdOK = new Button();
      this.cmdCancel = new Button();
      this.cbxGameTime = new CheckBox();
      this.cbxIncludeAnlas = new CheckBox();
      this.cbxIncludeTimeOfDay = new CheckBox();
      this.SuspendLayout();
      this.cbxElanthiaTime.AutoSize = true;
      this.cbxElanthiaTime.Location = new Point(13, 36);
      this.cbxElanthiaTime.Name = "cbxElanthiaTime";
      this.cbxElanthiaTime.Size = new Size(120, 17);
      this.cbxElanthiaTime.TabIndex = 2;
      this.cbxElanthiaTime.Text = "Show Elanthia Time";
      this.cbxElanthiaTime.UseVisualStyleBackColor = true;
      this.cbxNames.AutoSize = true;
      this.cbxNames.Location = new Point(13, 59);
      this.cbxNames.Name = "cbxNames";
      this.cbxNames.Size = new Size(192, 17);
      this.cbxNames.TabIndex = 4;
      this.cbxNames.Text = "Show Descriptive Dates and Times";
      this.cbxNames.UseVisualStyleBackColor = true;
      this.cmdOK.DialogResult = DialogResult.OK;
      this.cmdOK.Location = new Point(58, 93);
      this.cmdOK.Name = "cmdOK";
      this.cmdOK.Size = new Size(75, 23);
      this.cmdOK.TabIndex = 5;
      this.cmdOK.Text = "OK";
      this.cmdOK.UseVisualStyleBackColor = true;
      this.cmdOK.Click += new EventHandler(this.cmdOK_Click);
      this.cmdCancel.DialogResult = DialogResult.Cancel;
      this.cmdCancel.Location = new Point(164, 93);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new Size(75, 23);
      this.cmdCancel.TabIndex = 6;
      this.cmdCancel.Text = "Cancel";
      this.cmdCancel.UseVisualStyleBackColor = true;
      this.cmdCancel.Click += new EventHandler(this.cmdCancel_Click);
      this.cbxGameTime.AutoSize = true;
      this.cbxGameTime.Checked = true;
      this.cbxGameTime.CheckState = CheckState.Checked;
      this.cbxGameTime.Location = new Point(13, 13);
      this.cbxGameTime.Name = "cbxGameTime";
      this.cbxGameTime.Size = new Size(102, 17);
      this.cbxGameTime.TabIndex = 0;
      this.cbxGameTime.Text = "Use Game Time";
      this.cbxGameTime.UseVisualStyleBackColor = true;
      this.cbxIncludeAnlas.AutoSize = true;
      this.cbxIncludeAnlas.Location = new Point(140, 36);
      this.cbxIncludeAnlas.Name = "cbxIncludeAnlas";
      this.cbxIncludeAnlas.Size = new Size(121, 17);
      this.cbxIncludeAnlas.TabIndex = 3;
      this.cbxIncludeAnlas.Text = "Include Anlas Name";
      this.cbxIncludeAnlas.UseVisualStyleBackColor = true;
      this.cbxIncludeTimeOfDay.AutoSize = true;
      this.cbxIncludeTimeOfDay.Location = new Point(140, 13);
      this.cbxIncludeTimeOfDay.Name = "cbxIncludeTimeOfDay";
      this.cbxIncludeTimeOfDay.Size = new Size(121, 17);
      this.cbxIncludeTimeOfDay.TabIndex = 1;
      this.cbxIncludeTimeOfDay.Text = "Include Time of Day";
      this.cbxIncludeTimeOfDay.UseVisualStyleBackColor = true;
      this.AcceptButton = (IButtonControl) this.cmdOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.cmdCancel;
      this.ClientSize = new Size(292, 138);
      this.Controls.Add((Control) this.cbxIncludeTimeOfDay);
      this.Controls.Add((Control) this.cbxIncludeAnlas);
      this.Controls.Add((Control) this.cbxGameTime);
      this.Controls.Add((Control) this.cmdCancel);
      this.Controls.Add((Control) this.cmdOK);
      this.Controls.Add((Control) this.cbxNames);
      this.Controls.Add((Control) this.cbxElanthiaTime);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.HelpButton = true;
      this.Name = nameof (FormOptions);
      this.Text = "Time Tracker Options";
      this.HelpButtonClicked += new CancelEventHandler(this.FormOptions_HelpButtonClicked);
      this.VisibleChanged += new EventHandler(this.FormOptions_VisibleChanged);
      this.FormClosing += new FormClosingEventHandler(this.FormOptions_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
