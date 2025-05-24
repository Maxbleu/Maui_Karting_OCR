using CommunityToolkit.Maui.Alerts;

namespace MauiApp_Karting_OCR.Utils
{
    public static class GuiUtils
    {
        /// <summary>
        /// Este método se encarga de mostrar mensajes
        /// a partir del Sankbar
        /// </summary>
        /// <param name="message"></param>
        public static void SendSnakbarMessage(string message)
        {
            Task.Run(async () =>
            {
                await Snackbar.Make(message).Show();
            });
        }
        /// <summary>
        /// Este método se encarga de mostrar en una
        /// página en específico una alerta y
        /// dependiendo de lo que elija el usuario
        /// se devolverá un true o false
        /// </summary>
        /// <param name="page"></param>
        /// <param name="titulo"></param>
        /// <param name="message"></param>
        /// <param name="textOkButton"></param>
        /// <param name="textCancelButton"></param>
        /// <returns></returns>
        public async static Task<string> DisplayActionSheet(Page page, string titulo, string textOkButton, string textCancelButton, string[] actions)
        {
            return await page.DisplayActionSheet(
                titulo,
                textOkButton,
                textCancelButton,
                actions
            );
        }
    }
}
