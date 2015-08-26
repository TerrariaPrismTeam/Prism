using System;
using System.Collections.Generic;
using System.Linq;

#if !UNIX
using System.Windows;

using WpfMsgBox = System.Windows.MessageBox;
#else
using Gtk;
#endif

namespace Prism
{
    public static class MessageBox
    {
        public static void ShowError(string message)
        {
#if UNIX
            var d = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, message);
            d.Run();
            d.Destroy();
#else
            WpfMsgBox.Show(message, "Prism: Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
        }
    }
}
