using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArvoreLogica;

namespace ArvoreLogica 
{
    // Classe auxiliar para retornar informações de visualização
    public class NodeInfo
    {
        public int Valor { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double? ParentX { get; set; }
        public double? ParentY { get; set; }

        public NodeInfo(int valor, double x, double y, double? parentX = null, double? parentY = null)
        {
            Valor = valor;
            X = x;
            Y = y;
            ParentX = parentX;
            ParentY = parentY;
        }
    }

    public class Arvore
    {
        public No? Raiz { get; private set; } // Tornar Raiz acessível para visualização

        public void Inserir(int valor)
        {
            Raiz = Inserir(Raiz, valor);
        }

        private No Inserir(No? no, int valor)
        {
            if (no == null)
                return new No(valor);

            if (valor < no.Valor)
                no.Esquerda = Inserir(no.Esquerda, valor);
            else if (valor > no.Valor)
                no.Direita = Inserir(no.Direita, valor);
            else // Valor já existe, não faz nada ou lança exceção/retorna bool
                return no;

            AtualizarAltura(no);
            return Balancear(no);
        }

        public void Remover(int valor)
        {
            Raiz = Remover(Raiz, valor);
        }

        private No? Remover(No? no, int valor)
        {
            if (no == null)
                return null;

            if (valor < no.Valor)
                no.Esquerda = Remover(no.Esquerda, valor);
            else if (valor > no.Valor)
                no.Direita = Remover(no.Direita, valor);
            else
            {
                if (no.Esquerda == null || no.Direita == null)
                {
                    No? temp = no.Esquerda ?? no.Direita;
                    no = null; // Libera o nó
                    return temp;
                }
                else
                {
                    No sucessor = EncontrarMenor(no.Direita!);
                    no.Valor = sucessor.Valor;
                    no.Direita = Remover(no.Direita, sucessor.Valor);
                }
            }

            // Se o nó foi removido (caso de folha ou com um filho), no será null aqui.
            // Precisamos verificar isso antes de atualizar altura e balancear.
            if (no == null) return null;

            AtualizarAltura(no);
            return Balancear(no);
        }

        private No EncontrarMenor(No no) // Não pode ser nulo aqui
        {
            while (no.Esquerda != null)
                no = no.Esquerda;
            return no;
        }

        public bool Buscar(int valor)
        {
            return Buscar(Raiz, valor) != null;
        }

        private No? Buscar(No? no, int valor)
        {
            while (no != null)
            {
                if (valor == no.Valor)
                    return no;
                else if (valor < no.Valor)
                    no = no.Esquerda;
                else
                    no = no.Direita;
            }
            return null;
        }

        // Retorna lista de valores em InOrder
        public List<int> GetInOrderList()
        {
            var list = new List<int>();
            GetInOrderList(Raiz, list);
            return list;
        }

        private void GetInOrderList(No? no, List<int> list)
        {
            if (no != null)
            {
                GetInOrderList(no.Esquerda, list);
                list.Add(no.Valor);
                GetInOrderList(no.Direita, list);
            }
        }

        // Retorna lista de valores em PreOrder
        public List<int> GetPreOrderList()
        {
            var list = new List<int>();
            GetPreOrderList(Raiz, list);
            return list;
        }

        private void GetPreOrderList(No? no, List<int> list)
        {
            if (no != null)
            {
                list.Add(no.Valor);
                GetPreOrderList(no.Esquerda, list);
                GetPreOrderList(no.Direita, list);
            }
        }

        // Retorna lista de valores em PosOrder
        public List<int> GetPosOrderList()
        {
            var list = new List<int>();
            GetPosOrderList(Raiz, list);
            return list;
        }

        private void GetPosOrderList(No? no, List<int> list)
        {
            if (no != null)
            {
                GetPosOrderList(no.Esquerda, list);
                GetPosOrderList(no.Direita, list);
                list.Add(no.Valor);
            }
        }

        // Retorna lista de strings com informações de balanceamento
        public List<string> GetBalanceamentoInfo()
        {
            var info = new List<string>();
            GetBalanceamentoInfo(Raiz, info);
            return info;
        }

        private void GetBalanceamentoInfo(No? no, List<string> info)
        {
            if (no != null)
            {
                GetBalanceamentoInfo(no.Esquerda, info);
                int fb = FatorBalanceamento(no);
                info.Add($"Nó {no.Valor} - FB = {fb}");
                GetBalanceamentoInfo(no.Direita, info);
            }
        }

        // Retorna lista de NodeInfo para visualização
        public List<NodeInfo> GetVisualInfo(double canvasWidth, double nodeRadius)
        {
            var infoList = new List<NodeInfo>();
            if (Raiz != null)
            {
                // Calcula a posição inicial da raiz (centro horizontal, topo)
                double initialX = canvasWidth / 2;
                double initialY = nodeRadius * 2; // Espaço no topo
                double horizontalGap = canvasWidth / 4; // Ajuste inicial, pode precisar refinar
                double verticalGap = nodeRadius * 4; // Espaço vertical entre níveis

                GetVisualInfoRecursive(Raiz, initialX, initialY, horizontalGap, verticalGap, infoList, null, null);
            }
            return infoList;
        }

        private void GetVisualInfoRecursive(No no, double x, double y, double hGap, double vGap, List<NodeInfo> infoList, double? parentX, double? parentY)
        {
            infoList.Add(new NodeInfo(no.Valor, x, y, parentX, parentY));

            double childY = y + vGap;
            double childHGap = hGap / 2; // Reduz o espaçamento horizontal para os filhos

            if (no.Esquerda != null)
            {
                GetVisualInfoRecursive(no.Esquerda, x - hGap, childY, childHGap, vGap, infoList, x, y);
            }
            if (no.Direita != null)
            {
                GetVisualInfoRecursive(no.Direita, x + hGap, childY, childHGap, vGap, infoList, x, y);
            }
        }


        // --- Métodos privados de balanceamento (sem alterações) ---
        private int Altura(No? no) => no?.Altura ?? 0;

        private void AtualizarAltura(No no)
        {
            no.Altura = 1 + Math.Max(Altura(no.Esquerda), Altura(no.Direita));
        }

        private int FatorBalanceamento(No no)
        {
            return Altura(no.Esquerda) - Altura(no.Direita);
        }

        private No Balancear(No no)
        {
            int fb = FatorBalanceamento(no);

            // Rotação Direita (Caso Esquerda-Esquerda)
            if (fb > 1 && FatorBalanceamento(no.Esquerda!) >= 0)
            {
                return RotacionarDireita(no);
            }

            // Rotação Esquerda (Caso Direita-Direita)
            if (fb < -1 && FatorBalanceamento(no.Direita!) <= 0)
            {
                return RotacionarEsquerda(no);
            }

            // Rotação Esquerda-Direita (Caso Esquerda-Direita)
            if (fb > 1 && FatorBalanceamento(no.Esquerda!) < 0)
            {
                no.Esquerda = RotacionarEsquerda(no.Esquerda!);
                return RotacionarDireita(no);
            }

            // Rotação Direita-Esquerda (Caso Direita-Esquerda)
            if (fb < -1 && FatorBalanceamento(no.Direita!) > 0)
            {
                no.Direita = RotacionarDireita(no.Direita!);
                return RotacionarEsquerda(no);
            }

            return no; // Não precisa balancear
        }

        private No RotacionarDireita(No y)
        {
            No x = y.Esquerda!;
            No? T2 = x.Direita; // T2 pode ser null

            // Realiza a rotação
            x.Direita = y;
            y.Esquerda = T2;

            // Atualiza alturas
            AtualizarAltura(y);
            AtualizarAltura(x);

            // Retorna a nova raiz da subárvore
            return x;
        }

        private No RotacionarEsquerda(No x)
        {
            No y = x.Direita!;
            No? T2 = y.Esquerda; // T2 pode ser null

            // Realiza a rotação
            y.Esquerda = x;
            x.Direita = T2;

            // Atualiza alturas
            AtualizarAltura(x);
            AtualizarAltura(y);

            // Retorna a nova raiz da subárvore
            return y;
        }
    }
}