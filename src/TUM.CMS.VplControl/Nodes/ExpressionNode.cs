using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using NCalc;
using NCalc.Domain;
using TUM.CMS.VplControl.Core;
using Xceed.Wpf.DataGrid;

namespace TUM.CMS.VplControl.Nodes
{
    public class ExpressionNode : Node
    {

        public ExpressionNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("Result", typeof(object));

            var textBox = new TextBox { MinWidth = 120, MaxWidth = 300, IsHitTestVisible = false };
            textBox.TextChanged += textBox_TextChanged;
            textBox.KeyUp += textBox_KeyUp;

            AddControlToNode(textBox);

            MouseEnter += ExpressionNode_MouseEnter;
            MouseLeave += ExpressionNode_MouseLeave;
        }

        void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        void ExpressionNode_MouseLeave(object sender, MouseEventArgs e)
        {
            Border.Focusable = true;
            Border.Focus();
            Border.Focusable = false;
        }

        void ExpressionNode_MouseEnter(object sender, MouseEventArgs e)
        {
            ControlElements[0].Focus();
        }

        private Expression expression;

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                if (textBox.Text != "")
                {
                    expression = new Expression(textBox.Text);

                    List<String> paras = GetParametersInExpression(textBox.Text).Distinct().ToList();

                    if (paras.Any())
                    {
                        RemoveAllInputPortsFromNode(paras);

                        List<String> filteredParas= paras.Where(parameter => InputPorts.All(p => p.Name != parameter)).ToList();

                        foreach (var parameter in filteredParas)
                        {
                            AddInputPortToNode(parameter, typeof(object));     
                        }
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

        public List<String> GetParametersInExpression(string formula)
        {
            try
            {
                var expr = Expression.Compile(formula, false);

                ParameterExtractionVisitor visitor = new ParameterExtractionVisitor();
                expr.Accept(visitor);

                return visitor.Parameters;
            }
            catch (Exception ex)
            {
                return  new List<string>();
            }


        }

        public override void Calculate()
        {
            if (expression != null)
            {
                foreach (var port in InputPorts)
                {
                    expression.Parameters[port.Name] = port.Data;
                }

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
            {
                OutputPorts[0].Data = null;
            }
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

    class ParameterExtractionVisitor : LogicalExpressionVisitor
    {
        public List<string> Parameters = new List<string>();

        public override void Visit(Identifier function)
        {
            //Parameter - add to list
            Parameters.Add(function.Name);
        }

        public override void Visit(UnaryExpression expression)
        {
            expression.Accept(this);
        }

        public override void Visit(BinaryExpression expression)
        {
            //Visit left and right
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(TernaryExpression expression)
        {
            //Visit left, right and middle
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
            expression.MiddleExpression.Accept(this);
        }

        public override void Visit(Function function)
        {
            foreach (var expression in function.Expressions)
            {
                expression.Accept(this);
            }
        }

        public override void Visit(LogicalExpression expression)
        {
            expression.Accept(this);
        }

        public override void Visit(ValueExpression expression)
        {

        }
    }
}
