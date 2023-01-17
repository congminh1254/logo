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
        public Variable Color { get; set; } = new Variable(System.Drawing.Color.Black);
        public Variable Enable { get; set; } = new Variable(true);
        public Variable Width { get; set; } = new Variable(1);
        public Variable TextSize { get; set; } = new Variable(14);
        public TurtlePen()
        {

        }

        //public object get(string name, int argsCount)
        //{
        //    if (name == "Color")
        //        return color;
        //    if (name == "Enable")
        //        return enable;
        //    if (name == "Width")
        //        return width;
        //    if (name == "TextSize")
        //        return textSize;
        //    ErrorHandling.pushError(new ErrorHandling.LogoException("Params " + name + " of TurtlePen is not valid!"));
        //    return null;
        //}

        //public void set(string name, object value)
        //{
        //    if (name == "Color" && value is Color)
        //    { Color.value = value; return; }
        //    if (name == "Enable" && value is bool)
        //    { Enable.value = value; ; return; }
        //    if (name == "Width" && value is int)
        //    { Width.value = value; return; }
        //    if (name == "TextSize" && value is int)
        //    { TextSize.value = value; return; }
        //    ErrorHandling.pushError(new ErrorHandling.LogoException("Invalid value type " + value.GetType().Name + " for variable " + name));
        //}
    }

    internal class TurtleVar
    {
        public Variable Direction { get; set; } = new Variable(0);
        public Variable Hidden { get; set; } = new Variable(false);
        public Variable Pen { get; set; } = new Variable(new TurtlePen());
        public Variable X { get; set; } = new Variable(0);
        public Variable Y { get; set; } = new Variable(0);
        public ChildFunction Move { get; private set; }
        public ChildFunction MoveToXY { get; private set; }
        public ChildFunction MoveToCoord { get; private set; }
        public ChildFunction Write { get; private set; }
        public ChildFunction WriteXY { get; private set; }

        public TurtleVar()
        {
            initFunc();
        }

        public TurtleVar(int x, int y)
        {
            this.X.value = x;
            this.Y.value = y;
            initFunc();
        }

        private void initFunc()
        {
            Move = new ChildFunction(this, "Move", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.INT, null, "value", null, true),
            });
            MoveToXY = new ChildFunction(this, "MoveTo", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.INT, null, "x", null, true),
                new DeclarationStatement(VariableType.INT, null, "y", null, true),
            });
            MoveToCoord = new ChildFunction(this, "MoveTo", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.COORDINATE, null, "coordinate", null, true),
            });
            Write = new ChildFunction(this, "Write", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.STR, null, "value", null, true)
            });
            WriteXY = new ChildFunction(this, "Write", new List<DeclarationStatement>()
            {
                new DeclarationStatement(VariableType.INT, null, "x", null, true),
                new DeclarationStatement(VariableType.INT, null, "y", null, true),
                new DeclarationStatement(VariableType.STR, null, "value", null, true)
            });
        }

        //public object get(string name, int argsCount)
        //{
        //    switch (name)
        //    {
        //        case "Direction":
        //            return Direction;
        //        case "Hidden":
        //            return Hidden;
        //        case "Pen":
        //            return Pen;
        //        case "x":
        //            return X;
        //        case "y":
        //            return Y;
        //        case "Move":
        //            return Move;
        //        case "MoveTo":
        //            if (argsCount == 1)
        //                return MoveToCoord;
        //            if (argsCount == 2)
        //                return MoveToXY;
        //            break;
        //        case "Write":
        //            if (argsCount == 1)
        //                return Write;
        //            if (argsCount == 3)
        //                return WriteXY;
        //            break;
        //    }
        //    ErrorHandling.pushError(new ErrorHandling.LogoException("Params " + name + " of Turtle is not valid!"));
        //    return null;
        //}

        //public void set(string name, object value)
        //{
        //    if (name == "Direction" && value is int)
        //    { Direction.value = (int)value; return; }
        //    if (name == "Hidden" && value is bool)
        //        {   Hidden.value = (bool)value; ; return; }
        //    if (name == "x" && value is int)
        //        { X.value = (int)value; return; }
        //    if (name == "y" && value is int)
        //        { Y.value = (int)value; return; }
        //    ErrorHandling.pushError(new ErrorHandling.LogoException("Invalid value type " + value.GetType().Name + " for variable " + name));
        //}

        public object invoke(string func_name, Scope scope)
        {
            switch (func_name)
            {
                case "Move":
                    return move(scope);
                case "MoveTo":
                    return moveTo(scope);
                case "Write":
                    return write(scope);
            }
            return null;
        }

        public object move(Scope scope)
        {
            if (scope.contains("value"))
            {
                int value = (int)((Variable)scope.getVariable("value")).value;
                Board board = (Board)(scope.getVariable("Board")).value;
                TurtlePen turtlePen = (TurtlePen)Pen.value;
                int degree = (int)this.Direction.value;
                var radians = Math.PI * degree / 180.0;
                int size = (int)(turtlePen.Width).value;
                Color penColor = (Color)turtlePen.Color.value;
                
                Pen _pen = new Pen(penColor, size);
                var cos = Math.Round(Math.Cos(radians), 2);
                var sin = Math.Round(Math.Sin(radians), 2);
                var tan = Math.Round(Math.Tan(radians), 2);

                var x_delta = value * cos;
                var y_delta = value * sin;
                var cur_x = (int)X.value; var cur_y = (int)Y.value;
                var new_x = cur_x + x_delta;
                var new_y = cur_y - y_delta;
                if (new_x > (int)board.VW.value || new_x < 0)
                {
                    new_x = Math.Max(Math.Min(Convert.ToInt32(new_x), (int)board.VW.value), 0);
                    new_y = cur_y + tan * (cur_x - new_x);
                }
                if (new_y > (int)board.VH.value || new_y < 0)
                {
                    new_y = Math.Max(Math.Min(Convert.ToInt32(new_y), (int)board.VH.value), 0);
                    new_x = cur_x + (cur_y - new_y) / tan;
                }

                    using (var graphics = Graphics.FromImage(board.bitmap))
                {
                    if ((bool)turtlePen.Enable.value)
                        graphics.DrawLine(_pen, cur_x, cur_y, Convert.ToInt32(new_x), Convert.ToInt32(new_y));
                }
                X.value = Convert.ToInt32(new_x);
                Y.value = Convert.ToInt32(new_y);

            }
            return null;
        }

        public object moveTo(Scope scope)
        {
            int x_pos = 0, y_pos = 0;
            Variable boardVar = scope.getVariable("Board");
            Board board = (Board)boardVar.value;
            TurtlePen turtlePen = (TurtlePen)Pen.value;
            int size = (int)(turtlePen.Width).value;
            Color penColor = (Color)turtlePen.Color.value;
            Pen _pen = new Pen(penColor, size);
            var cur_x = (int)X.value; var cur_y = (int)Y.value;
            if (scope.contains("x") && scope.contains("y"))
            {
                x_pos = (int)scope.getVariable("x").value;
                y_pos = (int)scope.getVariable("y").value;
            } else if (scope.contains("coordinate"))
            {
                Coordinate coor = (Coordinate)scope.getVariable("coordinate").value;
                x_pos = coor.x; y_pos = coor.y;
            } else
            {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Provided params for function Turtle.MoveTo not valid!"));
                return null;
            }
            using (var graphics = Graphics.FromImage(board.bitmap))
            {
                if ((bool)turtlePen.Enable.value)
                    graphics.DrawLine(_pen, cur_x, cur_y, x_pos, y_pos);
            }
            X.value = x_pos;
            Y.value = y_pos;
            return null;
        }

        public object write(Scope scope)
        {
            Variable var = scope.getVariable("value");
            Variable boardVar = scope.getVariable("Board");
            Board board = (Board)boardVar.value;
            TurtlePen turtlePen = (TurtlePen)Pen.value;
            int fontSize = (int)(turtlePen.TextSize).value;
            Color penColor = (Color)turtlePen.Color.value;
            Font font = new Font("Microsoft Sans Serif",
                                 fontSize,
                                 FontStyle.Bold);
            Brush brush = new SolidBrush(penColor);
            int x_pos = (int)X.value;
            int y_pos = (int)Y.value;
            if (scope.contains("x") && scope.contains("y"))
            {
                x_pos = (int)scope.getVariable("x").value;
                y_pos = (int)scope.getVariable("y").value;
            }

            string value = var.value.ToString();


            using (Graphics g = Graphics.FromImage(board.bitmap))
            {
                if ((bool)turtlePen.Enable.value)
                    g.DrawString(value, font, brush, x_pos, y_pos);
            }

            return null;
        }
    }
}
