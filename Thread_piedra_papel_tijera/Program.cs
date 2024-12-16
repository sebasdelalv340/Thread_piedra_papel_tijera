// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Threading;

namespace PiedraPapelTijera
{
    class Program
    {
        static readonly List<string> Movimientos = new List<string> { "piedra", "papel", "tijera" };
        static int _victoriasHiloMain = 0;
        static int _victoriasHiloSecundario = 0;
        static string _movimientoHiloSecundario = string.Empty;
        static readonly object LockObj = new object();
        static bool _listoParaJugar = false;

        static void Main()
        {
            Thread hiloSecundario = new Thread(JugarSecundario);
            hiloSecundario.Start();

            Random random = new Random();

            while (_victoriasHiloMain < 3 && _victoriasHiloSecundario < 3)
            {
                // Generar movimiento del hilo principal
                string movimientoHiloMain = Movimientos[random.Next(Movimientos.Count)];

                // Preparar el hilo secundario para jugar
                lock (LockObj)
                {
                    _movimientoHiloSecundario = string.Empty;
                    _listoParaJugar = true;
                    Monitor.Pulse(LockObj); // Despertar el hilo secundario
                }

                // Esperar a que el hilo secundario haga su jugada
                lock (LockObj)
                {
                    while (_listoParaJugar)
                    {
                        Monitor.Wait(LockObj);
                    }

                    // Determinar el ganador de la ronda
                    Console.WriteLine($"Main juega {movimientoHiloMain} contra Secundario que juega {_movimientoHiloSecundario}");
                    int resultado = DeterminarGanador(movimientoHiloMain, _movimientoHiloSecundario);
                    if (resultado == 1)
                    {
                        _victoriasHiloMain++;
                        Console.WriteLine($"Main gana esta ronda. Marcador: Main {_victoriasHiloMain} - Secundario {_victoriasHiloSecundario}");
                    }
                    else if (resultado == -1)
                    {
                        _victoriasHiloSecundario++;
                        Console.WriteLine($"Secundario gana esta ronda. Marcador: Main {_victoriasHiloMain} - Secundario {_victoriasHiloSecundario}");
                    }
                    else
                    {
                        Console.WriteLine("Empate en esta ronda.");
                    }
                }

                Thread.Sleep(500); // Pausa para hacer más legible la salida
            }

            Console.WriteLine($"El juego ha terminado. ¡{(_victoriasHiloMain > _victoriasHiloSecundario ? "Main" : "Secundario")} ganó!");

            // Finalizar el hilo secundario
            hiloSecundario.Join();
        }

        static void JugarSecundario()
        {
            Random random = new Random();

            while (_victoriasHiloMain < 3 && _victoriasHiloSecundario < 3)
            {
                lock (LockObj)
                {
                    // Esperar hasta que el hilo principal esté listo
                    while (!_listoParaJugar)
                    {
                        Monitor.Wait(LockObj);
                    }

                    // Hacer la jugada del hilo secundario
                    _movimientoHiloSecundario = Movimientos[random.Next(Movimientos.Count)];
                    _listoParaJugar = false;
                    Monitor.Pulse(LockObj); // Notificar al hilo principal
                }
            }
        }

        static int DeterminarGanador(string movimiento1, string movimiento2)
        {
            if (movimiento1 == movimiento2)
                return 0; // Empate
            if ((movimiento1 == "piedra" && movimiento2 == "tijera") ||
                (movimiento1 == "tijera" && movimiento2 == "papel") ||
                (movimiento1 == "papel" && movimiento2 == "piedra"))
                return 1; // Gana el movimiento 1
            return -1; // Gana el movimiento 2
        }
    }
}


