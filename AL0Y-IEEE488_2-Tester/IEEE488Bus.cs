using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ivi.Visa;


namespace AL0Y_IEEE488_2_Tester
{
    internal class IEEE488Bus
    {

        internal static IMessageBasedSession initializeInstrument()
        {
            IMessageBasedSession instrument;
            string busAddress = Properties.Settings.Default.busAddress;
            string resourceName = $"GPIB0::{busAddress}::INSTR";

            instrument = GlobalResourceManager.Open(resourceName) as IMessageBasedSession;
            instrument.TimeoutMilliseconds = Properties.Settings.Default.TimeoutMilliseconds;
            instrument.TerminationCharacter = Properties.Settings.Default.terminatinByte;
            instrument.TerminationCharacterEnabled = true;
            return instrument;
        }

        internal static string read()
        {
            Thread.Sleep(Properties.Settings.Default.waitBeforeRead);

            IMessageBasedSession instrument = initializeInstrument();
            return instrument.RawIO.ReadString();
        }

        internal static void clearBuffer()
        {
            try
            {
                read();
            }
            catch (Ivi.Visa.IOTimeoutException)
            { }
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
            return read().Trim();
        }
    }
}