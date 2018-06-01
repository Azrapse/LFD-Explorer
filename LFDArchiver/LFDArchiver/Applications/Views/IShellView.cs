using LfdArchiver.Domain;
using System.Waf.Applications;

namespace LfdArchiver.Applications.Views
{
    internal interface IShellView : IView
    {
        void Show();

        void Close();

        ResourceEntry[] GetSelectedEntries();
    }
}
