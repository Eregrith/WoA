using System.Collections.Generic;

namespace WoA.Lib
{
    public interface IStylizedConsole
    {
        void WriteLine(string line);
        void WriteAscii(string line);
        void Write(string msg);
    }
}