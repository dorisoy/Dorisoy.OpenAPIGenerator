using Microsoft.AspNetCore.JsonPatch.Operations;
using MyToolkit.Model;
using NSwagStudio.Views.CodeGenerators;

namespace NSwagStudio.ViewModels
{
    public class CodeGeneratorModel
    {
        public CodeGeneratorViewBase View { get; set; }
    }


    //public class OperationDataModel : ObservableObject
    //{
    //    public string Id { get; set; }

    //    /// <summary>Gets or sets the HTTP path (i.e. the absolute route).</summary>
    //    public string Path { get; set; }

    //    /// <summary>Gets or sets the HTTP method.</summary>
    //    public string HttpMethod { get; set; }

    //    /// <summary>Gets or sets the name of the operation.</summary>
    //    public string OperationName { get; set; }

    //    public bool IsChecked { get; set; }
    //}
}