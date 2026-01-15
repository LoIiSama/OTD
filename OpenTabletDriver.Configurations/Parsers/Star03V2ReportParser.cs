using System;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Configurations.Parsers.XP_Pen
{
    public class Star03V2ReportParser : IReportParser<IDeviceReport>
    {
        private readonly XP_PenReportParser fallback = new();

        public IDeviceReport Parse(byte[] report)
        {
            // Star 03 V2 шлёт 8-байтовые репорты 0x07 или 0x08
            if (report.Length == 8 && (report[0] == 0x07 || report[0] == 0x08))
            {
                bool outOfRange = (report[1] & 0x40) != 0;
                if (outOfRange) return new OutOfRangeReport(report);

                ushort x = (ushort)(report[3] | (report[4] & 0x0F) << 8);
                ushort y = (ushort)(report[5] << 4 | (report[4] & 0xF0) >> 4);
                uint pressure = (uint)(report[6] | report[7] << 8);
                bool pen1 = (report[1] & 0x01) != 0;
                bool pen2 = (report[1] & 0x02) != 0;

                return new XP_PenTabletReport
                {
                    Position = new System.Numerics.Vector2(x, y),
                    Pressure = pressure,
                    PenButtons = new[] { pen1, pen2 }
                };
            }

            // если вдруг придёт не 8-байт – отдаёмся стандартному XP_Pen парсеру
            return fallback.Parse(report);
        }
    }
}
