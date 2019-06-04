using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoA.Lib;

namespace WorldOfAuctions
{
    public class ClipboardManager : IClipboardManager
    {
        public void SetText(string text)
        {
            System.Windows.Forms.Clipboard.SetText(text);
        }
    }
}
