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
            endBitWorker.RunWorkerAsync();
        }

        private void getData_Click(object sender, EventArgs e)
        {
            readValuesWorker.RunWorkerAsync();
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

                repeatingDataWorker.RunWorkerAsync();
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
            resetInstWorker.RunWorkerAsync();
        }

        internal void testTimer_Tick(object sender, EventArgs e)
        {
            repeatingDataWorker.RunWorkerAsync();
        }

        internal void handleError(Exception ex)
        {
            Action action;
            testTimer.Stop();
            testTimer.Enabled = false;

            action = () => responseLabel.Text = $"Error: {ex.Message}";
            responseLabel.Invoke(action);
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

        private void endBitWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Action action;
            try
            {
                action = () => responseLabel.Text = "(1/4) Ending BIT mode...";
                responseLabel.BeginInvoke(action);
                IEEE488Bus.write("BITDME:CH0");

                action = () => responseLabel.Text = "(2/4) Resetting buffer";
                responseLabel.BeginInvoke(action);
                IEEE488Bus.clearBuffer();
                Thread.Sleep(2000);
                
                action = () => responseLabel.Text = "(3/4) Getting instrument Type";
                responseLabel.BeginInvoke(action);
                instType = IEEE488Bus.fetch("GETCFG ");
                if (instType == "F") instType = "Flight Inspection Unit";
                if (instType == "N") instType = "DTS-200";
                if (instType == "200c") instType = "DTS-200c";
                action = ()=> instrumentTypeLabel.Text = instType;
                instrumentTypeLabel.BeginInvoke(action);

                Thread.Sleep(1000);
                action = () => responseLabel.Text = "(3/4) Getting initial data...";
                responseLabel.BeginInvoke(action);
                readValuesWorker.RunWorkerAsync();

                action = () => responseLabel.Text = "Ready...";
                responseLabel.BeginInvoke(action);
                runTestBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void readValuesWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Action action;
            try
            {
                action = () => modeLabel.Text = "Normal";
                modeLabel.BeginInvoke(action);
                action = () => inverseLabel.Text = "No";
                inverseLabel.BeginInvoke(action);

                string idTone = IEEE488Bus.fetch("FTHIDTN");
                if (idTone == "O") idTone = "On";
                if (idTone == "F") idTone = "Off";
                action = () => idToneLabel.Text = idTone;
                idToneLabel.BeginInvoke(action);


                string channel = IEEE488Bus.fetch("FTHCHNL");
                channel = channel.Substring(0, channel.Length - 1);
                string mode = IEEE488Bus.fetch("FTHXYMD");
                mode = mode.Substring(1, mode.Length - 1);
                string channelMode = $"{channel}({mode})";
                action = () => channleLabel.BeginInvoke(action);

                string freq = IEEE488Bus.fetch("FTHFREQ");
                action = () => freqLabel.Text = freq;
                freqLabel.BeginInvoke(action);

                string presetStatus = IEEE488Bus.fetch("FTHPRST");
                if (presetStatus == "Y") presetStatus = "Yes";
                if (presetStatus == "N") presetStatus = "No";
                action = () => presetStatusLabel.Text = presetStatus;
                presetStatusLabel.BeginInvoke(action);

                //// RF Data
                string rfLEvel = IEEE488Bus.fetch("FTHRFLV");
                action = () => rfLevelLabel.Text = rfLEvel;
                rfLevelLabel.BeginInvoke(action);

                string rfPulse = IEEE488Bus.fetch("FTHRFOT");
                if (rfPulse == "C") rfPulse = "CW";
                if (rfPulse == "P") rfPulse = "Pulse";
                action = () => rfPulseLabel.Text = rfPulse;
                rfPulseLabel.BeginInvoke(action);

                //// UUT Data
                string peakPower = IEEE488Bus.fetch("FTHPWER");
                action = () => peakPowerLabel.Text = peakPower;
                peakPowerLabel.BeginInvoke(action);

                action = () => aaReplyEffLabel1.Text = "N/A";
                aaReplyEffLabel1.BeginInvoke(action);

                action = () => aaReplyEffLabel2.Text = "N/A";
                aaReplyEffLabel2.BeginInvoke(action);

                action = () => aaIntLabel.Text = "N/A";
                aaIntLabel.BeginInvoke(action);

                string prf = IEEE488Bus.fetch("FTHUUTP");
                action = () => prfLabel.Text = prf;
                prfLabel.BeginInvoke(action);

                //// Range Data
                string range = IEEE488Bus.fetch("FTHTGTD");
                action = () => rangeLabel.Text = range;
                rangeLabel.BeginInvoke(action);

                string rangeRate = IEEE488Bus.fetch("FTHTGTR");
                action = () => rangeRateLabel.Text = rangeRate;
                rangeRateLabel.BeginInvoke(action);

                string replyEff = IEEE488Bus.fetch("FTHRPLE");
                action = () => replyEffLabel.Text = replyEff;
                replyEffLabel.BeginInvoke(action);

                string pulseSpacing = IEEE488Bus.fetch("FTHPLSP");
                action = () => pulseSpacingLabel.Text = pulseSpacing;
                pulseSpacingLabel.BeginInvoke(action);

                // Bearing Data
                string bearing = IEEE488Bus.fetch("FTHBRNG");
                action = () => bearingLabel.Text = bearing;
                bearingLabel.BeginInvoke(action);

                string bearingRate = IEEE488Bus.fetch("FTHBRRT");
                action = () => bearingRateLabel.Text = bearingRate;
                bearingRateLabel.BeginInvoke(action);

                string b15Phase = IEEE488Bus.fetch("FTHPHSE");
                action = () => b15PaseLabel.Text = b15Phase;
                b15PaseLabel.BeginInvoke(action);

                string b15mod = IEEE488Bus.fetch("FTHMD15");
                action = () => b15ModLabel.Text = b15mod;
                b15ModLabel.BeginInvoke(action);

                string b135mod = IEEE488Bus.fetch("FTHM135");
                action = () => b135ModLabel.Text = b135mod;
                b135ModLabel.BeginInvoke(action);

                string mrbStatus = IEEE488Bus.fetch("FTHMRBS");
                if (mrbStatus == "O") mrbStatus = "On";
                if (mrbStatus == "F") mrbStatus = "Off";
                action = () => mrbStatusLabel.Text = mrbStatus;
                mrbStatusLabel.BeginInvoke(action);

                string arbStatus = IEEE488Bus.fetch("FTHMRBS");
                if (arbStatus == "O") arbStatus = "On";
                if (arbStatus == "F") arbStatus = "Off";
                action = () => arbStatusLabel.Text = arbStatus;
                arbStatusLabel.BeginInvoke(action);

                string squitter = IEEE488Bus.fetch("FTHSQTR");
                action = () => squitterRateLabel.Text = squitter;
                squitterRateLabel.BeginInvoke(action);

                action = () => activeStepLabel.Text = "+1";
                activeStepLabel.BeginInvoke(action);


                // Flight Inspection Unit
                if (instType == "Flight Inspection Unit")
                {
                    string rfPort = IEEE488Bus.fetch("FTHRFSW");
                    action = () => rfPortLabel.Text = rfPort;
                    rfPortLabel.BeginInvoke(action);

                    string antennaSpeed = IEEE488Bus.fetch("FTHANTS");
                    action = () => antennaSpdLabel.Text = antennaSpeed;
                    antennaSpdLabel.BeginInvoke(action);

                    string mrbDev = IEEE488Bus.fetch("FTHMRBF");
                    action = () => mrbDeviationLabel.Text = mrbDev;
                    mrbDeviationLabel.BeginInvoke(action);

                    string mrbSpace = IEEE488Bus.fetch("FTHMRPS");
                    action = () => mrbSpacingDevLabel.Text = mrbSpace;
                    mrbSpacingDevLabel.BeginInvoke(action);

                    string arbCount = IEEE488Bus.fetch("FTHARBC");
                    action = () => arbCountLabel.Text = arbCount;
                    arbCountLabel.BeginInvoke(action);

                    string arbSize = IEEE488Bus.fetch("FTHARBF");
                    action = () => arbSizeLabel.Text = arbSize;
                    arbSizeLabel.BeginInvoke(action);

                    string intExtBurst = IEEE488Bus.fetch("FTHBRST");
                    if (intExtBurst == " I") intExtBurst = "Int.";
                    if (intExtBurst == " E") intExtBurst = "Ext.";
                    action = () => intExtBurstLabel.Text = intExtBurst;
                    intExtBurstLabel.BeginInvoke(action);
                }
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void resetInstWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Action action;
            try
            {
                action = () => responseLabel.Text = "Resetting instrument...";
                responseLabel.BeginInvoke(action);
                IEEE488Bus.write("RSTDME:CH0");

                action = () => endBitBtn.Enabled = true;
                endBitBtn.BeginInvoke(action);

                Thread.Sleep(1000);
                action = () => responseLabel.Text = "Ready...";
                responseLabel.BeginInvoke(action);
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }

        private void repeatingDataWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Action action;
            try
            {
                string range = IEEE488Bus.fetch("FTHTGTD");
                action = () => rangeLabel.Text = range;
                rangeLabel.BeginInvoke(action);

                string rangeRate = IEEE488Bus.fetch("FTHTGTR");
                action = () => rangeRateLabel.Text = range;
                rangeRateLabel.BeginInvoke(action);

                string bearing = IEEE488Bus.fetch("FTHBRNG");
                action = () => bearingLabel.Text = range;
                bearingLabel.BeginInvoke(action);

                string bearingRate = IEEE488Bus.fetch("FTHBRRT");
                action = () => bearingRateLabel.Text = range;
                bearingRateLabel.BeginInvoke(action);
            }
            catch (Exception ex)
            {
                handleError(ex);
            }
        }
    }
}
