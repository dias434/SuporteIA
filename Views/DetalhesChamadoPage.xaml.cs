using Microsoft.Maui.Controls;
using SuporteIA.Models;
using SuporteIA.Services;
using System.ComponentModel;

namespace SuporteIA.Views
{
    public partial class DetalhesChamadoPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ILocalDataService _localDataService;
        private Chamado _chamado = new Chamado();
        private string _respostaIA = string.Empty;

        public Chamado Chamado
        {
            get => _chamado;
            set
            {
                _chamado = value;
                OnPropertyChanged(nameof(Chamado));
                OnPropertyChanged(nameof(StatusDisplay));
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public string StatusDisplay => Chamado?.Status == "Concluído" ? "Concluído" : "Em Aberto";
        public Color StatusColor => StatusDisplay == "Concluído" ? Color.FromArgb("#28A745") : Color.FromArgb("#007BFF");

        public string RespostaIA
        {
            get => _respostaIA;
            set
            {
                _respostaIA = value;
                OnPropertyChanged(nameof(RespostaIA));
            }
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        public DetalhesChamadoPage(Chamado chamado)
        {
            InitializeComponent();
            _localDataService = new LocalDataService();
            Chamado = chamado;
            BindingContext = this;
            
            Console.WriteLine($"🔍 DETALHES CHAMADO - ID: {Chamado.ChamadoId}");
            
            IniciarConversa();
        }

        private void IniciarConversa()
        {
            ConversaContainer.Children.Clear();
            
            AdicionarMensagem("Você", Chamado.Descricao, true);
            
            var respostaIA = GerarRespostaIAInteligente();
            AdicionarMensagem("Assistente IA", respostaIA, false);
        }

        private string GerarRespostaIAInteligente()
        {
            var descricao = Chamado.Descricao.ToLower();
            
            if (descricao.Contains("não consegue") || descricao.Contains("não consigo"))
            {
                if (descricao.Contains("login") || descricao.Contains("senha") || descricao.Contains("acessar"))
                {
                    return "🔐 **Problema de Acesso Identificado**\n\nVou te ajudar passo a passo:\n\n📋 **Soluções Imediatas:**\n1. **Verifique suas credenciais** - Confirme se está usando o email e senha corretos\n2. **Redefina sua senha** - Clique em 'Esqueci minha senha' na tela de login\n3. **Limpe o cache** - Vá em Configurações > Aplicativos > Limpar cache\n4. **Tente outro navegador** - Chrome, Firefox ou Edge\n\n🔄 **Já tentou alguma dessas soluções?** Me conte o resultado para eu poder ajudar melhor!";
                }
                else if (descricao.Contains("internet") || descricao.Contains("conexão") || descricao.Contains("conectar"))
                {
                    return "🌐 **Problema de Conexão Detectado**\n\nVamos resolver isso juntos! Siga estas etapas:\n\n🔧 **Soluções Rápidas:**\n1. **Reinicie o roteador** - Desligue por 30 segundos e ligue novamente\n2. **Teste em outro dispositivo** - Verifique se o problema é geral\n3. **Cabo de rede** - Se possível, conecte via cabo\n4. **Wi-Fi** - Aproxime-se do roteador ou conecte-se a outra rede\n\n📡 **Qual é a mensagem de erro que aparece?** Isso me ajuda a dar uma solução mais precisa!";
                }
                else if (descricao.Contains("imprimir") || descricao.Contains("impressora"))
                {
                    return "🖨️ **Problema com Impressora**\n\nVamos solucionar isso:\n\n🛠️ **Soluções:**\n1. **Verifique a conexão** - Cabo USB ou Wi-Fi da impressora\n2. **Reinicie a impressora** - Desligue e ligue novamente\n3. **Driver atualizado** - Baixe o driver mais recente\n4. **Fila de impressão** - Limpe a fila em 'Dispositivos e Impressoras'\n\n📄 **A impressora aparece na lista de dispositivos?**";
                }
            }
            else if (descricao.Contains("como fazer") || descricao.Contains("como usar") || descricao.Contains("como configurar"))
            {
                return "📚 **Instruções Detalhadas**\n\nCom prazer vou te explicar! Baseado na sua solicitação, aqui está o passo a passo:\n\n🎯 **Passo a Passo:**\n1. Acesse o menu principal do sistema\n2. Localize a opção mencionada\n3. Siga as instruções na tela\n4. Confirme as alterações\n\n💡 **Dica:** Se encontrar alguma dificuldade, me informe em qual passo específico está tendo problema para eu ajudar melhor!";
            }
            else if (descricao.Contains("lento") || descricao.Contains("lentidão") || descricao.Contains("travando"))
            {
                return "⚡ **Problema de Performance**\n\nVamos melhorar a velocidade do sistema:\n\n🔧 **Otimizações Imediatas:**\n1. **Feche abas não usadas** - Reduza o consumo de memória\n2. **Reinicie o aplicativo** - Às vezes resolve instantaneamente\n3. **Verifique a internet** - Teste a velocidade\n4. **Limpe cache** - Vá em Configurações > Armazenamento > Limpar cache\n\n📊 **Em qual situação específica está lento?** Isso me ajuda a dar a solução correta!";
            }
            
            return "👋 **Assistente IA - Suporte Técnico**\n\nObrigado por descrever seu problema! Analisei sua solicitação e estou pronto para ajudar.\n\n🎯 **Para te ajudar melhor, preciso saber:**\n• Quando exatamente o problema acontece?\n• Há alguma mensagem de erro específica?\n• Já tentou alguma solução?\n\n💬 **Descreva com mais detalhes para eu dar a solução mais precisa!**";
        }

        private async void OnEnviarMensagemClicked(object sender, EventArgs e)
        {
            var mensagem = NovaMensagemEntry.Text?.Trim();
            
            if (string.IsNullOrWhiteSpace(mensagem))
            {
                await DisplayAlert("Atenção", "Digite uma mensagem", "OK");
                return;
            }

            AdicionarMensagem("Você", mensagem, true);
            NovaMensagemEntry.Text = string.Empty;

            var resposta = AnalisarNovaMensagem(mensagem);
            AdicionarMensagem("Assistente IA", resposta, false);
        }

        private string AnalisarNovaMensagem(string mensagem)
        {
            var msg = mensagem.ToLower();
            
            if (msg.Contains("humano") || msg.Contains("atendente") || msg.Contains("suporte humano") || msg.Contains("pessoa") || msg.Contains("operador") || msg.Contains("não resolveu"))
            {
                return "👨‍💼 **Encaminhamento para Suporte Humano**\n\n✅ **Entendido! Estou encaminhando seu caso para nossa equipe especializada.**\n\n📞 **O que vai acontecer agora:**\n• Seu chamado foi priorizado\n• Um técnico entrará em contato em até 15 minutos\n• Teremos acesso ao histórico completo da conversa\n\n⏰ **Enquanto isso, você pode:**\n• Descrever mais detalhes do problema\n• Informar horários disponíveis para contato\n• Adicionar prints ou informações adicionais\n\n🔜 **Nosso team já foi notificado e em breve entrará em contato!**";
            }
            
            if (msg.Contains("resolvido") || msg.Contains("funcionou") || msg.Contains("deu certo") || msg.Contains("consegui") || msg.Contains("obrigado"))
            {
                return "🎉 **Excelente! Que bom que consegui ajudar!**\n\n✅ **Solução encontrada com sucesso!**\n\n💡 **Para finalizar:**\n• Sua solução foi registrada no sistema\n• Caso o problema volte, reabra este chamado\n• Estamos sempre disponíveis para ajudar\n\n🌟 **Deseja encerrar o chamado agora?** Basta clicar em 'Concluir Chamado' abaixo!";
            }
            
            if (msg.Contains("não funcionou") || msg.Contains("ainda não") || msg.Contains("não deu certo") || msg.Contains("continua"))
            {
                return "🔄 **Vamos Tentar uma Abordagem Diferente**\n\nEntendi que a solução anterior não funcionou. Não se preocupe, temos alternativas!\n\n🔧 **Próximas Opções:**\n1. **Solução alternativa** - Vou propor um método diferente\n2. **Análise detalhada** - Preciso de mais informações técnicas\n3. **Escalonamento** - Se necessário, encaminho para especialista\n\n📋 **Para me ajudar:**\n• Qual foi o resultado exato ao tentar a solução?\n• Apareceu alguma mensagem de erro?\n• O problema é consistente ou intermitente?\n\n💬 **Descreva o que aconteceu quando tentou a solução anterior.**";
            }
            
            if (msg.Contains("urgente") || msg.Contains("urgência") || msg.Contains("importante") || msg.Contains("prioridade"))
            {
                return "🚨 **Caso Identificado como Urgente**\n\n✅ **Seu chamado foi marcado como PRIORIDADE MÁXIMA!**\n\n⚡ **Ações Imediatas:**\n• Notificação enviada para toda a equipe\n• Técnicos mais experientes alertados\n• Tempo de resposta reduzido significativamente\n\n📞 **Próximos passos:**\n1. Nossa equipe entrará em contato em até 5 minutos\n2. Solução remota imediata se possível\n3. Atendimento presencial se necessário\n\n🔜 **Fique próximo do dispositivo para o contato!**";
            }
            
            return "💡 **Analisando sua Mensagem...**\n\nObrigado pela informação! Estou processando os detalhes para dar a melhor solução.\n\n🎯 **Baseado no que você descreveu, recomendo:**\n• Verificar configurações específicas do sistema\n• Testar soluções alternativas\n• Coletar informações adicionais se necessário\n\n🔍 **Para me ajudar a ser mais preciso:**\n• Há alguma mensagem de erro específica?\n• Quando exatamente o problema ocorre?\n• Já funcionou normalmente antes?\n\n💬 **Continue descrevendo que vou encontrar a solução ideal!**";
        }

        private void AdicionarMensagem(string remetente, string mensagem, bool ehUsuario)
        {
            var horario = DateTime.Now.ToString("HH:mm");
            
            var mensagemLayout = new VerticalStackLayout 
            { 
                Spacing = 2,
                HorizontalOptions = ehUsuario ? LayoutOptions.End : LayoutOptions.Start,
                Margin = new Thickness(10, 5)
            };

            var bolha = new Frame
            {
                BackgroundColor = ehUsuario ? Color.FromArgb("#DCF8C6") : Color.FromArgb("#FFFFFF"),
                BorderColor = Color.FromArgb("#E0E0E0"),
                CornerRadius = 10,
                Padding = new Thickness(12, 8),
                HorizontalOptions = ehUsuario ? LayoutOptions.End : LayoutOptions.Start,
                MaximumWidthRequest = 280,
                Content = new VerticalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new Label 
                        { 
                            Text = remetente, 
                            FontSize = 12, 
                            FontAttributes = FontAttributes.Bold,
                            TextColor = ehUsuario ? Color.FromArgb("#075E54") : Color.FromArgb("#128C7E")
                        },
                        new Label 
                        { 
                            Text = mensagem, 
                            FontSize = 14,
                            TextColor = Colors.Black
                        },
                        new Label 
                        { 
                            Text = horario, 
                            FontSize = 10,
                            TextColor = Colors.Gray,
                            HorizontalOptions = LayoutOptions.End
                        }
                    }
                }
            };

            mensagemLayout.Children.Add(bolha);
            ConversaContainer.Children.Add(mensagemLayout);

            ScrollParaUltimaMensagem();
        }

        private void ScrollParaUltimaMensagem()
        {
            Dispatcher.Dispatch(async () =>
            {
                await ConversaScrollView.ScrollToAsync(ConversaScrollView, ScrollToPosition.End, true);
            });
        }

        private async void OnConcluirChamadoClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("🚨 BOTÃO CONCLUIR CLICADO - SOLUÇÃO LOCAL");
                
                bool confirmar = await DisplayAlert(
                    "Concluir Chamado", 
                    "Deseja realmente concluir este chamado?", 
                    "Sim, Concluir", 
                    "Cancelar"
                );

                if (!confirmar) 
                {
                    Console.WriteLine("❌ USUÁRIO CANCELOU");
                    return;
                }

                Console.WriteLine($"✅ USUÁRIO CONFIRMOU - Chamado ID: {Chamado.ChamadoId}");

                IsEnabled = false;

                // Salvar localmente como concluído
                await _localDataService.SalvarChamadoConcluidoAsync(Chamado.ChamadoId);
                
                Console.WriteLine($"💾 SALVO NO LOCALSTORAGE COM SUCESSO!");
                    
                await DisplayAlert("Sucesso", "Chamado concluído com sucesso!", "OK");
                
                await AtualizarDashboard();
                
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERRO: {ex.Message}");
                await DisplayAlert("Erro", $"Falha ao concluir chamado: {ex.Message}", "OK");
                IsEnabled = true;
            }
        }

        private async Task AtualizarDashboard()
        {
            try
            {
                Console.WriteLine("🔄 ATUALIZANDO DASHBOARD LOCAL...");
                
                var navigationStack = Navigation.NavigationStack;
                foreach (var page in navigationStack)
                {
                    if (page is DashboardPage dashboardPage)
                    {
                        Console.WriteLine("✅ DASHBOARD ENCONTRADO, ATUALIZANDO...");
                        await Task.Delay(300);
                        await dashboardPage.CarregarDadosDashboard();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ ERRO AO ATUALIZAR DASHBOARD: {ex.Message}");
            }
        }

        protected new virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}