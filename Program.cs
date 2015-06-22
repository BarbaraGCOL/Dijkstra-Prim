using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace TP1_Dijkstra_prim
{
    class DijkstraPrim
    {
        public static int MAX_VERTICES = 2000;
        enum Status {naoVisto = 0, naArvore = 1, naBorda = 2};
        
        /// <summary>
        /// Cria um NodoArco, a ser armazenado na lista de adjacência, composto por um vértice,
        /// o peso do arco e o prox, que aponta para o próximo nodo
        /// </summary>
        class NodoArco
        {
            public Object vertice;
            public Object peso;
            public NodoArco prox;

            /// <summary>
            /// Contrutora. Inicializa o vértice, o peso e o prox com valor null
            /// </summary>
            public NodoArco()
            {
                vertice = null;
                peso = null;
                prox = null;
            }

            /// <summary>
            /// Construtora. Inicializa o vértice e o peso com os valores recebidos como parâmetro e o prox com valor null
            /// </summary>
            /// <param name="ValorItem">Valor do vértice</param>
            /// <param name="ValorItem2">Valor do peso</param>
            public NodoArco(object ValorItem, object ValorItem2)
            {
                peso = ValorItem2;
                vertice = ValorItem;
                prox = null;
            }

            /// <summary>
            /// Construtora. Inicializa o vértice, o peso e o prox com os valores recebidos como parâmetro
            /// </summary>
            /// <param name="valorItem"></param>
            /// <param name="ValorItem2"></param>
            /// <param name="proxCelula"></param>
            public NodoArco(object valorItem, object ValorItem2, NodoArco proxCelula)
            {
                peso = ValorItem2;
                vertice = valorItem;
                prox = proxCelula;
            }
        }

        /// <summary>
        /// Cria uma lista de adjacência, uma lista encadeada composta por NodoArco
        /// </summary>
        class ListaDeAdjacencia
        {
            private NodoArco primeira, pos, ultima;
            private int Qtde = 0;

            /// <summary>
            /// Função construtora. Aloca a célula cabeça e faz todas as referências
            /// apontarem para ela.
            /// </summary>
            public ListaDeAdjacencia()
            {
                primeira = new NodoArco();
                ultima = primeira;
            }

            /// <summary>
            /// Verifica se a lista está vazia
            /// </summary>
            /// <returns>Retorna TRUE se a lista estiver vazia e FALSE caso contrário.</returns>
            public bool Vazia()
            {
                return primeira == ultima;
            }

            /// <summary>
            /// Insere um novo item no fim da lista.
            /// </summary>
            /// <param name="valorItem">O Object contendo o item a ser inserido.</param>
            public void InsereFim(Object valorItem, Object ValorItem2)
            // insere um item no fim da lista
            {
                ultima.prox = new NodoArco(valorItem, ValorItem2);
                ultima = ultima.prox;
                Qtde++;
            }

            /// <summary>
            /// Propriedade que retorna a quantidade de itens da lista.
            /// </summary>
            public int Count
            {
                get
                {
                    return Qtde;
                }
            }

            /// <summary>
            ///  Move o cursor para a próxima posição.
            /// </summary>
            public void Proximo()
            {
                if (pos != ultima)
                    pos = pos.prox;
            }

            /// <summary>
            /// Move o cursor pos para a primeira posição.
            /// </summary>
            public void Primeiro()
            {
                if (primeira != ultima)
                    pos = primeira.prox;
            }

            /// <summary>
            /// Retorna o item localizado na posição atual do cursor pos.
            /// </summary>
            /// <returns>Um object contendo o elemento da atual posição do cursor "pos" da lista. Se a lista estiver vazia ou pos estiver posicionado em uma posição inválida a função retorna null.</returns>
            public NodoArco RetornaPosAtual()
            {
                if (pos != null)
                    return pos;
                else
                    return null;
            }
        }
        
        class Grafo 
        {
            private int[]proxDaBorda=new int[MAX_VERTICES];
            private int[]pesoDaBorda=new int[MAX_VERTICES];
            private int[]pai=new int[MAX_VERTICES];
            private Status[] status = new Status[MAX_VERTICES];
            private ListaDeAdjacencia[]listadeAdjacencia=new ListaDeAdjacencia [MAX_VERTICES];
            private int qtVert=0;
            private bool grafoEncontrado = false;

            // lê os dados de um grafo da entrada e insere nas listas de adjacência
            void leGrafo(ListaDeAdjacencia[]lista,String end)
            {
                String[] pesos;
                int cont = 0;
                FileInfo fileInfo = new FileInfo(end);
                ListaDeAdjacencia nodos;

                if (fileInfo.Exists)
                {
                    FileStream file = new FileStream(end, FileMode.Open);
                    StreamReader str = new StreamReader(file);
                    String texto;
                    while (!str.EndOfStream)
                    {
                        texto = str.ReadLine();
                        pesos = texto.Split(' ');//Separa os pesos através dos espaços em branco
                        nodos = new ListaDeAdjacencia();
                        double peso;
                        for (int i = 0; i < pesos.Length; i++)
                        {
                            peso = double.Parse(pesos[i]);
                            if (peso != -1 && i != cont)//Não adiciona quando o valor for igual ao do vértice analisado pois não permite loop
                            {
                                nodos.InsereFim(i, pesos[i]);//Insere o nodo (vértice e o peso) na lista
                            }
                        }
                        lista[cont] = nodos;//Insere a lista no vetor de listas de adjacência
                        cont++;
                    }
                    str.Close();
                    qtVert = cont;//Incrementa a quantidade de vértices total do grafo
                    grafoEncontrado = true;
                }
                else
                {
                    Console.WriteLine("O arquivo não foi encontrado!");
                }
            }
        
            /// <summary>
            /// Contrutora. Lê o grafo a partir do arquivo .txt recebido como parâmetro e o armazena nas listas de adjacência
            /// </summary>
            /// <param name="endereco">Endereço do arquivo .txt que contém o grafo</param>
            public Grafo(String endereco) 
            {
                leGrafo(listadeAdjacencia,endereco);
            }

            /// <summary>
            /// Imprime a árvore geradora mínima do grafo
            /// </summary>
            public void ImprimeAGM()
            {
                Console.WriteLine("**** ÁRVORE GERADORA MÍNIMA ****");
                for (int i = 1; i < qtVert; i++)
                {
                    Console.WriteLine("Pai->  "+pai[i]+"    Peso->  "+pesoDaBorda[i]+"    Vértice->  "+i);
                }
            }

            /// <summary>
            /// encontra a árvore geradora mínima do grafo
            /// </summary>
            /// <param name="grafo">grafo no qual será encontrada a AGM</param>
            public void arvoreGeradoraMinima(Grafo grafo)
            {
                int quantosArcos=0,candidatos=0;
                bool travou;

                //Verifica se o grafo recebido como parâmetro é válido
                if (grafoEncontrado)
                {
                    // Inicialização
                    //x = um vértice arbitrário;
                    int x = 0;

                    //Insere x na árvore
                    status[x] = Status.naArvore;

                    //Faz o conjunto de arcos na árvore vazio.
                    for (int i = 0; i < qtVert; i++)
                    {
                        if (i != x)
                        {
                            status[i] = Status.naoVisto;
                        }
                    }

                    travou = false;

                    //Passo 2. 
                    // Loop principal; x acabou de ser incorporado à arvore.
                    //// Atualiza a borda e os candidatos. Depois adiciona um arco e um vértice.
                    while ((quantosArcos + 1) != qtVert && !travou)
                    {
                        ListaDeAdjacencia nodos = (listadeAdjacencia[x]);
                        NodoArco nodo;
                        int peso = 0, vert, borda = 0, menorPeso = 0;

                        nodos.Primeiro();

                        //Passo 3. 
                        // troca alguns arcos candidatos para manter apenas os melhores por vértice
                        for (int i = 0; i < nodos.Count; i++)
                        {
                            nodo = nodos.RetornaPosAtual();
                            nodos.Proximo();
                            peso = Convert.ToInt32(nodo.peso);
                            vert = Convert.ToInt32(nodo.vertice);

                            //verifica se o vértice y, adjacente a x, está na borda
                            if (status[vert] == Status.naBorda)
                            {
                                //Verifica se o peso do arco nesse vértice é menor que o peso da borda armazenado nesse vértice
                                if (peso < pesoDaBorda[vert])
                                {
                                    //o arco será o novo arco candidato para y
                                    pesoDaBorda[vert] = peso;
                                    pai[vert] = x;
                                }
                            }
                            else
                                //Passo 4. // Determina novos vértices da borda e arcos candidatos
                                //Verifica se y, adjacente a x, não foi visto
                                if (status[vert] == Status.naoVisto)
                                {
                                    pesoDaBorda[vert] = peso;//xy agora é um arco candidato;
                                    pai[vert] = x;
                                    status[vert] = Status.naBorda;//y agora é um vértice da borda;
                                }
                        }

                        //Atualiza os proxs da borda
                        for (int a = 0; a < qtVert; a++)//Percorre todos os vértices do grafo
                        {
                            int cont = 0;
                            bool achou1 = false, achou2 = false;
                            menorPeso = 0;
                            borda = 0;

                            //Verifica se o vértice ainda possui vértice na borda
                            if (proxDaBorda[a] != -1)
                            {
                                for (int b = 0; b < qtVert; b++)
                                {
                                    //Verifica se é um vértice diferente do atual e se está na borda
                                    if (pai[b] == a && status[b] == Status.naBorda)
                                    {
                                        achou1 = true;
                                        //Verifica o arco adjacente a a com menor peso
                                        if (cont == 0 || pesoDaBorda[b] < menorPeso)
                                        {
                                            menorPeso = pesoDaBorda[b];
                                            borda = b;
                                            achou2 = true;
                                        }
                                        cont++;
                                    }
                                }
                                //Se encontrou um arco com menor peso, atualiza o proxDaBorda
                                if (achou2)
                                {
                                    proxDaBorda[a] = borda;
                                }
                                else
                                    //Se o vértice não tem mais arcos candidatos se proxDaBorda terá valor -1
                                    if (achou1)
                                    {
                                        proxDaBorda[a] = -1;
                                    }
                            }
                        }
                        //Passo 5. 
                        // Testa para escolher o próximo arco
                        for (int i = 0; i < qtVert; i++)
                        {
                            //Conta quantos vértices há na borda
                            if (status[i] == Status.naBorda)
                            {
                                candidatos++;
                            }
                        }

                        //Verifica se há arcos candidatos
                        if (candidatos == 0)
                        {
                            // se não existirem mais arcos candidatos então não há árvore geradora
                            travou = true;
                        }
                        //Passo 6. 
                        // Escolhe o próximo arco
                        else
                        {
                            //Encontra um arco candidato com peso mínimo;
                            int cont = 0;
                            menorPeso = 0;
                            for (int i = 0; i < qtVert; i++)
                            {
                                if (proxDaBorda[i] != -1 && status[proxDaBorda[i]] == Status.naBorda)
                                {
                                    peso = pesoDaBorda[proxDaBorda[i]];
                                    if (cont == 0 || (peso < menorPeso))
                                    {
                                        menorPeso = peso;
                                        borda = proxDaBorda[i];
                                    }
                                    cont++;
                                }
                            }
                            //x = vértice da borda incidente ao arco;
                            x = borda;

                            //Adiciona x e o arco à árvore; // x não é mais da borda e o arco não é mais candidato
                            status[borda] = Status.naArvore;
                        }
                        candidatos = 0;
                        //Se não travou incrementa o número de arcos da AGM
                        if (!travou)
                            quantosArcos++;
                    } // while    
                    if (travou)
                    {
                        Console.WriteLine("Grafo desconexo! Não existe árvore geradora!");
                    }
                    else
                    {
                        ImprimeAGM();
                    }
                }
            }
        }
        class Program
        {
            static void Main(string[] args)
            {
                char escolha='N';
                do
                {
                    Console.Clear();
                    Console.WriteLine("Digite o endereço do arquivo que contém o grafo e tecle ENTER: ");
                    String endereco = Console.ReadLine();
                    Grafo g = new Grafo(endereco);
                    g.arvoreGeradoraMinima(g);
                    Console.WriteLine("Digite S para encontrar a AGM de outro grafo ou outra \ntecla para encerrar o programa e tecle ENTER: ");
                    escolha = char.Parse(Console.ReadLine());
                } while (escolha == 'S');
                Console.ReadKey(false);
            }
        }
    }
}
