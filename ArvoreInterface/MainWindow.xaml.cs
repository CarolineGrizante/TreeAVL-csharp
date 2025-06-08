using ArvoreLogica;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArvoreInterface
{
    public partial class MainWindow : Window
    {
        private Arvore arvore = new Arvore();
        private const double NodeRadius = 15; // Raio do círculo do nó
        private const double LevelHeight = 60; // Espaço vertical entre níveis

        public MainWindow()
        {
            InitializeComponent();
            DesenharArvore(); // Desenha a árvore inicial (vazia)
        }

        // --- Manipuladores de Eventos dos Botões (sem alterações) ---

        private void InserirButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ValorTextBox.Text, out int valor))
            {
                arvore.Inserir(valor);
                ValorTextBox.Clear();
                ResultadoTextBox.Text = $"Valor {valor} inserido.";
                DesenharArvore();
            }
            else
            {
                MessageBox.Show("Por favor, insira um valor inteiro válido.", "Erro de Entrada", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoverButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RemoverTextBox.Text, out int valor))
            {
                bool existeAntes = arvore.Buscar(valor);
                arvore.Remover(valor);
                bool existeDepois = arvore.Buscar(valor);
                RemoverTextBox.Clear();
                ResultadoTextBox.Text = existeAntes && !existeDepois ? $"Valor {valor} removido." : $"Valor {valor} não encontrado ou não removido.";
                DesenharArvore();
            }
            else
            {
                MessageBox.Show("Por favor, insira um valor inteiro válido.", "Erro de Entrada", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BuscarButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BuscarTextBox.Text, out int valor))
            {
                bool encontrado = arvore.Buscar(valor);
                ResultadoTextBox.Text = encontrado ? $"Valor {valor} encontrado!" : $"Valor {valor} não encontrado.";
                BuscarTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Por favor, insira um valor inteiro válido.", "Erro de Entrada", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void InOrderButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> lista = arvore.GetInOrderList();
            ResultadoTextBox.Text = "InOrder: " + string.Join(", ", lista);
        }

        private void PreOrderButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> lista = arvore.GetPreOrderList();
            ResultadoTextBox.Text = "PreOrder: " + string.Join(", ", lista);
        }

        private void PosOrderButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> lista = arvore.GetPosOrderList();
            ResultadoTextBox.Text = "PosOrder: " + string.Join(", ", lista);
        }

        private void BalanceamentoButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> info = arvore.GetBalanceamentoInfo();
            ResultadoTextBox.Text = "Fatores de Balanceamento:\n" + string.Join("\n", info);
        }

        private void LimparArvoreButton_Click(object sender, RoutedEventArgs e)
        {
            arvore = new Arvore(); // Cria uma nova árvore vazia
            ResultadoTextBox.Text = "Árvore limpa.";
            DesenharArvore();
        }

        private void CarregarDadosButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "dados.txt"; // Assume que está no diretório de execução
            if (File.Exists(filePath))
            {
                try
                {
                    string[] linhas = File.ReadAllLines(filePath);
                    int count = 0;
                    foreach (var linha in linhas)
                    {
                        if (int.TryParse(linha.Trim(), out int valor))
                        {
                            arvore.Inserir(valor);
                            count++;
                        }
                    }
                    // CORREÇÃO: String interpolation corrigida
                    ResultadoTextBox.Text = $"{count} valores carregados de '{filePath}'.";
                    DesenharArvore();
                }
                catch (Exception ex) // A variável 'ex' é usada aqui
                {
                    // CORREÇÃO: String interpolation corrigida
                    MessageBox.Show($"Erro ao ler o arquivo '{filePath}': {ex.Message}", "Erro de Arquivo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // CORREÇÃO: String interpolation corrigida
                MessageBox.Show($"Arquivo '{filePath}' não encontrado no diretório da aplicação.", "Arquivo Não Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // --- Lógica de Desenho da Árvore ATUALIZADA ---

        private void DesenharArvore()
        {
            ArvoreCanvas.Children.Clear(); // Limpa o canvas
            if (arvore.Raiz == null) return;

            // Busca os Brushes definidos no XAML
            SolidColorBrush lineStroke = FindResource("LineStrokeBrush") as SolidColorBrush ?? Brushes.Gray;
            SolidColorBrush nodeFill = FindResource("NodeFillBrush") as SolidColorBrush ?? Brushes.LightBlue;
            SolidColorBrush nodeStroke = FindResource("NodeStrokeBrush") as SolidColorBrush ?? Brushes.DarkBlue;
            SolidColorBrush textForeground = FindResource("TextBrush") as SolidColorBrush ?? Brushes.Black;

            List<NodeInfo> nodesInfo = arvore.GetVisualInfo(ArvoreCanvas.Width, NodeRadius);

            foreach (var nodeInfo in nodesInfo)
            {
                // Desenha a linha para o pai (se existir)
                if (nodeInfo.ParentX.HasValue && nodeInfo.ParentY.HasValue)
                {
                    Line line = new Line
                    {
                        X1 = nodeInfo.ParentX.Value,
                        Y1 = nodeInfo.ParentY.Value,
                        X2 = nodeInfo.X,
                        Y2 = nodeInfo.Y,
                        Stroke = lineStroke, // Usa o Brush do XAML
                        StrokeThickness = 1.5 // Um pouco mais grossa talvez?
                    };
                    ArvoreCanvas.Children.Add(line);
                }

                // Desenha o nó (círculo)
                Ellipse ellipse = new Ellipse
                {
                    Width = NodeRadius * 2,
                    Height = NodeRadius * 2,
                    Fill = nodeFill,      // Usa o Brush do XAML
                    Stroke = nodeStroke,    // Usa o Brush do XAML
                    StrokeThickness = 1.5
                };
                Canvas.SetLeft(ellipse, nodeInfo.X - NodeRadius);
                Canvas.SetTop(ellipse, nodeInfo.Y - NodeRadius);
                // Adiciona a elipse antes do texto para que o texto fique por cima
                ArvoreCanvas.Children.Add(ellipse);

                // Desenha o valor do nó (texto)
                TextBlock textBlock = new TextBlock
                {
                    Text = nodeInfo.Valor.ToString(),
                    Foreground = textForeground, // Usa o Brush do XAML
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 11, // Um pouco menor para caber melhor
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Width = NodeRadius * 2 // Para centralizar dentro da elipse
                };
                // Ajuste para centralizar melhor o texto verticalmente
                double topOffset = nodeInfo.Y - (textBlock.FontSize / 2) - 1;
                Canvas.SetLeft(textBlock, nodeInfo.X - NodeRadius);
                Canvas.SetTop(textBlock, topOffset);
                ArvoreCanvas.Children.Add(textBlock);
            }
        }
    }
}