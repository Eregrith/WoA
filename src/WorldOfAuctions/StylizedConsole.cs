using Colorful;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class StylizedConsole : IStylizedConsole
    {
        private StyleSheet _stylesheet;
        private ColorAlternator _currentAlternator;
        private Color _savedBackground;
        private readonly List<string> _notifications = new List<string>();
        private readonly IConfiguration _config;

        public StylizedConsole(IConfiguration config)
        {
            _config = config;
            InitStyleSheet();

            Console.SetIn(new StreamReader(Console.OpenStandardInput(8192), Console.InputEncoding, false, 8192));
        }

        public void InitStyleSheet()
        {
            _stylesheet = new StyleSheet(Color.White);
            _stylesheet.AddStyle("[\xa0,0-9]+g", Color.Gold);
            _stylesheet.AddStyle("[0-9]+s", Color.Silver);
            _stylesheet.AddStyle("[0-9]+c", Color.Brown);

            _stylesheet.AddStyle("12h", Color.YellowGreen);
            _stylesheet.AddStyle("48h", Color.Green);
            _stylesheet.AddStyle(" 2h", Color.Red);
            _stylesheet.AddStyle("30m", Color.Red);

            _stylesheet.AddStyle("(" + string.Join("|", _config.PlayerToons) + ")", Color.SteelBlue);
            string itemRegex = @"[a-zA-Z\-0-9 :\(\)\.'" + '"' + "]+";
            _stylesheet.AddStyle($"---{itemRegex}---", Color.Gray, match => match.Substring(3, match.Length - 6));
            _stylesheet.AddStyle($@"==={itemRegex}===", Color.White, match => match.Substring(3, match.Length - 6));
            _stylesheet.AddStyle($@"\[\[\[{itemRegex}\]\]\]", Color.Green, match => match.Substring(3, match.Length - 6));
            _stylesheet.AddStyle($@"\{{\{{\{{{itemRegex}\}}\}}\}}", Color.DeepSkyBlue, match => match.Substring(3, match.Length - 6));
            _stylesheet.AddStyle($@"\+\+\+{itemRegex}\+\+\+", Color.MediumVioletRed, match => match.Substring(3, match.Length - 6));
            _stylesheet.AddStyle($@"\{{\+\+{itemRegex}\+\+\}}", Color.Orange, match => match.Substring(3, match.Length - 6));
        }

        public void WriteLine(string line)
        {
            Console.WriteLineStyled(line, _stylesheet);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }
        public void WriteNotificationLine(string line)
        {
            _notifications.Add($"[Notif {System.DateTime.Now.ToShortTimeString()}] " + line);
        }

        public void WriteAscii(string line)
        {
            Console.WriteAscii(RemoveDiacritics(line), Color.White);
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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

        public void FlushNotificationLines()
        {
            List<string> snapshotOfNotifications = _notifications.ToList();
            foreach (string notification in snapshotOfNotifications)
            {
                Console.WriteLineStyled(notification, _stylesheet);
            }
            _notifications.RemoveAll(n => snapshotOfNotifications.Contains(n));
        }
    }
}