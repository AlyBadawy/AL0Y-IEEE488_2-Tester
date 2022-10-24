using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace AL0Y_IEEE488_2_Tester
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void endBitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IEEE488Bus.write("BITDME:CH0");
                Thread.Sleep(500);
                getInitialData();
                Enabled = false;
            }
            catch (Exception ex)
            {
                Enabled = true;
                handleError(ex);
            }
        }

        private void runTestBtn_Click(object sender, EventArgs e)
        {
            // TODO: Writeb command to change range and bearing rates
            testTimer.Start();
            testTimer.Enabled = true;
        }

        private void endTestBtn_Click(object sender, EventArgs e)
        {
            // TODO: Write command to change range and bearing rates to 0.
            testTimer.Stop();
            testTimer.Enabled = false;
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
            try
            {
                IEEE488Bus.write("RSTDME:CH0");
                endBitBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }










        private void testTimer_Tick(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        internal void getInitialData()
        {
            // DTS Mode Data
            // // Missing command for Mode
            // // missing command for inverse
            idToneLabel.Text = IEEE488Bus.fetch("FTHIDTN");
            channleLabel.Text = IEEE488Bus.fetch("FTHCHNL");
            freqLabel.Text = IEEE488Bus.fetch("FTHFREQ");
            presetStatusLabel.Text = IEEE488Bus.fetch("FTHPRST");

            // RF Data
            rfLevelLabel.Text = IEEE488Bus.fetch("FTHRFLV");
            rfPulseLabel.Text = IEEE488Bus.fetch("FTHRFOT");

            // UUT Data
            peakPowerLabel.Text = IEEE488Bus.fetch("FTHPWER");
            aaReplyEffLabel1.Text = IEEE488Bus.fetch("FTHUUTR");
            // // missing command for A/A Reply EFF. (%)
            prfLabel.Text = IEEE488Bus.fetch("FTHUUTP");

            // Range Data
            rangeLabel.Text = IEEE488Bus.fetch("FTHTGTD");
            rangeRateLabel.Text = IEEE488Bus.fetch("FTHTGTR");
            // // A/A Int Rate
            replyEffLabel.Text = IEEE488Bus.fetch("FTHRPLE");
            pulseSpacingLabel.Text = IEEE488Bus.fetch("FTHPLSP");

            // Bearing Data
            bearingLabel.Text = IEEE488Bus.fetch("FTHBRNG");
            bearingRateLabel.Text = IEEE488Bus.fetch("FTHBRRT");
            b15PaseLabel.Text = IEEE488Bus.fetch("FTHPHSE");
            b15ModLabel.Text = IEEE488Bus.fetch("FTHMD15");
            b135ModLabel.Text = IEEE488Bus.fetch("FTHM135");
            mrbStatusLabel.Text = IEEE488Bus.fetch("FTHMRBS");
            arbStatusLabel.Text = IEEE488Bus.fetch("FTHARBS");
            squitterRateLabel.Text = IEEE488Bus.fetch("FTHSQTR");
            // // Missing command for Active Step Size.
        }

        internal void handleError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error communicating with Instrument!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            testTimer.Stop();
            testTimer.Enabled = false;

        }
    }
}
