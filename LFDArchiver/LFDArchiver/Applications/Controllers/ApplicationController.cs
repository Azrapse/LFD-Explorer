using System.ComponentModel.Composition;
using LfdArchiver.Applications.ViewModels;
using LfdArchiver.Domain;

namespace LfdArchiver.Applications.Controllers
{
    [Export]
    internal class ApplicationController
    {
        private readonly ShellViewModel shellViewModel;
        private ResourceArchive archive;

        [ImportingConstructor]
        public ApplicationController(ShellViewModel shellViewModel)
        {
            this.shellViewModel = shellViewModel;
        }



        public void Initialize()
        {
            archive = new ResourceArchive();
            shellViewModel.Archive = archive;            
        }

        public void Run()
        {
            shellViewModel.Show();
        }

        public void Shutdown()
        {
        }


    }
}
