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
        internal static string read(string busAddress)
        {
            IMessageBasedSession instrument;

            string resourceName = "GPIB0::" + busAddress + "::INSTR";
            string responseString = "";
            string currentByte;
            bool terminateReading;

            try
            {
                instrument = GlobalResourceManager.Open(resourceName) as IMessageBasedSession;

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
            }
            catch
            {
                return "";
            }
            return responseString;
        }

        internal static string write(string busAddress, string command)
        {
            IMessageBasedSession instrument;
            string response = "";

            string resourceName = "GPIB0::" + busAddress + "::INSTR";
            try
            {
                instrument = GlobalResourceManager.Open(resourceName) as IMessageBasedSession;
                instrument.RawIO.Write(command);
            }
            catch (Exception exp)
            {
                response = exp.Message;
            }
            return response;
        }
    }   
}
