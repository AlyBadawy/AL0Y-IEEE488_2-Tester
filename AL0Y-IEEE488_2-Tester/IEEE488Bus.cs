using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ivi.Visa;
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
            return instrument;
        }

        internal static string read()
        {
            IMessageBasedSession instrument = initializeInstrument();
            string responseString = "";
            string currentByte;
            bool terminateReading;

            do
            {
                currentByte = instrument.RawIO.ReadString(1);
                byte[] bytes = Encoding.UTF8.GetBytes(currentByte);
                var hexString = BitConverter.ToString(bytes);

                terminateReading = hexString == "0D";
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

        internal static void write(string command)
        {
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
