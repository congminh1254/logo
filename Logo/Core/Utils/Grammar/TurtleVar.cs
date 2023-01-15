using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class TurtlePen
    {
        public Variable color = new Variable("Color", VariableType.COLOR, Color.Black);
        public Variable enable = new Variable("Enable", VariableType.BOOL, true);
        public Variable width = new Variable("Width", VariableType.INT, 1);
        public Variable textSize = new Variable("TextSize", VariableType.INT, 14);
        public TurtlePen()
        {

        }

        public object get(string name, int argsCount)
        {
            if (name == "Color")
                return color;
            if (name == "Enable")
                return enable;
            if (name == "Width")
                return width;
            if (name == "TextSize")
                return textSize;
            ErrorHandling.pushError(new ErrorHandling.LogoException("Params " + name + " of TurtlePen is not valid!"));
            return null;
        }

        public void set(string name, object value)
        {
            if (name == "Color" && value is Color)
            { color.value = value; return; }
            if (name == "Enable" && value is bool)
            { enable.value = value; ; return; }
            if (name == "Width" && value is int)
            { width.value = value; return; }
            if (name == "TextSize" && value is int)
            { textSize.value = value; return; }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Invalid value type " + value.GetType().Name + " for variable " + name));
        }
    }

    internal class TurtleVar
    {
        public Variable direction = new Variable("Direction", VariableType.INT, 0);
        public Variable hidden = new Variable("Hidden", VariableType.BOOL, false);
        public Variable pen = new Variable("Pen", VariableType.TURTLE_PEN, new TurtlePen());
        public Variable x = new Variable("X", VariableType.INT, 0);
        public Variable y = new Variable("Y", VariableType.INT, 0);
        public ChildFunction move;
        public ChildFunction moveToXY;
        public ChildFunction moveToCoord;
        public ChildFunction write;
        public ChildFunction writeXY;

        public TurtleVar()
        {
            initFunc();
        }

        public TurtleVar(int x, int y)
        {
            this.x.value = x;
            this.y.value = y;
            initFunc();
        }

        private void initFunc()
        {
            move = new ChildFunction(this, "Move", new List<DeclarationStatement>());
            moveToXY = new ChildFunction(this, "MoveTo", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.INT, null, "x", null, true),
                new DeclarationStatement(VariableType.INT, null, "y", null, true),
            });
            moveToCoord = new ChildFunction(this, "MoveTo", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.COORDINATE, null, "coordinate", null, true),
            });
            write = new ChildFunction(this, "Write", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.STR, null, "value", null, true)
            });
            writeXY = new ChildFunction(this, "Write", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.INT, null, "x", null, true),
                new DeclarationStatement(VariableType.INT, null, "y", null, true),
                new DeclarationStatement(VariableType.STR, null, "value", null, true)
            });
        }

        public object get(string name, int argsCount)
        {
            switch (name)
            {
                case "Direction":
                    return direction;
                case "Hidden":
                    return hidden;
                case "Pen":
                    return pen;
                case "x":
                    return x;
                case "y":
                    return y;
                case "Move":
                    return move;
                case "MoveTo":
                    if (argsCount == 1)
                        return moveToCoord;
                    if (argsCount == 2)
                        return moveToXY;
                    break;
                case "Write":
                    if (argsCount == 1)
                        return write;
                    if (argsCount == 2)
                        return writeXY;
                    break;
            }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Params " + name + " of Turtle is not valid!"));
            return null;
        }

        public void set(string name, object value)
        {
            if (name == "Direction" && value is int)
            { direction.value = (int)value; return; }
            if (name == "Hidden" && value is bool)
                {   hidden.value = (bool)value; ; return; }
            if (name == "x" && value is int)
                { x.value = (int)value; return; }
            if (name == "y" && value is int)
                { y.value = (int)value; return; }
            ErrorHandling.pushError(new ErrorHandling.LogoException("Invalid value type " + value.GetType().Name + " for variable " + name));
        }

        public object invoke(string func_name, Scope scope)
        {
            switch (func_name)
            {
                case "Move":
                    return Move(scope);
                case "MoveTo":
                    return MoveTo(scope);
                case "Write":
                    return Write(scope);
            }
            return null;
        }

        public object Move(Scope scope)
        {
            return null;
        }

        public object MoveTo(Scope scope)
        {
            if (scope.contains("x") && scope.contains("y"))
            {
                var x_pos = (int)scope.getVariable("x").value;
                var y_pos = (int)scope.getVariable("y").value;
                x.value = x_pos;
                y.value = y_pos;
                return null;
            }
            return null;
        }

        public object Write(Scope scope)
        {
            Variable var = scope.getVariable("value");
            Variable boardVar = scope.getVariable("Board");
            Board board = (Board)boardVar.value;

            int fontSize = (int)(((TurtlePen)pen.value).textSize).value;
            Font font = new Font("Microsoft Sans Serif",
                                 fontSize,
                                 FontStyle.Bold);
            Brush brush = Brushes.Black;
            int x_pos = (int)x.value;
            int y_pos = (int)y.value;
            
            using (Graphics g = Graphics.FromImage(board.bitmap))
            {
                g.DrawString((string)var.value, font, brush, x_pos, y_pos);
            }

            return null;
        }
    }
}
