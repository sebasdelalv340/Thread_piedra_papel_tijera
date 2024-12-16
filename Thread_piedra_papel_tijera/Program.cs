// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Threading;

namespace PiedraPapelTijera
{
    class Program
    {
        static readonly List<string> movimientos = new List<string> { "piedra", "papel", "tijera" };
        static int victoriasHilo1 = 0;
        static int victoriasHilo2 = 0;
        static readonly object lockObj = new object();

        static void Main()
        {
            Thread hilo1 = new Thread(Jugar);
            Thread hilo2 = new Thread(Jugar);

            hilo1.Start("Hilo 1");
            hilo2.Start("Hilo 2");

            hilo1.Join();
            hilo2.Join();

            Console.WriteLine($"El juego ha terminado. ¡{(victoriasHilo1 > victoriasHilo2 ? "Hilo 1" : "Hilo 2")} ganó!");
        }

        static void Jugar(object nombreHilo)
        {
            Random random = new Random();
            while (victoriasHilo1 < 3 && victoriasHilo2 < 3)
            {
                lock (lockObj)
                {
                    if (victoriasHilo1 >= 3 || victoriasHilo2 >= 3) break;

                    string movimientoHilo1 = movimientos[random.Next(movimientos.Count)];
                    string movimientoHilo2 = movimientos[random.Next(movimientos.Count)];

                    Console.WriteLine($"[{nombreHilo}] Jugador 1 juega {movimientoHilo1} contra Jugador 2 que juega {movimientoHilo2}");

                    int resultado = DeterminarGanador(movimientoHilo1, movimientoHilo2);
                    if (resultado == 1)
                    {
                        victoriasHilo1++;
                        Console.WriteLine($"Jugador 1 gana esta ronda. Marcador: {victoriasHilo1}-{victoriasHilo2}");
                    }
                    else if (resultado == -1)
                    {
                        victoriasHilo2++;
                        Console.WriteLine($"Jugador 2 gana esta ronda. Marcador: {victoriasHilo1}-{victoriasHilo2}");
                    }
                    else
                    {
                        Console.WriteLine("Empate en esta ronda.");
                    }
                }
                Thread.Sleep(500); // Pausa para simular el tiempo de juego
            }
        }

        static int DeterminarGanador(string movimiento1, string movimiento2)
        {
            if (movimiento1 == movimiento2)
                return 0; // Empate
            if (
                (movimiento1 == "piedra" && movimiento2 == "tijera") ||
                (movimiento1 == "tijera" && movimiento2 == "papel") ||
                (movimiento1 == "papel" && movimiento2 == "piedra")
                )
                return 1; // Gana el jugador 1
            return -1; // Gana el jugador 2
        }
    }
}

