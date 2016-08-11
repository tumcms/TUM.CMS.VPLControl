using System.Collections.Generic;
using NCalc.Domain;

namespace TUM.CMS.VplControl.Utilities
{
    public class ParameterExtractionVisitor : LogicalExpressionVisitor
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
                expression.Accept(this);
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