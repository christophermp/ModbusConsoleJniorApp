using System;
using EasyModbus;

namespace JNIOR_Modbus
{
    class Program
    {
        private const string IPAddress = "10.0.0.102";  // Replace with the IP address of your JNIOR device
        private const int Port = 502;  // Default Modbus port

        // Replace these with the appropriate register addresses for your JNIOR device
        private const int OutputsStartAddress = 8;
        private const int NumOutputs = 12;
        private const int InputsStartAddress = 0;
        private const int NumInputs = 4;
        static void Main(string[] args)
        {
            var client = new ModbusClient(IPAddress, Port);
            client.Connect();

            while (true)
            {
                ShowMenu();
                int choice = int.Parse(Console.ReadLine());

                if (choice == 0)
                {
                    break;
                }
                else if (choice >= 1 && choice <= 3)
                {
                    Console.Write("Enter output number (1-12): ");
                    int outputNumber = int.Parse(Console.ReadLine());

                    if (outputNumber >= 1 && outputNumber <= 12)
                    {
                        if (choice == 1)
                        {
                            PulseOutput(client, outputNumber);
                        }
                        else if (choice == 2)
                        {
                            SetOutput(client, outputNumber, true);
                        }
                        else if (choice == 3)
                        {
                            SetOutput(client, outputNumber, false);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid output number. Must be between 1 and 12.");
                    }
                }
                else if (choice == 4)
                {
                    ReadInputs(client);
                }
                else if (choice == 5)
                {
                    ReadOutputs(client);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }

            client.Disconnect();
        }
        static void ShowMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Pulse output");
            Console.WriteLine("2. Close output");
            Console.WriteLine("3. Open output");
            Console.WriteLine("4. Read input statuses");
            Console.WriteLine("5. Read output statuses");
            Console.WriteLine("0. Quit");
            Console.Write("Enter your choice: ");
        }

        static void ReadInputs(ModbusClient client)
        {
            bool[] inputs = client.ReadDiscreteInputs(InputsStartAddress, NumInputs);
            Console.WriteLine("Input statuses:");
            for (int i = 0; i < inputs.Length; i++)
            {
                Console.WriteLine($"Input {i + 1}: {(inputs[i] ? "ON" : "OFF")}");
            }
        }

        static void ReadOutputs(ModbusClient client)
        {
            bool[] outputs = client.ReadCoils(OutputsStartAddress, NumOutputs);
            Console.WriteLine("Output statuses:");
            for (int i = 0; i < outputs.Length; i++)
            {
                Console.WriteLine($"Output {i + 1}: {(outputs[i] ? "CLOSED" : "OPEN")}");
            }
        }

        static void SetOutput(ModbusClient client, int outputNumber, bool state)
        {
            int address = OutputsStartAddress + outputNumber - 1;
            client.WriteSingleCoil(address, state);
            Console.WriteLine($"Set output {outputNumber} {(state ? "CLOSED" : "OPEN")}");
        }

        static void PulseOutput(ModbusClient client, int outputNumber)
        {
            int address = OutputsStartAddress + outputNumber - 1;
            client.WriteSingleCoil(address, true); // Set the output to CLOSED state
            System.Threading.Thread.Sleep(5000); // Wait for 5 seconds
            client.WriteSingleCoil(address, false); // Set the output back to OPEN state
            Console.WriteLine($"Pulsed output {outputNumber} CLOSED for 5 seconds"); // Print the message
        }
    }
}

