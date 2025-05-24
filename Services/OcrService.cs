using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace MauiApp_Karting_OCR.Services
{
    public class OcrService
    {
        private const string Endpoint = "https://37202-maldtts9-eastus2.cognitiveservices.azure.com/";
        private const string ApiKey = "9dPR9kovcRkEOdCzzDJtcQlCcGdhJVZLjIUJfZdQ5YqxPIRpYZT0JQQJ99BEACHYHv6XJ3w3AAAAACOGUCWL";
        private readonly DocumentAnalysisClient _client;

        public OcrService()
        {
            this._client = new DocumentAnalysisClient(new Uri(Endpoint), new AzureKeyCredential(ApiKey));
        }
        /// <summary>
        /// Analiza una imagen utilizando un modelo OCR predefinido para extraer texto estructurado.
        /// </summary>
        /// <param name="imageBytesStream">Flujo de memoria que contiene los bytes de la imagen a analizar.</param>
        /// <returns>
        /// Un objeto <see cref="AnalyzeResult"/> con el resultado del análisis de texto.
        /// Devuelve <c>null</c> si ocurre un error durante el análisis.
        /// </returns>
        public async Task<AnalyzeResult> RecognizeTextAsync(MemoryStream imageBytesStream)
        {
            AnalyzeResult result = null;
            try
            {
                var op = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-document", imageBytesStream);
                result = op.Value;

            }catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
    }
}
