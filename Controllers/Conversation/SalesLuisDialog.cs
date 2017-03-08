using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstBot.Controllers.Conversation
{
    [LuisModel("", "")]
    [Serializable]
    public class SalesLuisDialog : LuisDialog<object>
    {
        private Func<IForm<object>> buildForm;

        public SalesLuisDialog(Func<IForm<object>> buildForm)
        {
            this.buildForm = buildForm;
            var form = buildForm();
        }

        [LuisIntent("Sales")]
        public async Task IntentSales(IDialogContext context, LuisResult result)
        {
            //var technologyEntity = (from entity in result.Entities where entity.Type == "Produto" select entity).FirstOrDefault();

            var form = new FormDialog<CakeSalesEnquiry>(
                new CakeSalesEnquiry() {},
                CakeSalesEnquiry.BuildForm,
                FormOptions.PromptInStart
            );

            context.Call(form, async (formContext, formResult) =>
            {
                var order = await formResult;
                formContext.Wait(MessageReceived);
            });
        }

        [LuisIntent("Greetings")]
        public async Task IntentGreetings(IDialogContext context, LuisResult result)
        {
            var replyMessage = "Olá, tudo bem?\n\n";
            replyMessage += "Eu sou SweetBot. Desenvolvido para anotar seus pedidos na Black Marble Cakes.\n\n";
            replyMessage += "Atualmente estamos trabalhando com 3 sabores de massas (Baunilha, Chocolate, Festa).\n\n";
            replyMessage += "E com 5 sabores de recheios (Brigadeiro, Ninho, Nêmesis, Floresta Negra, Marta Rocha).\n\n";
            replyMessage += "Caso necessite de ajuda para realizar seu pedido, digite 'ajuda'.\n\n";
            await context.PostAsync(replyMessage);
            context.Wait(MessageReceived);
        }

        [LuisIntent("ReadMenu")]
        public async Task IntentReadMenu(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("<< MENU >>");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task IntentHelp(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("<< AJUDA >>");
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task IntentNone(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Desculpe, não entendi o que você quis dizer. Digite 'ajuda' para que eu possa te ajudar a encontrar o que você procura.");
            context.Wait(MessageReceived);
        }
    }
}