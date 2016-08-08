using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using NCalc;
using TUM.CMS.VplControl.Core;
using TUM.CMS.VplControl.Utilities;

namespace TUM.CMS.VplControl.Nodes
{
    public class ExpressionNode : Node
    {
        private Expression expression;

        public ExpressionNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("Result", typeof (object));

            var textBox = new TextBox {MinWidth = 120, MaxWidth = 300, IsHitTestVisible = false};
            textBox.TextChanged += textBox_TextChanged;
            textBox.KeyUp += textBox_KeyUp;

            AddControlToNode(textBox);

            MouseEnter += ExpressionNode_MouseEnter;
            MouseLeave += ExpressionNode_MouseLeave;
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void ExpressionNode_MouseLeave(object sender, MouseEventArgs e)
        {
            Border.Focusable = true;
            Border.Focus();
            Border.Focusable = false;
        }

        private void ExpressionNode_MouseEnter(object sender, MouseEventArgs e)
        {
            ControlElements[0].Focus();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                if (textBox.Text != "")
                {
                    expression = new Expression(textBox.Text);

                    var paras = GetParametersInExpression(textBox.Text).Distinct().ToList();

                    if (paras.Any())
                    {
                        RemoveAllInputPortsFromNode(paras);

                        var filteredParas = paras.Where(parameter => InputPorts.All(p => p.Name != parameter)).ToList();

                        foreach (var parameter in filteredParas)
                            AddInputPortToNode(parameter, typeof (object));
                    }
                }
                else
                {
                    expression = null;
                    RemoveAllInputPortsFromNode();
                }
            }

            Calculate();
        }

        public List<string> GetParametersInExpression(string formula)
        {
            try
            {
                var expr = Expression.Compile(formula, false);

                var visitor = new ParameterExtractionVisitor();
                expr.Accept(visitor);

                return visitor.Parameters;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public override void Calculate()
        {
            if (expression != null)
            {
                foreach (var port in InputPorts)
                    expression.Parameters[port.Name] = port.Data;

                try
                {
                    OutputPorts[0].Data = expression.Evaluate();
                }
                catch (Exception ex)
                {
                    OutputPorts[0].Data = null;
                }
            }
            else
                OutputPorts[0].Data = null;
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            xmlWriter.WriteStartAttribute("Formula");
            xmlWriter.WriteValue(textBox.Text);
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            textBox.Text = xmlReader.GetAttribute("Formula");
        }

        public override Node Clone()
        {
            return new ExpressionNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}