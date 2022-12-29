using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyToolkit.Command;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using NSwag.Commands.Generation;
using NSwag.CodeGeneration.Models;
using System.Collections.Generic;
using NSwag;
using System.Linq;

namespace NSwagStudio.ViewModels.SwaggerGenerators
{
    public class SwaggerInputViewModel : ViewModelBase
    {
        public FromDocumentCommand Command { get; set; }
        public ICommand LoadSwaggerUrlCommand { get; }
        public ObservableCollection<OperationDataModel> OperationDatas { get; set; }


        private int _operationDataCount;
        public int OperationDataCount
        {
            get { return _operationDataCount; }
            set { Set(ref _operationDataCount, value); }
        }


        public SwaggerInputViewModel()
        {
            LoadSwaggerUrlCommand = new AsyncRelayCommand<string>(LoadSwaggerUrlAsync);
            OperationDatas = new ObservableCollection<OperationDataModel>();
        }




        public async Task LoadSwaggerUrlAsync(string url)
        {
            var json = string.Empty;
            var selections= new List<OperationDataModel>();
            var document = new OpenApiDocument();

            await RunTaskAsync(async () =>
            {
                json = url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                       url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) ?
                    await DynamicApis.HttpGetAsync(url, CancellationToken.None) : DynamicApis.FileReadAllText(url);

                if (json.StartsWith("{"))
                {
                    json = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
                }

                document = await Command.RunAsync().ConfigureAwait(false);

                selections = GetOperations(document).ToList();

                OperationDataCount = selections.Count();
                OperationDatas = new ObservableCollection<OperationDataModel>(selections);
            });

            Command.Json = json;
            Command.Selections = selections;
        }


        private IEnumerable<OperationDataModel> GetOperations(OpenApiDocument document)
        {
            document.GenerateOperationIds();

            foreach (var pair in document.Paths)
            {
                foreach (var p in pair.Value.ActualPathItem)
                {
                    var path = pair.Key.TrimStart('/');
                    var httpMethod = p.Key;
                    var operation = p.Value;
                    var operationName = operation.OperationId;

                    if (operationName.Contains("."))
                    {
                        operationName = operationName.Replace(".", "_");
                    }

                    if (operationName.EndsWith("Async"))
                    {
                        operationName = operationName.Substring(0, operationName.Length - "Async".Length);
                    }
     
                    yield return new OperationDataModel
                    {
                        Path = path,
                        HttpMethod = httpMethod,
                        OperationName = operationName
                    };
                }
            }

            yield break;
        }

        public async Task<string> GenerateSwaggerAsync()
        {
            return await RunTaskAsync(async () =>
            {
                return await Task.Run(async () =>
                {
                    var document = await Command.RunAsync().ConfigureAwait(false);
                    return document.ToJson();
                });
            });
        }
    }
}