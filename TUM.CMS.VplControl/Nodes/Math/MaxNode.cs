using System;
            using System.Windows;
            using System.Windows.Controls;
            using System.Xml;
            using TUM.CMS.VplControl.Core;

            namespace TUM.CMS.VplControl.Test.Nodes
            {
                public class MaxNode : Node
                {
                    public MaxNode(Core.VplControl hostCanvas)            : base(hostCanvas)

                    {

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
                        return new MaxNode(HostCanvas)
                        {
                            Top = Top,
                            Left = Left
                        };
                    }
                }
            }