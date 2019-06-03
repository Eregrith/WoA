using System.Collections.Generic;
using System.Drawing;

namespace WoA.Lib
{
    public interface IStylizedConsole
    {
        void WriteLine(string line);
        void WriteAscii(string line);
        void Write(string msg);
        void Write(string msg, Color color);
        void StartAlternating();
        void WriteLineWithAlternatingBackground(string line);
        void StopAlternating();
    }
}