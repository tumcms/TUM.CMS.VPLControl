using System;
using System.Collections.Generic;
using System.IO;

namespace TUM.CMS.NodeClassGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Input class name:");
            var name = Console.ReadLine();

            List<PortInfo> inputPortInfos= new List<PortInfo>();
            List<PortInfo> outputPortInfos = new List<PortInfo>();

            Console.WriteLine("New input port?");

            while (Console.ReadLine()=="y")
            {
                Console.WriteLine("Port name");
                var portName = Console.ReadLine();

                Console.WriteLine("Port type");
                var portType = Console.ReadLine();

                inputPortInfos.Add(new PortInfo(){Name=portName, Type = portType});
                Console.WriteLine("New input port?");
            }

            Console.WriteLine("New ouput port?");

            while (Console.ReadLine() == "y")
            {
                Console.WriteLine("Port name");
                var portName = Console.ReadLine();

                Console.WriteLine("Port type");
                var portType = Console.ReadLine();

                outputPortInfos.Add(new PortInfo() { Name = portName, Type = portType });
                Console.WriteLine("New output port?");
            }


            string fileString =
                @"using System;
            using System.Windows;
            using System.Windows.Controls;
            using System.Xml;
            using TUM.CMS.VplControl.Core;

            namespace TUM.CMS.VplControl.Test.Nodes
            {
                public class " + name + @" : Node
                {
                    public " + name + @"(Core.VplControl hostCanvas)            : base(hostCanvas)

                    {" + Environment.NewLine;

            foreach (var portInfo in inputPortInfos)
            {
                fileString += @"AddInputPortToNode(""" + portInfo.Name + @"""," + "typeof(" + portInfo.Type + "));" + Environment.NewLine;
            }

            foreach (var portInfo in outputPortInfos)
            {
                fileString += @"AddOutputPortToNode(""" + portInfo.Name + @"""," + "typeof(" + portInfo.Type + "));" + Environment.NewLine;
            }

            fileString +=@"
                    }

                    public override void Calculate()
                    {
                            OutputPorts[0].Data = InputPorts[0].Data;
                    }

                    public override void SerializeNetwork(XmlWriter xmlWriter)
                    {
                        base.SerializeNetwork(xmlWriter);

                        // add your xml serialization methods here
                    }

                    public override void DeserializeNetwork(XmlReader xmlReader)
                    {
                        base.DeserializeNetwork(xmlReader);

                        // add your xml deserialization methods here
                    }

                    public override Node Clone()
                    {
                        return new " + name + @"(HostCanvas)
                        {
                            Top = Top,
                            Left = Left
                        };
                    }
                }
            }";

            string path = @"c:\temp\" + name + ".cs";
            File.WriteAllText(path, fileString);

        }
    }

    public class PortInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}