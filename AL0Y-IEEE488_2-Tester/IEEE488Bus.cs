using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ivi.Visa;
using alyBadawy;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AL0Y_IEEE488_2_Tester
{
    internal class IEEE488Bus
    {

        internal static IMessageBasedSession initializeInstrument()
        {
            IMessageBasedSession instrument;
            string busAddress = Properties.Settings.Default.busAddress;
            string resourceName = "GPIB0::" + busAddress + "::INSTR";
            instrument = GlobalResourceManager.Open(resourceName) as IMessageBasedSession;
            instrument.TimeoutMilliseconds = 25;
            instrument.TerminationCharacter = alyBadawy.HexParser.StringToByteArray("0A")[0];
            instrument.TerminationCharacterEnabled = true;
            return instrument;
        }

        internal static string read()
        {
            Thread.Sleep(Properties.Settings.Default.waitBeforeRead);

            IMessageBasedSession instrument = initializeInstrument();
            string responseString = "";
            string currentByte;
            bool terminateReading;

            do
            {
                currentByte = instrument.RawIO.ReadString(1);
                var hexString =  HexParser.parse(currentByte);

                terminateReading = (hexString == "0D" || responseString.Length > Properties.Settings.Default.maxResponseSize);
                if (terminateReading)
                {
                    instrument.RawIO.ReadString(1);
                }
                else
                {
                    responseString += currentByte;
                }

            } while (!terminateReading);

            return responseString;
        }

        internal static string readTillBufferEnd()
        {
            Thread.Sleep(Properties.Settings.Default.waitBeforeRead);

            IMessageBasedSession instrument = initializeInstrument();
            string responseString = "";
            string currentByte;
            try
            {
                while (true)
                {
                    currentByte = instrument.RawIO.ReadString(1);
                    responseString += currentByte;

                }
            }
            catch (Ivi.Visa.IOTimeoutException)
            {
                return responseString;
            }
        }

        internal static string readTillTerminator()
        {
            Thread.Sleep(Properties.Settings.Default.waitBeforeRead);

            IMessageBasedSession instrument = initializeInstrument();
            return instrument.RawIO.ReadString();
        }

        internal static void clearBuffer()
        {
            readTillTerminator();
            //readTillBufferEnd();
        }

        internal static void write(string command)
        {
            Thread.Sleep(Properties.Settings.Default.waitBeforeWrite);
            IMessageBasedSession instrument = initializeInstrument();
            instrument.RawIO.Write(command);
        }

        internal static string fetch(string command)
        {
            write(command);
            return read();
        }
    }
}