# Logo - Final Documentation

```
Minh Nguyen Cong
317077
15/01/2022
```

## General
Creating a new language call as LOGO with turtles drawing on a board, with interpreter using C# and an interface build with .NET Window Form.

Language can do:

- Define function
- Define variables (int, float, string, turtle, ...)
- With Turtle object, can do action like Draw, Write, ... and get output as Image.

## Getting started

To get started with Logo, first clone this repository and setup development environment.

### Prerequisites:

Tools:

- Visual Studio
- Opencover (optional)
- ReportGenerator (optional)

Library:

- .NET Framework 3.7.2

### Starting project

To start the project, open the `Logo.sln` file with Visual Studio, build and run the solution.

Working screen:
![](https://i.imgur.com/Bc2AVT1.png)


## Logo language

In the language we have variable int, float, str (string), turtle (Turtle), color, coordinate.

The source file must contain one unargued "main" function, it is the function from which program execution begins. If function does not return anything so it will return null by default. Variables are passed to functions by reference, to create copy of variable, use syntax `copyof variable`. The interpreter does not allow you to declare global variables.

### Token

```
"AND", "OR", "NOT", "if", "else", "while", "return", "true", "false", "int", "str", "float", "bool", "color", "coordinate" "turtle_t","copyof",
"=", ".", ".", ";", "(", ")", "{", "}", ":", "#", "_", "*", "+", "-", "/", "%", "<", ">", "<=", ">=", "==", "!=", "__"
```

### Grammar

```
program        = header { functionDef } ;
header         = "#__" intNumber "_" intNumber "__"
functionDef    = signature parameters block ;
parameters     = "(" [ parameter { "," parameter } ] ")";
arguments      = "(" [ expression { "," expression } ] ")" ;
parameter      = signature ":" type;
block          = "{" {statement} "}";
statement      = ifStatement | whileStatement | returnStatement | assignStatement | functionCallStatement;
ifStatement    = "if" condition block [ "else" block ] ;
whileStatement = "while" "(" condition ")" block ;
returnStatement           = "return" expression ;
assignStatement           = id assignable ;
functionCallStatement     = id arguments ;
assignable     = assignmentOp expression ;
expression     = multiplicativeExpr { additiveOp multiplicativeExpr };
multiplicativeExpr        = primaryExpr { multiplicativeOp primaryExpr };
primaryExpr    = literal | color | id | parenthExpr | functionCall ;
parenthExpr    = "(" expression ")" ;
condition      = andCond { orOp andCond } ;
andCond        = equalityCond { andOp equalityCond } ;
equalityCond   = relationalCond [ equalOp relationalCond ] ;
relationalCond = primaryCond [ realtionOp primaryCond ] ;
primaryCond    = [ unaryOp ] ( parenthCond | expression ) ;
parenthCond    = "(" condition ")" ;
signature      = type id ;
unaryOp        = "!" ;
assignmentOp   = "=" ;
orOp           = "OR";
andOp          = "AND";
equalOp        = "==" | "!=";
relationOp     = "<" | ">" | "<=" | ">=" ;
additiveOp     = "+" | "-" ;
multiplicativeOp = "*" | "/" | "%" ;
literal        = ["-"] number;
intNumber      = naturalDigit {digit} ;
number         = (digit["."digit{digit}]) | ( naturalDigit {digit} [ "." digit {digit} ] ) ;
id             = letter { digit | letter } ;
digit          = "0".."9" ;
naturalDigit   = "1".."9" ;
type           = "int" | "float" | "bool" | "str" | "turtle_t" | "coordinate" | "color" ;
color          = "0x" hex_char hex_char hex_char hex_char hex_char hex_char;
hex_char       = "0".."9" | "a".."f" | "a".."f" ;
string         = { letter | digit } ;
```

### Usage

#### Rule
* Single command per line, no need to use semicolon.
* String need to be wrap in quote mark `"`.
* Define variable must have initial value.
* First line of file ***must*** be `#___width_height___`, with the width and height of the output image, also the size for the playground of turtle. The value can read from code by name `Board.VW` and `Board.VH`.
* The turtle will stop if it try to move outside of the border.


#### Predefined function and variable

- `copyof <expression>`: Return copied instance of a expression.
- `<turtle name> = Turtle()`: Create new turtle.
- `turtle.Pen.Color = 0xAABBCC`: Set color of turtle's pen.
- `turtle.Pen.Enable = True`: Set turtle drawing on/off.
- `turtle.Pen.Width = 1`: Size of turtle drawing.
- `turtle.Pen.TextSize = 10`: Size of turtle text.
- `turtle.Direction = 100`: Change direction of the turtle by 100 degree.
- `turtle.Hidden = False`: Change the visibility of the turtle icon on the final image.
- `turtle.Move(100)`: Turtle move to the current direction by 100 pixels.
- `turtle.MoveTo(100,100)`: Turtle move to point (100, 100) on the image.
- `turtle.MoveTo(coordinate)`: Turtle move to a coordinate on the image.
- `turtle.Write("Some Text")`: Turtle will write some text to image.
- `turtle.Write(1, 3, "Abc")`: Turtle will write text at specified position.

#### Variable
For every variable, data type can not be changed after defined.
Variable define only valid in the block
Define, assign:
```
x = 1
y = 0x00FF00
y.R = 100 * 0.25 ~~ Can be from 0 - 254
y = y * 0.25
coor = Coordinate(x, y)
```

Data types: bool, integer, float, string, Turtle (contains Pen, Direction), Coordinate, Board, Color.

#### Calculation, comparation
We support the basic math operation for integer number: `+`, `-`, `*`, `/` (auto rounded to rearest integer), `%` (devision with remainder).

For string, we support concat string by `+` operator (also string with number).

Comparation: `>`, `<`, `>=`, `<=` (number) and `==`, `!=`,  for number and string, color, coordinates.
Logic: `AND`, `OR`, `NOT` (can only work with boolean).
Negative negation: `-`.
Math and logic ordering by `(` and `)`.

#### Looping
```
while turtle.Position.X < Board.VW {
    turtle.Move(1)
}
```

#### Conditional statement

```
x = 1
if turtle.Position.X < Board.VW {
    turtle.MoveTo(100, 100)
} else { ~~~ optional
    turtle.Move()
}
```

#### Function
Function arguments will pass by reference, not by value.
```
function_name(x: int, y: int) {
    x = x+1
    return x+y
}

main() {
    g = 10
    h = 20
    function_name(copyof g, h)
}
```

#### Example code
Example 1:
```
#__300_300__
main() {
    turtle = Turtle()
    turtle.Pen.Enable = false
    turtle.Pen.Color = 0xFF0000
    turtle.MoveTo(100,150)
    turtle.Direction = 72
    turtle.Pen.Enable = true

    x = 0
    while x<5 {
        turtle.Move(100)
        turtle.Direction = (turtle.Direction+216)%360
        x = x+1
    }
}

```

Example 2
```
#__300_300__
main() {
    turtle = Turtle()
    turtle.Pen.Enable = true

    x = -150
    while x<300 {
        y=-x*x/50
        turtle.MoveTo(x+150, y+150)
        x = x+1
    }
}

```
Example 3
```
#__300_300__
main() {
    turtle = Turtle()
    turtle.Pen.Enable = false
    turtle.MoveTo(0,150)
    turtle.Direction = 0
    x = 0
turtle.Pen.Enable = true
    while x<10 {
if (x%2==0) {
turtle.Pen.Color = 0xFF0000
} else {
turtle.Pen.Color = 0x0000FF
}
turtle.Move(30)
       x = x+1
    }
}

```

Example 4
```
#__500_500__
recur(width: int, depth: int, t: turtle_t) {
    if (depth == 0) {
        t.Move(width)
    } else {
       recur(width / 3, depth - 1, t)
       t.Direction = (t.Direction+60)%360
       recur(width / 3, depth - 1, t)
       t.Direction = (t.Direction-120)%360
       recur(width / 3, depth - 1, t)
       t.Direction = (t.Direction+60)%360
       recur(width / 3, depth - 1, t)

    }
}

main() {
    turtle = Turtle()
    turtle.Pen.Enable = false
    turtle.Pen.Color = 0xFF0000
    turtle.MoveTo(0,250)
    turtle.Direction = 0
    turtle.Pen.Enable = true

    recur(500, 4, turtle)
}

```
Example 5
```
#__300_300__
draw_sierpinski(length: int, depth: int, turtle: turtle_t) {
    if (depth == 0) {
       i = 0
       while i < 3 {
          i=i+1
          turtle.Move(length)
          turtle.Direction = (turtle.Direction+120)%360
       }
   } else {
      draw_sierpinski(length/2, depth-1,turtle)
      turtle.Move(length/2)
      draw_sierpinski(length/2, depth-1, turtle)
      turtle.Direction = (turtle.Direction+120)%360
      turtle.Move(length/2)
      turtle.Direction = (turtle.Direction-120)%360
      draw_sierpinski(length/2, depth-1, turtle)
      turtle.Direction = (turtle.Direction-120)%360
      turtle.Move(length/2)
      turtle.Direction = (turtle.Direction+120)%360
   }
}
main() {
    turtle = Turtle()
    turtle.Pen.Enable = false
    turtle.Pen.Color = 0xFF0000
    turtle.MoveTo(100,150)
    turtle.Direction = 0
    turtle.Pen.Enable = true
    draw_sierpinski(100, 3, turtle)
}
```

### Technical detail

The project is implemented in C# using the .NET Framework. The NTest library was used for unit tests.

#### Test

Testing of this project includes:

- Lexer Test: 15 tests.
Check the correctness of the lexical parse.
- Parser Test: 10 tests.
Check the correctness of the parser.
- Interpreter Test: 20 tests.
Check the correctness of the Interpreter.

Project using `NUnit` as test library.

Coverage: 65%.