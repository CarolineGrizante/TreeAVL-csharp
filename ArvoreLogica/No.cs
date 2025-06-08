using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArvoreLogica
{
    public class No
    {
        public int Valor;
        public int Altura;
        public No? Esquerda;
        public No? Direita;

        public No(int valor)
        {
            Valor = valor;
            Altura = 1;
            Esquerda = null;
            Direita = null;
        }
    }
}