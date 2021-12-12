using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Management;
using OpenHardwareMonitor.Hardware;

namespace Test
{
    class Program
    {
        public sealed class OpenHardwareMonitor { }

        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);


            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
       
        static float GetCPUTemp()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;

            computer.Accept(updateVisitor);

            float temp = 0;
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            if (j == 4)
                            {
                                temp = float.Parse(computer.Hardware[i].Sensors[j].Value.ToString()) +
                                        float.Parse(computer.Hardware[i].Sensors[j - 1].Value.ToString());
                                computer.Close();                             
                                return temp/2;
                            }
                        }
                    }
                }
            }
            return temp;
        }

        static float GetRAMLoad()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.RAMEnabled = true;

            computer.Accept(updateVisitor);

            float ram_load = 0;
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.RAM)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
                        {
                       
                            ram_load = float.Parse(computer.Hardware[i].Sensors[j].Value.ToString());

                            computer.Close();
                            return ram_load;


                            //if (j == 4)
                            //{
                            //    temp = float.Parse(computer.Hardware[i].Sensors[j].Value.ToString()) +
                            //            float.Parse(computer.Hardware[i].Sensors[j - 1].Value.ToString());
                            //    computer.Close();
                            //    return temp / 2;
                            //}
                        }
                    }
                }
            }
            return ram_load;
        }

        //static float GetCPULoad()
        //{
        //    UpdateVisitor updateVisitor = new UpdateVisitor();
        //    Computer computer = new Computer();
        //    computer.Open();
        //    computer.CPUEnabled = true;

        //    computer.Accept(updateVisitor);

        //    float cpu_load = 0;
        //    for (int i = 0; i < computer.Hardware.Length; i++)
        //    {
        //        if (computer.Hardware[i].HardwareType == HardwareType.CPU)
        //        {
        //            for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
        //            {
        //                if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
        //                {

        //                    if (j == 2)
        //                    {
        //                        cpu_load = float.Parse(computer.Hardware[i].Sensors[j].Value.ToString());
        //                        computer.Close();
        //                        return cpu_load;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return cpu_load;
        //}

        static float GetCPULoad_wmi()
        {
            float cpu_load = 0;
            System.Management.ManagementObjectSearcher man =
                         new System.Management.ManagementObjectSearcher("SELECT LoadPercentage  FROM Win32_Processor");
            foreach (System.Management.ManagementObject obj in man.Get())
                cpu_load =  float.Parse(obj["LoadPercentage"].ToString());

            return cpu_load;
        }
        static void Main(string[] args)
            {

                while (true)
                {
                    Console.WriteLine(GetCPUTemp());
                    Console.WriteLine(GetCPULoad_wmi());
                    Console.WriteLine(GetRAMLoad());
                   

                    Thread.Sleep(1000);

                    Console.Clear();

                }

         
        }


    }


}




       