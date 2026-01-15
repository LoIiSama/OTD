using System;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Configurations.Parsers.XP_Pen
{
    public class Star03V2ReportParser : IReportParser<IDeviceReport>
    {
        public IDeviceReport Parse(byte[] report)
        {
            if (report.Length < 8 || (report[0] != 0x07 && report[0] != 0x08))
                return new DeviceReport(report);

            bool outOfRange   = (report[1] & 0x40) != 0;
            ushort x          = (ushort)(report[3] | (report[4] & 0x0F) << 8);
            ushort y          = (ushort)(report[5] << 4 | (report[4] & 0xF0) >> 4);
            uint pressure     = (uint)(report[6] | report[7] << 8);
            bool penBtn1      = (report[1] & 0x01) != 0;
            bool penBtn2      = (report[1] & 0x02) != 0;

            return new TabletReport
            {
                Position   = new System.Numerics.Vector2(x, y),
                Pressure   = pressure,
                PenButtons = new bool[] { penBtn1, penBtn2 },
                ExtraButtons = new bool[0]
            };
        }
    }
}