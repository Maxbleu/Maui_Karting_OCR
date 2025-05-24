using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace MauiApp_Karting_OCR
{
    internal class BoundingBoxDrawable : IDrawable
    {
        public AnalyzeResult AnalysisResult { get; set; }
        public float ImageDisplayWidth { get; set; } 
        public float ImageDisplayHeight { get; set; }

        /// <summary>
        /// Dibuja las líneas y marcas de selección detectadas por OCR sobre el lienzo proporcionado,
        /// aplicando escalado según las dimensiones de la imagen original y mostrada.
        /// </summary>
        /// <param name="canvas">Lienzo de dibujo proporcionado por el sistema.</param>
        /// <param name="dirtyRect">Área sucia que necesita ser redibujada.</param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (AnalysisResult == null || AnalysisResult.Pages.Count == 0)
                return;

            canvas.SaveState(); 

            var page = AnalysisResult.Pages[0];

            float originalImageWidth = (float)(page.Width ?? ImageDisplayWidth);
            float originalImageHeight = (float)(page.Height ?? ImageDisplayHeight);

            if (originalImageWidth <= 0 || originalImageHeight <= 0 || ImageDisplayWidth <= 0 || ImageDisplayHeight <= 0)
            {
                canvas.RestoreState();
                return;
            }

            float scaleX = ImageDisplayWidth / originalImageWidth;
            float scaleY = ImageDisplayHeight / originalImageHeight;

            foreach (var line in page.Lines)
            {
                DrawPolygon(canvas, line.BoundingPolygon, scaleX, scaleY, Colors.Red, 1f);
            }

            foreach (var mark in page.SelectionMarks)
            {
                DrawPolygon(canvas, mark.BoundingPolygon, scaleX, scaleY, Colors.Blue, 1f);
            }

            canvas.RestoreState();
        }
        /// <summary>
        /// Dibuja un polígono escalado sobre el lienzo utilizando una lista de puntos, un color de trazo y un grosor de línea especificados.
        /// </summary>
        /// <param name="canvas">Lienzo donde se dibuja el polígono.</param>
        /// <param name="polygonPoints">Lista de puntos que forman el contorno del polígono.</param>
        /// <param name="scaleX">Factor de escala horizontal.</param>
        /// <param name="scaleY">Factor de escala vertical.</param>
        /// <param name="strokeColor">Color del trazo del polígono.</param>
        /// <param name="strokeSize">Grosor del trazo del polígono.</param>
        private void DrawPolygon(ICanvas canvas,IReadOnlyList<System.Drawing.PointF> polygonPoints,float scaleX, float scaleY,Color strokeColor, float strokeSize)
        {
            if (polygonPoints == null || polygonPoints.Count < 2)
                return;

            PathF path = new PathF();
            path.MoveTo(polygonPoints[0].X * scaleX, polygonPoints[0].Y * scaleY);

            for (int i = 1; i < polygonPoints.Count; i++)
            {
                path.LineTo(polygonPoints[i].X * scaleX, polygonPoints[i].Y * scaleY);
            }
            path.Close();

            canvas.StrokeColor = strokeColor;
            canvas.StrokeSize = strokeSize;
            canvas.DrawPath(path);
        }
    }
}
