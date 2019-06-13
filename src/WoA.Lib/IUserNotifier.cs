using System;
using System.Collections.Generic;
using System.Text;

namespace WoA.Lib
{
    public interface IUserNotifier
    {
        void NotifySomethingNew();
        void ClearNotifications();
        void Toast(string v);
    }
}
