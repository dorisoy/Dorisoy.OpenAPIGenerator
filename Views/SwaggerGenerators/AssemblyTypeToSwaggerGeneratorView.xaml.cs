using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwag.Commands.Generation;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class AssemblyTypeToSwaggerGeneratorView : ISwaggerGeneratorView
    {
        public AssemblyTypeToSwaggerGeneratorView(TypesToSwaggerCommand command, NSwagDocument document)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);

            Model.Command = command;
            Model.Document = document;

            ControllersList.SelectedItems.Clear();
            foreach (var controller in Model.ClassNames)
                ControllersList.SelectedItems.Add(controller);
        }

        private AssemblyTypeToSwaggerGeneratorViewModel Model => (AssemblyTypeToSwaggerGeneratorViewModel)Resources["ViewModel"];

        public string Title => "使用.NET程序集";

        public IOutputCommand Command => Model.Command;

        public Task<string> GenerateSwaggerAsync()
        {
            return Model.GenerateSwaggerAsync();
        }

        public override string ToString()
        {
            return Title;
        }

        private void ControllersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.ClassNames = ((ListBox)sender).SelectedItems.OfType<string>().ToArray();
        }
    }
}
