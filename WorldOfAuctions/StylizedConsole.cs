using Colorful;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class StylizedConsole : IStylizedConsole
    {
        private readonly StyleSheet _stylesheet;

        public StylizedConsole()
        {
            _stylesheet = new StyleSheet(System.Drawing.Color.White);
            _stylesheet.AddStyle("[\xa0,0-9]+g", System.Drawing.Color.Gold);
            _stylesheet.AddStyle("[0-9]+s", System.Drawing.Color.Silver);
            _stylesheet.AddStyle("[0-9]+c", System.Drawing.Color.Brown);
        }

        public void WriteLine(string line)
        {
            Console.WriteLineStyled(line, _stylesheet);
        }
        
        public void WriteAscii(string line)
        {
            Console.WriteAscii(line);
        }
    }
}