using System.Collections.Generic;

namespace IotClientLib
{
    internal class Cpu
    {
        public double Usage { get; set; }
        public double UserUsage { get; set; }
        public double SystemUsage { get; set; }
        public double Idle { get; set; }
        public double Temperature { get; set; }
        public IDictionary<string, Cpu> Cores { get; set; }

    }
    internal class Memory
    {
        public int Total { get; set; }
        public int Cached { get; set; }
        public int Used { get; set; }
        public int Free { get; set; }
        public int Available { get; set; }
        public double UsedPercent { get; set; }
    }
    internal class Sys
    {
        public Cpu Cpu { get; set; }
        public Memory Memory { get; set; }
    }
    internal class PinIO
    {
        public string Pin { get; set; }
        public int Mode { get; set; }
        public int Value { get; set; }
    }
    internal class Gpio
    {
        public IList<PinIO> Pins { get; internal set; }
    }
    internal class UltrasonicData
    {
        public double Value { get; set; }
        public int PosH { get; set; }
        public int PosV { get; set; }
    }
    internal class ScanDistanceData
    {
        public IList<double> Value { get; set; }
        public IList<int> PosH { get; set; }
        public IList<int> PosV { get; set; }
    }

    internal static class JsonHelper
    {
    }
}
