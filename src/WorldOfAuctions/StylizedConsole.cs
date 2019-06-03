using Colorful;
using System.Drawing;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class StylizedConsole : IStylizedConsole
    {
        private readonly StyleSheet _stylesheet;
        private ColorAlternator _currentAlternator;
        private Color _savedBackground;

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

        public void StartAlternating()
        {
            ColorAlternatorFactory factory = new ColorAlternatorFactory();
            _currentAlternator = factory.GetAlternator(1, Color.DarkSlateGray, Color.Black);
            _savedBackground = Console.BackgroundColor;
        }

        public void WriteLineWithAlternatingBackground(string line)
        {
            if (_currentAlternator != null)
                Console.BackgroundColor = _currentAlternator.GetNextColor(line);
            Console.WriteLineStyled(line, _stylesheet);
        }

        public void StopAlternating()
        {
            Console.BackgroundColor = _savedBackground;
            _currentAlternator = null;
        }
    }
}