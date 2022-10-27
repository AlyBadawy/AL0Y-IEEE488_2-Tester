using System;
using System.Threading;
using System.Windows.Forms;

namespace AL0Y_IEEE488_2_Tester
{
    public partial class mainForm : Form
    {
        int testStep = 0;
        string instType;
        public mainForm()
        {
            InitializeComponent();
        }

        private void endBitBtn_Click(object sender, EventArgs e)
        {
            responseLabel.Text = "(1/4) Ending BIT mode...";
            try
            {
                IEEE488Bus.write("BITDME:CH0");
                responseLabel.Text = "(2/4) Resetting buffer";
                IEEE488Bus.clearBuffer();
                Thread.Sleep(2000);
                responseLabel.Text = "(3/4) Getting instrument Type";
                instType = IEEE488Bus.fetch("GETCFG ");
                instrumentTypeLabel.Text = instType;
                Thread.Sleep(1000);
                responseLabel.Text = "(3/4) Getting initial data...";
                getInitialData();
                responseLabel.Text = "Ready...";
                runTestBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void runTestBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IEEE488Bus.write("SETBRRT5  ");
                IEEE488Bus.write("SETTGTR1500  ");
                IEEE488Bus.clearBuffer();
                testTimer.Start();
                testTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void endTestBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IEEE488Bus.write("SETBRRT0  ");
                IEEE488Bus.write("SETTGTR0  ");
                IEEE488Bus.clearBuffer();

                testTimer.Stop();
                testTimer.Enabled = false;

                getRepeatingData();


            }
            catch (Exception ex)
            {
                handleError(ex);
            }

        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
            try
            {
                IEEE488Bus.write("RSTDME:CH0");
                endBitBtn.Enabled = true;
                responseLabel.Text = "Ready...";
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        internal void testTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                getRepeatingData();
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }



        internal void getRepeatingData()
        {

            rangeLabel.Text = IEEE488Bus.fetch("FTHTGTD");
            rangeRateLabel.Text = IEEE488Bus.fetch("FTHTGTR");
            bearingLabel.Text = IEEE488Bus.fetch("FTHBRNG");
            bearingRateLabel.Text = IEEE488Bus.fetch("FTHBRRT");
        }

        internal void getInitialData()
        {
            //// DTS Mode Data
            // Missing command for Mode (check FTHPDME
            modeLabel.Text = "Normal";

            // Missing command for inverse
            inverseLabel.Text = "No";

            idToneLabel.Text = IEEE488Bus.fetch("FTHIDTN");
            if (idToneLabel.Text == "O") idToneLabel.Text = "On";
            if (idToneLabel.Text == "F") idToneLabel.Text = "Off";

            string channel = IEEE488Bus.fetch("FTHCHNL");
            channel = channel.Substring(0, channel.Length - 1);
            string mode = IEEE488Bus.fetch("FTHXYMD");
            mode = mode.Substring(1, mode.Length - 1);
            channleLabel.Text = $"{channel}({mode})";

            freqLabel.Text = IEEE488Bus.fetch("FTHFREQ").ToString();

            presetStatusLabel.Text = IEEE488Bus.fetch("FTHPRST");
            if (presetStatusLabel.Text == "Y") presetStatusLabel.Text = "Yes";
            if (presetStatusLabel.Text == "N") presetStatusLabel.Text = "No";

            //// RF Data
            rfLevelLabel.Text = IEEE488Bus.fetch("FTHRFLV");

            rfPulseLabel.Text = IEEE488Bus.fetch("FTHRFOT");
            if (rfPulseLabel.Text == "C") rfPulseLabel.Text = "CW";
            if (rfPulseLabel.Text == "P") rfPulseLabel.Text = "Pulse";

            //// UUT Data
            peakPowerLabel.Text = IEEE488Bus.fetch("FTHPWER");

            aaReplyEffLabel1.Text = "N/A";
            aaReplyEffLabel2.Text = "N/A";
            aaIntLabel.Text = "N/A";

            // Missing command for A/A Reply EFF. (%)

            prfLabel.Text = IEEE488Bus.fetch("FTHUUTP");

            //// Range Data
            rangeLabel.Text = IEEE488Bus.fetch("FTHTGTD");
            rangeRateLabel.Text = IEEE488Bus.fetch("FTHTGTR");

            // Missig command for A/A Int Rate
            replyEffLabel.Text = IEEE488Bus.fetch("FTHRPLE");

            pulseSpacingLabel.Text = IEEE488Bus.fetch("FTHPLSP");

            // Bearing Data
            bearingLabel.Text = IEEE488Bus.fetch("FTHBRNG");

            bearingRateLabel.Text = IEEE488Bus.fetch("FTHBRRT");

            b15PaseLabel.Text = IEEE488Bus.fetch("FTHPHSE");
            b15ModLabel.Text = IEEE488Bus.fetch("FTHMD15");
            b135ModLabel.Text = IEEE488Bus.fetch("FTHM135");
            mrbStatusLabel.Text = IEEE488Bus.fetch("FTHMRBS");
            if (mrbStatusLabel.Text == "O") mrbStatusLabel.Text = "On";
            if (mrbStatusLabel.Text == "F") mrbStatusLabel.Text = "Off";
            arbStatusLabel.Text = IEEE488Bus.fetch("FTHARBS");
            if (arbStatusLabel.Text == "O") arbStatusLabel.Text = "On";
            if (arbStatusLabel.Text == "F") arbStatusLabel.Text = "Off";
            squitterRateLabel.Text = IEEE488Bus.fetch("FTHSQTR");
            // Missing command for Active Step Size.

            activeStepLabel.Text = "+1";


            // Flight Inspection Unit
            if (instType == "F")
            {
                rfPortLabel.Text = IEEE488Bus.fetch("FTHRFSW");
                antennaSpdLabel.Text = IEEE488Bus.fetch("FTHANTS");
                mrbDeviationLabel.Text = IEEE488Bus.fetch("FTHMRBF");
                mrbSpacingDevLabel.Text = IEEE488Bus.fetch("FTHMRPS");
                arbCountLabel.Text = IEEE488Bus.fetch("FTHARBC");
                arbSizeLabel.Text = IEEE488Bus.fetch("FTHARBF");
                intExtBurstLabel.Text = IEEE488Bus.fetch("FTHBRST");
                if (intExtBurstLabel.Text == " I") intExtBurstLabel.Text = "Int.";
                if (intExtBurstLabel.Text == " E") intExtBurstLabel.Text = "Ext.";
            }
        }

        internal void handleError(Exception ex)
        {
            testTimer.Stop();
            testTimer.Enabled = false;
            responseLabel.Text = $"Error: {ex.Message}";

        }

        private void mainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                switch (testStep)
                {
                    case 0:
                        endBitBtn.PerformClick();
                        testStepStatus.Text = "Press SPACE to start test";
                        break;
                    case 1:
                        runTestBtn.PerformClick();
                        testStepStatus.Text = "Press SPACE to end test";
                        break;
                    case 2:
                        endTestBtn.PerformClick();
                        testStepStatus.Text = "Press SPACE to reset instrument";
                        break;
                    case 3:
                        resetBtn.PerformClick();
                        testStepStatus.Text = "Press SPACE to exit BIT mode";
                        break;
                    default:
                        break;
                }
                testStep++;
                if (testStep >= 4)
                {
                    testStep = 0;
                }
            }
        }

        private void commandTextBox_TextChanged(object sender, EventArgs e)
        {
            sendBtn.Enabled = Text.Length != 0;
        }

        private void commandTextBox_Enter(object sender, EventArgs e)
        {
            KeyPreview = false;

        }

        private void commandTextBox_Leave(object sender, EventArgs e)
        {
            KeyPreview = true;
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string response = IEEE488Bus.fetch(commandTextBox.Text.ToUpper());
                responseLabel.Text = response;
            }
            catch (Exception ex)
            {
                responseLabel.Text = $"Error: {ex.Message}";
            }
        }
    }
}
