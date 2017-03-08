using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;

namespace MyFirstBot.Controllers.Conversation
{
    public enum OpcoesSabores
    {
        Chocolate, Baunilha, Fubá
    };

    public enum OpcoesPeso
    {
        MeioQuilo, UmQuilo, UmQuiloEMeio, DoisQuilos
    };

    public enum OpcoesRecheio
    {
        Nêmesis,
        FlorestaNegra,
        MarthaRocha,
        Brigadeiro,
        LeiteNinho,
        LeiteCondensado,
        MorangoComLeiteCondensado        
    };

    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Não entendi. \"{0}\".", "Tente novamente, eu não entendi \"{0}\".")]
    [Template(TemplateUsage.EnumSelectOne, "Qual sabor de {&} você gostaria no seu bolo? {||}")]
    public class CakeSalesEnquiry
    {
        [Prompt("Quanto gostaria que pesasse seu bolo? {||}")]
        public OpcoesPeso? Peso;

        public OpcoesSabores? Massa;

        public OpcoesRecheio? Recheio;

        [Prompt("Digite um número de telefone para contato, por favor. (xx xxxxx-xxxx)")]
        [Pattern(@"(\d)?\s*\d{5}(-|\s*)\d{4}")]
        public string Telefone;

        [Prompt("Antes de começarmos, qual seu nome?")]
        public string Nome;

        [Prompt("Por favor insira o endereço da entrega:")]
        public string EnderecoEntrega;

        [Template(TemplateUsage.StatusFormat, "{&}: {:t}", FieldCase = CaseNormalization.None)]
        public DateTime? HoraEntrega;

        public static IForm<CakeSalesEnquiry> BuildForm()
        {
            var builder = new FormBuilder<CakeSalesEnquiry>();
            builder.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.Auto;
            builder.Configuration.Yes = new string[] { "yes", "sim" };
            builder.Configuration.No = new string[] { "no", "não" };
            
            return builder
                    .Message("Olá, obrigado por entrar em contato conosco.")
                    .Field(nameof(Nome))
                    .Field(nameof(Peso))
                    .Field(nameof(Massa))                        
                    .Field(nameof(Recheio))
                    .Field(nameof(EnderecoEntrega))
                    .Field(nameof(HoraEntrega), "Quando você gostaria que fosse entregue o bolo?")
                    .Confirm("Então você deseja um bolo de {Massa} de {Peso} com recheio {Recheio}, entregue no endereço {EnderecoEntrega}  {?às {HoraEntrega:t}}?")
                    .Message("Ótimo, obrigado {Nome}, vou pedir alguns detalhes de contato se estiver tudo bem, e entraremos em contato para avisarmos quando o bolo estiver finalizado.")
                    .AddRemainingFields()
                    .Message("Obrigado pelo pedido!")
                    .OnCompletion(async (ctx, enquiry) =>
                    {
                        var replyToConversation = ctx.MakeMessage();
                        replyToConversation.Text = "";
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "http://www.blackmarble.co.uk/media/1789/amy.png"));
                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = "https://www.blackmarblecakes.com.br",
                            Type = "openUrl",
                            Title = "Black Marble Cakes"
                        };
                        cardButtons.Add(plButton);
                        HeroCard plCard = new HeroCard()
                        {
                            Title = "Entramemos em contato em breve",
                            Subtitle = "Enquanto espera, dê uma olhada nos produtos de nosso parceiro!",
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        await ctx.PostAsync(replyToConversation);
                    })
                    .Build();
        }
    }
}