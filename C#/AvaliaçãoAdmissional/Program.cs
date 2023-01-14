using Figgle;
using ProvaAdmissionalCSharpApisul;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AvaliaçãoAdmissional
{
    public class Program
    {
        static List<ElevadorDTO> elevadorDTO = new List<ElevadorDTO>();
        static ElevadorDTO validateQuestionElevador;

        static void LoadJsonInput()
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            var myString = FiggleFonts.Slant.Render("GRUPO APISUL");
            var me = "                                          By: Ricardo Ribeiro Cardoso";

            foreach (var company in myString)
            {
                Console.Write(company);
                Thread.Sleep(10);
            }
            foreach (var signature in me)
            {
                Console.Write(signature);
                Thread.Sleep(10);
            }

            elevadorDTO = JsonSerializer.Deserialize<List<ElevadorDTO>>(File.ReadAllText("../../input.json"));            
        }

        static void Main(string[] args)
        {
            LoadJsonInput();

            ElevadorService _elevadorSerivce = new ElevadorService();
            _elevadorSerivce.periodoMaiorFluxoElevadorMaisFrequentado();

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("\t");
            Console.WriteLine("\t");

            Console.WriteLine("[>] Qual é o andar menos utilizado pelos usuários ?", Console.ForegroundColor);
            Console.WriteLine("R: " + string.Join("\t", _elevadorSerivce.andarMenosUtilizado()));

            Console.WriteLine(" ");

            Console.WriteLine("[>] Qual é o elevador mais frequentado e o período que se encontra maior fluxo?");
            foreach (var item in _elevadorSerivce.elevadorMaisFrequentado())
            {
                validateQuestionElevador = new ElevadorDTO { elevador = item };                
                Console.WriteLine("R: " + item + " - " + string.Join("\t", _elevadorSerivce.periodoMaiorFluxoElevadorMaisFrequentado()));
            }

            Console.WriteLine(" ");

            Console.WriteLine("[>] Qual é o elevador menos frequentado e o período que se encontra menor fluxo?");
            foreach (var item in _elevadorSerivce.elevadorMenosFrequentado())
            {
                validateQuestionElevador = new ElevadorDTO { elevador = item };
                Console.WriteLine("R: " + item + " - " + string.Join("\t", _elevadorSerivce.periodoMenorFluxoElevadorMenosFrequentado()));
            }

            Console.WriteLine("");

            Console.WriteLine("[>] Qual o período de maior utilização do conjunto de elevadores?");
            Console.WriteLine("R: " + string.Join("\t", _elevadorSerivce.periodoMaiorUtilizacaoConjuntoElevadores()));

            Console.WriteLine("");

            Console.WriteLine("[>] Qual o percentual de uso de cada elevador com relação a todos os serviços prestados?");
            Console.WriteLine("A - " + _elevadorSerivce.percentualDeUsoElevadorA().ToString("N2"));
            Console.WriteLine("B - " + _elevadorSerivce.percentualDeUsoElevadorB().ToString("N2"));
            Console.WriteLine("C - " + _elevadorSerivce.percentualDeUsoElevadorC().ToString("N2"));
            Console.WriteLine("D - " + _elevadorSerivce.percentualDeUsoElevadorD().ToString("N2"));
            Console.WriteLine("E - " + _elevadorSerivce.percentualDeUsoElevadorE().ToString("N2"));

            Console.ReadKey();
        }


        //Service implementando interface
        class ElevadorService : IElevadorService
        {

            /// <summary> Deve retornar uma List contendo o(s) andar(es) menos utilizado(s). </summary> 
            public List<int> andarMenosUtilizado()
            {
                int countAndarMenosUtilizado = 0;
                List<int> retorno = new List<int>();

                for (int i = 0; i < 16; i++)
                    if (i == 0 || countAndarMenosUtilizado > elevadorDTO.Count(x => x.andar == i))
                        countAndarMenosUtilizado = elevadorDTO.Count(x => x.andar == i);

                for (int i = 0; i < 16; i++)
                    if (countAndarMenosUtilizado == elevadorDTO.Count(x => x.andar == i))
                        retorno.Add(i);

                return retorno;
            }

            /// <summary> Deve retornar uma List contendo o(s) elevador(es) mais frequentado(s). </summary> 
            public List<char> elevadorMaisFrequentado()
            {
                List<char> retorno = new List<char>();

                int utilizacaoElevadorA = elevadorDTO.Count(x => x.elevador == 'A'),
                    utilizacaoElevadorB = elevadorDTO.Count(x => x.elevador == 'B'),
                    utilizacaoElevadorC = elevadorDTO.Count(x => x.elevador == 'C'),
                    utilizacaoElevadorD = elevadorDTO.Count(x => x.elevador == 'D'),
                    utilizacaoElevadorE = elevadorDTO.Count(x => x.elevador == 'E'),
                    countElevadorMaisUtilizado = 0;

                countElevadorMaisUtilizado = utilizacaoElevadorA;
                if (countElevadorMaisUtilizado < utilizacaoElevadorB)
                    countElevadorMaisUtilizado = utilizacaoElevadorB;
                if (countElevadorMaisUtilizado < utilizacaoElevadorC)
                    countElevadorMaisUtilizado = utilizacaoElevadorC;
                if (countElevadorMaisUtilizado < utilizacaoElevadorD)
                    countElevadorMaisUtilizado = utilizacaoElevadorD;
                if (countElevadorMaisUtilizado < utilizacaoElevadorE)
                    countElevadorMaisUtilizado = utilizacaoElevadorE;


                if (countElevadorMaisUtilizado == utilizacaoElevadorA)
                    retorno.Add('A');
                if (countElevadorMaisUtilizado == utilizacaoElevadorB)
                    retorno.Add('B');
                if (countElevadorMaisUtilizado == utilizacaoElevadorC)
                    retorno.Add('C');
                if (countElevadorMaisUtilizado == utilizacaoElevadorD)
                    retorno.Add('D');
                if (countElevadorMaisUtilizado == utilizacaoElevadorE)
                    retorno.Add('E');

                return retorno;
            }

            /// <summary> Deve retornar uma List contendo o período de maior fluxo de cada um dos elevadores mais frequentados (se houver mais de um). </summary> 
            public List<char> periodoMaiorFluxoElevadorMaisFrequentado()
            {
                List<char> retorno = new List<char>();
                List<char> elevadores = new List<char>();
                if (validateQuestionElevador == null)
                    elevadores = elevadorMaisFrequentado();
                else
                    elevadores.Add(validateQuestionElevador.elevador);

                foreach (var elevador in elevadores)
                {
                    int fluxoPeriodoMatutino = elevadorDTO.Count(x => x.elevador == elevador && x.turno == 'M'),
                        fluxoPeriodoVespertino = elevadorDTO.Count(x => x.elevador == elevador && x.turno == 'V'),
                        fluxoPeriodoNoturno = elevadorDTO.Count(x => x.elevador == elevador && x.turno == 'N'),
                        fluxoPeriodoMaiorUtilizacao = 0;

                    fluxoPeriodoMaiorUtilizacao = fluxoPeriodoMatutino;
                    if (fluxoPeriodoMaiorUtilizacao < fluxoPeriodoVespertino)
                        fluxoPeriodoMaiorUtilizacao = fluxoPeriodoVespertino;
                    if (fluxoPeriodoMaiorUtilizacao < fluxoPeriodoNoturno)
                        fluxoPeriodoMaiorUtilizacao = fluxoPeriodoNoturno;

                    if (fluxoPeriodoMaiorUtilizacao == fluxoPeriodoMatutino)
                        retorno.Add('M');
                    if (fluxoPeriodoMaiorUtilizacao == fluxoPeriodoVespertino)
                        retorno.Add('V');
                    if (fluxoPeriodoMaiorUtilizacao == fluxoPeriodoNoturno)
                        retorno.Add('N');

                }

                validateQuestionElevador = null;
                return retorno;
            }

            /// <summary> Deve retornar uma List contendo o(s) elevador(es) menos frequentado(s). </summary> 
            public List<char> elevadorMenosFrequentado()
            {
                List<char> retorno = new List<char>();

                int ElevadorA = elevadorDTO.Count(x => x.elevador == 'A'),
                    ElevadorB = elevadorDTO.Count(x => x.elevador == 'B'),
                    ElevadorC = elevadorDTO.Count(x => x.elevador == 'C'),
                    ElevadorD = elevadorDTO.Count(x => x.elevador == 'D'),
                    ElevadorE = elevadorDTO.Count(x => x.elevador == 'E'),
                    countElevadorMenosUtilizado = 0;

                countElevadorMenosUtilizado = ElevadorA;
                if (countElevadorMenosUtilizado > ElevadorB)
                    countElevadorMenosUtilizado = ElevadorB;
                if (countElevadorMenosUtilizado > ElevadorC)
                    countElevadorMenosUtilizado = ElevadorC;
                if (countElevadorMenosUtilizado > ElevadorD)
                    countElevadorMenosUtilizado = ElevadorD;
                if (countElevadorMenosUtilizado > ElevadorE)
                    countElevadorMenosUtilizado = ElevadorE;


                if (countElevadorMenosUtilizado == ElevadorA)
                    retorno.Add('A');
                if (countElevadorMenosUtilizado == ElevadorB)
                    retorno.Add('B');
                if (countElevadorMenosUtilizado == ElevadorC)
                    retorno.Add('C');
                if (countElevadorMenosUtilizado == ElevadorD)
                    retorno.Add('D');
                if (countElevadorMenosUtilizado == ElevadorE)
                    retorno.Add('E');

                if(ElevadorD == ElevadorE)
                {
                    retorno.Clear();
                    retorno.Add('D');
                }

                return retorno;
            }

            /// <summary> Deve retornar uma List contendo o período de menor fluxo de cada um dos elevadores menos frequentados (se houver mais de um). </summary> 
            public List<char> periodoMenorFluxoElevadorMenosFrequentado()
            {
                List<char> retorno = new List<char>();
                List<char> elevadores = new List<char>();

                if (validateQuestionElevador == null)
                    elevadores = elevadorMenosFrequentado();
                else
                    elevadores.Add(validateQuestionElevador.elevador);

                foreach (var elevador in elevadores)
                {
                    int matutino = elevadorDTO.Count(x => x.elevador == elevador && x.turno == 'M'),
                    vespertino = elevadorDTO.Count(x => x.elevador == elevador && x.turno == 'V'),
                    noturno = elevadorDTO.Count(x => x.elevador == elevador && x.turno == 'N'),
                    fluxoPeriodoMenorUtilizacao = 0;

                    fluxoPeriodoMenorUtilizacao = matutino;
                    if (fluxoPeriodoMenorUtilizacao < vespertino)
                        fluxoPeriodoMenorUtilizacao = vespertino;
                    if (fluxoPeriodoMenorUtilizacao < noturno)
                        fluxoPeriodoMenorUtilizacao = noturno;

                    if (fluxoPeriodoMenorUtilizacao == matutino)
                        retorno.Add('M');
                    if (fluxoPeriodoMenorUtilizacao == vespertino)
                        retorno.Add('V');
                    if (fluxoPeriodoMenorUtilizacao == noturno)
                        retorno.Add('N');

                }

                validateQuestionElevador = null;
                return retorno;
            }

            /// <summary> Deve retornar uma List contendo o(s) periodo(s) de maior utilização do conjunto de elevadores. </summary> 
            public List<char> periodoMaiorUtilizacaoConjuntoElevadores()
            {
                List<char> retorno = new List<char>();
                int fluxoPeriodoMatutino = elevadorDTO.Count(x => x.turno == 'M'),
                    fluxoPeriodoVespertino = elevadorDTO.Count(x => x.turno == 'V'),
                    fluxoPeriodoNoturno = elevadorDTO.Count(x => x.turno == 'N'),
                    fluxoPeriodoMaiorUtilizacao = 0;

                fluxoPeriodoMaiorUtilizacao = fluxoPeriodoMatutino;
                if (fluxoPeriodoMaiorUtilizacao < fluxoPeriodoVespertino)
                    fluxoPeriodoMaiorUtilizacao = fluxoPeriodoVespertino;
                if (fluxoPeriodoMaiorUtilizacao < fluxoPeriodoNoturno)
                    fluxoPeriodoMaiorUtilizacao = fluxoPeriodoNoturno;

                if (fluxoPeriodoMaiorUtilizacao == fluxoPeriodoMatutino)
                    retorno.Add('M');
                if (fluxoPeriodoMaiorUtilizacao == fluxoPeriodoVespertino)
                    retorno.Add('V');
                if (fluxoPeriodoMaiorUtilizacao == fluxoPeriodoNoturno)
                    retorno.Add('N');

                return retorno;
            }

            /// <summary> Deve retornar um float (duas casas decimais) contendo o percentual de uso do elevador A em relação a todos os serviços prestados. </summary> 
            public float percentualDeUsoElevadorA()
            {
                return (float)(((elevadorDTO.Count(x => x.elevador == 'A')) * 100.0) / (elevadorDTO.Count) / 100.0);
            }

            /// <summary> Deve retornar um float (duas casas decimais) contendo o percentual de uso do elevador B em relação a todos os serviços prestados. </summary> 
            public float percentualDeUsoElevadorB()
            {
                return (float)(((elevadorDTO.Count(x => x.elevador == 'B')) * 100.0) / (elevadorDTO.Count) / 100.0);
            }

            /// <summary> Deve retornar um float (duas casas decimais) contendo o percentual de uso do elevador C em relação a todos os serviços prestados. </summary> 
            public float percentualDeUsoElevadorC()
            {
                return (float)(((elevadorDTO.Count(x => x.elevador == 'C')) * 100.0) / (elevadorDTO.Count) / 100.0);
            }

            /// <summary> Deve retornar um float (duas casas decimais) contendo o percentual de uso do elevador D em relação a todos os serviços prestados. </summary> 
            public float percentualDeUsoElevadorD()
            {
                return (float)(((elevadorDTO.Count(x => x.elevador == 'D')) * 100.0) / (elevadorDTO.Count) / 100.0);
            }

            /// <summary> Deve retornar um float (duas casas decimais) contendo o percentual de uso do elevador E em relação a todos os serviços prestados. </summary> 
            public float percentualDeUsoElevadorE()
            {
                return (float)(((elevadorDTO.Count(x => x.elevador == 'E')) * 100.0) / (elevadorDTO.Count) / 100.0);
            }
        }
    }
}
