using Colorful;
using System.Drawing;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class StylizedConsole : IStylizedConsole
    {
        private readonly StyleSheet _stylesheet;

        public StylizedConsole()
        {
            _stylesheet = new StyleSheet(Color.White);
            _stylesheet.AddStyle("[\xa0,0-9]+g", Color.Gold);
            _stylesheet.AddStyle("[0-9]+s", Color.Silver);
            _stylesheet.AddStyle("[0-9]+c", Color.Brown);
            _stylesheet.AddStyle("12h", Color.YellowGreen);
            _stylesheet.AddStyle("48h", Color.Green);
            _stylesheet.AddStyle(" 2h", Color.Red);
            _stylesheet.AddStyle("30m", Color.Red);
        }

        public void WriteLine(string line)
        {
            Console.WriteLineStyled(line, _stylesheet);
        }
        
        public void WriteAscii(string line)
        {
            Console.WriteAscii(line, Color.White);
        }

        public void Write(string msg)
        {
            Console.Write(msg, _stylesheet);
        }

        public void Write(string msg, Color color)
        {
            Console.Write(msg, color);
        }
    }
}