using System;
using System.Threading;
using System.Windows.Forms;

namespace AL0Y_IEEE488_2_Tester
{
    public partial class mainForm : Form
    {
        int testStep = 0;
        public mainForm()
        {
            InitializeComponent();
        }

        private void endBitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                IEEE488Bus.write("BITDME:CH0");
                Thread.Sleep(2000);
                getInitialData();
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
                progresTimer.Start();
                progresTimer.Enabled = true;
                progressBar.Visible = true;
                IEEE488Bus.write("SETBRRT500  ");
                IEEE488Bus.write("SETTGTR5  ");
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
                IEEE488Bus.write("SETBRRT500  ");
                IEEE488Bus.write("SETTGTR1  ");
                testTimer.Stop();
                testTimer.Enabled = false;
                progresTimer.Stop();
                progresTimer.Enabled = false;
                progressBar.Visible = false;
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
            testTimer.Stop();
            testTimer.Enabled = false;
            progressBar.Visible = false;
            progresTimer.Stop();
            progresTimer.Enabled = false;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

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
                responseLabel.Text = IEEE488Bus.fetch(commandTextBox.Text.ToUpper());
            } 
            catch (Exception ex)
            {
                responseLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void progresTimer_Tick(object sender, EventArgs e)
        {
            progressBar.Value += 20;
            if (progressBar.Value > 100)
            {
                progressBar.Value = 0;
            }
        }
    }
}
