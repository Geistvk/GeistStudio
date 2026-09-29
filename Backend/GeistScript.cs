using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace GeistStudio
{
    public enum TokenType
    {
        End, Empty, NewLine, Number, StringLiteral, Identifier,
        Window, Button, TextBox, Label,
        Let, Const, Print, If, Else, While, For, Function, Return,
        Plus, Minus, Multiply, Divide, Modulo,
        Assign, Equal, NotEqual, And, Or, Not, String,
        Less, Greater, LessEqual, GreaterEqual,
        LParen, RParen, LBrace, RBrace, LBracket, RBracket,
        NewClass, New, Public, Private, Protected, Dot,
        Semicolon, Comma
    }

    public class Token
    {
        public TokenType Type = TokenType.End;
        public string Text = "";

        public Token() { }

        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;
        }
    }

    public class Lexer
    {
        private readonly string src;
        private int pos = 0;

        public Lexer(string s) { src = s; }

        public Token Next()
        {
            while (pos < src.Length)
            {
                while (pos < src.Length && char.IsWhiteSpace(src[pos]))
                    pos++;

                if (pos >= src.Length)
                    return new Token(TokenType.End, "");

                char c = src[pos];

                if (c == '/' && pos + 1 < src.Length && src[pos + 1] == '/')
                {
                    pos += 2;
                    while (pos < src.Length && src[pos] != '\n')
                        pos++;
                    continue;
                }

                if (c == '/' && pos + 1 < src.Length && src[pos + 1] == '*')
                {
                    pos += 2;
                    while (pos + 1 < src.Length)
                    {
                        if (src[pos] == '*' && src[pos + 1] == '/')
                        {
                            pos += 2;
                            break;
                        }
                        pos++;
                    }
                    continue;
                }

                if (char.IsDigit(c))
                {
                    string n = "";
                    while (pos < src.Length && char.IsDigit(src[pos]))
                        n += src[pos++];
                    return new Token(TokenType.Number, n);
                }

                if (char.IsLetter(c) || c == '_')
                {
                    string id = "";
                    while (pos < src.Length && (char.IsLetterOrDigit(src[pos]) || src[pos] == '_'))
                        id += src[pos++];

                    switch (id)
                    {
                        case "let": return new Token(TokenType.Let, id);
                        case "const": return new Token(TokenType.Const, id);
                        case "print": return new Token(TokenType.Print, id);
                        case "if": return new Token(TokenType.If, id);
                        case "else": return new Token(TokenType.Else, id);
                        case "while": return new Token(TokenType.While, id);
                        case "for": return new Token(TokenType.For, id);
                        case "function": return new Token(TokenType.Function, id);
                        case "return": return new Token(TokenType.Return, id);
                        case "nl": return new Token(TokenType.NewLine, id);
                        case "class": return new Token(TokenType.NewClass, id);
                        case "public": return new Token(TokenType.Public, id);
                        case "private": return new Token(TokenType.Private, id);
                        case "protected": return new Token(TokenType.Protected, id);
                        case "new": return new Token(TokenType.New, id);
                        case "Window": return new Token(TokenType.Window, id);
                        case "Button": return new Token(TokenType.Button, id);
                        case "TextBox": return new Token(TokenType.TextBox, id);
                        case "Label": return new Token(TokenType.Label, id);
                        default: return new Token(TokenType.Identifier, id);
                    }
                }

                if (c == '"')
                {
                    pos++;
                    string s = "";
                    while (pos < src.Length && src[pos] != '"')
                        s += src[pos++];
                    pos++;
                    return new Token(TokenType.StringLiteral, s);
                }

                pos++;

                switch (c)
                {
                    case '+': return new Token(TokenType.Plus, "+");
                    case '-': return new Token(TokenType.Minus, "-");
                    case '*': return new Token(TokenType.Multiply, "*");
                    case '/': return new Token(TokenType.Divide, "/");
                    case '%': return new Token(TokenType.Modulo, "%");
                    case '(': return new Token(TokenType.LParen, "(");
                    case ')': return new Token(TokenType.RParen, ")");
                    case '{': return new Token(TokenType.LBrace, "{");
                    case '}': return new Token(TokenType.RBrace, "}");
                    case '[': return new Token(TokenType.LBracket, "[");
                    case ']': return new Token(TokenType.RBracket, "]");
                    case ';': return new Token(TokenType.Semicolon, ";");
                    case ',': return new Token(TokenType.Comma, ",");
                    case '.': return new Token(TokenType.Dot, ".");
                    case '!':
                        if (pos < src.Length && src[pos] == '=')
                        {
                            pos++;
                            return new Token(TokenType.NotEqual, "!=");
                        }
                        return new Token(TokenType.Not, "!");

                    case '=':
                        if (pos < src.Length && src[pos] == '=')
                        {
                            pos++;
                            return new Token(TokenType.Equal, "==");
                        }
                        return new Token(TokenType.Assign, "=");

                    case '<':
                        if (pos < src.Length && src[pos] == '=')
                        {
                            pos++;
                            return new Token(TokenType.LessEqual, "<=");
                        }
                        return new Token(TokenType.Less, "<");

                    case '>':
                        if (pos < src.Length && src[pos] == '=')
                        {
                            pos++;
                            return new Token(TokenType.GreaterEqual, ">=");
                        }
                        return new Token(TokenType.Greater, ">");

                    case '&':
                        if (pos < src.Length && src[pos] == '&')
                        {
                            pos++;
                            return new Token(TokenType.And, "&&");
                        }
                        throw new Exception("Unexpected character '&'");

                    case '|':
                        if (pos < src.Length && src[pos] == '|')
                        {
                            pos++;
                            return new Token(TokenType.Or, "||");
                        }
                        throw new Exception("Unexpected character '|'");
                }

                throw new Exception("Unexpected character: " + c);
            }

            return new Token(TokenType.End, "");
        }
    }

    public class Value
    {
        public bool IsConst = false;
        public bool IsString = false;
        public bool IsObject = false;
        public long Number = 0;
        public string Str = "";
        public int Layer;
        public string Parent = "";
        public Token ClassName = new Token();
        public Token GeistObj = null;

        public static Value HandleVal(
            string par,
            int lay,
            bool isConstant,
            string vS = "",
            long vL = 0,
            Token className = null,
            Token geistObj = null)
        {
            var x = new Value();
            x.Layer = lay;
            x.Parent = par;
            x.ClassName = className ?? new Token();
            x.GeistObj = geistObj ?? new Token();
            x.IsObject = className != null;
            x.IsConst = isConstant;

            if (vS != "")
            {
                x.IsString = true;
                x.Str = vS;
            }
            else if (vL != 0)
            {
                x.Number = vL;
            }

            return x;
        }
    }

    public class Func
    {
        public int Layer;
        public string Parent = "";
        public string Name = "";
        public List<Token> Params = new List<Token>();
        public List<Token> Body = new List<Token>();
        public Dictionary<string, Value> LocalVars = new Dictionary<string, Value>();
        public bool HasReturned = false;
        public Token ReturnValue = new Token(TokenType.End, "");

        public Func() { }

        public Func(int layer, string parent, string name, List<Token> pars, List<Token> body)
        {
            Layer = layer;
            Parent = parent;
            Name = name;
            Params = pars;
            Body = body;
        }

        // Mirrors C++ struct-assignment (Func copy) into an already-referenced object.
        public void AssignFrom(Func other)
        {
            Layer = other.Layer;
            Parent = other.Parent;
            Name = other.Name;
            Params = new List<Token>(other.Params);
            Body = new List<Token>(other.Body);
            LocalVars = new Dictionary<string, Value>(other.LocalVars);
            HasReturned = other.HasReturned;
            ReturnValue = other.ReturnValue;
        }
    }

    public enum AccessModifier
    {
        Public,
        Private,
        Protected
    }

    public class ClassVariable
    {
        public Value Value = new Value();
        public AccessModifier Access = AccessModifier.Private;
    }

    public class ClassFunction
    {
        public Func Func = new Func();
        public AccessModifier Access = AccessModifier.Private;
    }

    public class Class
    {
        public string Name = "";
        public string Parent = "";
        public int Layer;

        public Dictionary<string, ClassVariable> Variables = new Dictionary<string, ClassVariable>();
        public Dictionary<string, ClassFunction> Functions = new Dictionary<string, ClassFunction>();

        public Class() { }

        public Class(string name, string parent, int layer)
        {
            Name = name;
            Parent = parent;
            Layer = layer;
        }
    }

    public class Object {
        public string Type = "";
        public string Var = "";
        public string Name = "";

        public string[] Titles = new string[]
        { 
            "width",
            "height",
            "x",
            "y"
        };

        public Dictionary<int, int> AttributeMap = new Dictionary<int, int>();
        public int[] Attributes = new int[] 
        { 
            0, 
            0, 
            0, 
            0 
        };

        public Object(string type, string var, string name)
        {
            Type = type;
            Var = var;
            Name = name;
        }
    }

    public class Interpreter
    {
        private readonly Dictionary<string, Value> vars = new Dictionary<string, Value>();
        private readonly Dictionary<string, Func> funcs = new Dictionary<string, Func>();
        private readonly Dictionary<string, Class> classes = new Dictionary<string, Class>();
        private readonly Dictionary<string, Object> objects = new Dictionary<string, Object>();

        private struct ConditionResult
        {
            public bool Value;
            public int NewPos;
        }

        private struct Arithmetic
        {
            public Token Val;
            public int NewPos;
        }

        // Replicates std::unordered_map::operator[] auto-vivification for missing keys.
        private static TV GetOrCreate<TV>(Dictionary<string, TV> dict, string key) where TV : class, new()
        {
            if (!dict.TryGetValue(key, out var v))
            {
                v = new TV();
                dict[key] = v;
            }
            return v;
        }

        private ClassFunction GetOrCreateClassFunc(string className, string funcName)
        {
            var cls = GetOrCreate(classes, className);
            if (!cls.Functions.TryGetValue(funcName, out var cf))
            {
                cf = new ClassFunction();
                cls.Functions[funcName] = cf;
            }
            return cf;
        }

        private ClassVariable GetOrCreateClassVar(string className, string varName)
        {
            var cls = GetOrCreate(classes, className);
            if (!cls.Variables.TryGetValue(varName, out var cv))
            {
                cv = new ClassVariable();
                cls.Variables[varName] = cv;
            }
            return cv;
        }

        private Token Combine(List<Token> tokens)
        {
            string combinedText = "";
            foreach (var t in tokens)
                combinedText += t.Text + " ";
            return new Token(TokenType.Empty, combinedText);
        }

        private bool IsTokenType(Token token)
        {
            return (token.Type >= TokenType.End &&
                    token.Type <= TokenType.Comma &&
                    token.Type != TokenType.Identifier);
        }

        private bool isGeistObj(Token token) {
            if (token != null)
            {
                switch (token.Type)
                {
                    case TokenType.Window: return true;
                    case TokenType.Button: return true;
                    case TokenType.TextBox: return true;
                    case TokenType.Label: return true;
                }
            }
            return false;
        }

        private bool isGeistObjVar(string parentFunc, Token token) 
        {
            if (token != null)
            {
                if (IsVarName(token) &&
                    isGeistObj(vars[token.Text].GeistObj)
                    )
                    return true;
                else if (IsLocalVarName(parentFunc, token))
                {
                    var func = GetOrCreate(funcs, parentFunc);
                    if (isGeistObj(func.LocalVars[token.Text].GeistObj))
                        return true;
                }
            }
            return false;
        }

        private bool IsVarName(Token token)
        {
            foreach (var v in vars)
                if (v.Key == token.Text)
                    return true;
            return false;
        }

        private bool IsLocalVarName(string parentFunc, Token token)
        {
            var func = GetOrCreate(funcs, parentFunc);
            foreach (var v in func.LocalVars)
                if (v.Key == token.Text)
                    return true;
            return false;
        }

        private Value TokenToValue(Token t)
        {
            string varStr = "";
            long varNum = 0;

            if (t.Type == TokenType.StringLiteral)
            {
                varStr = t.Text;
            }
            else if (t.Type == TokenType.Number)
            {
                varNum = long.Parse(t.Text);
            }
            else if (t.Type == TokenType.Identifier)
            {
                var v = GetOrCreate(vars, t.Text);
                if (v.IsString) varStr = v.Str;
                else varNum = v.Number;
            }

            return Value.HandleVal("root", 0, false, varStr, varNum);
        }

        private bool IsFuncName(Token token)
        {
            foreach (var f in funcs)
                if (f.Value.Name == token.Text)
                    return true;
            return false;
        }

        private bool IsClassName(string name) => classes.ContainsKey(name);
        private bool IsClassName(Token token) => IsClassName(token.Text);

        private bool IsClassVariable(string cls, Token varTok)
        {
            if (!IsClassName(cls))
                return false;

            var c = classes[cls];
            return c.Variables.ContainsKey(varTok.Text);
        }
        private bool IsClassVariable(Token cls, Token varTok) => IsClassVariable(cls.Text, varTok);

        private bool IsClassFunction(string cls, Token func)
        {
            if (!IsClassName(cls))
                return false;

            var c = classes[cls];
            return c.Functions.ContainsKey(func.Text);
        }
        private bool IsClassFunction(Token cls, Token func) => IsClassFunction(cls.Text, func);

        private bool IsClassAccess(List<Token> tokens, int pos)
        {
            if (pos + 2 >= tokens.Count)
                return false;

            return
                tokens[pos].Type == TokenType.Identifier &&
                tokens[pos + 1].Type == TokenType.Dot &&
                tokens[pos + 2].Type == TokenType.Identifier;
        }

        private void setGeistObjectData(
            string var, 
            string name, 
            string className,
            Dictionary<int, int> attributeMap = null) 
        {
            objects[var] = new Object(
                className,
                var,
                name
            );

            if (attributeMap == null)
                for (int i = 0; i < objects[var].Titles.Length; i++)
                    objects[var].AttributeMap[i] = 0;
            else 
                for (int i = 0; i < objects[var].Titles.Length; i++)
                    if (attributeMap[i] != 0 && objects[var].AttributeMap[i] != attributeMap[i])
                        objects[var].AttributeMap[i] = attributeMap[i];
        }

        private Object getObjectData(Token var) 
        { 
            Object found;
            if (objects.TryGetValue(var.Text, out found))
                return found;
            else 
                return null;
        }

        private string formatGeistObjectData(Token var) 
        {
            Object obj = getObjectData(var);

            if (obj == null)
                return "None";

            string output = ""
                + "{\n"
                + $"   Type: {obj.Type}\n"
                + $"   Name: '{obj.Name}'\n"
                + $"   Variable Name: '{obj.Var}'\n";

            for (int i = 0; i < obj.Titles.Length; i++)
                output += $"   {obj.Titles[i]}: {obj.AttributeMap[i]}\n";

            output += "}";

            return output;
        }

        private bool isValidAttribute(Token t)
        {
            switch (t.Text)
            {
                case "width":
                case "height":
                case "x":
                case "y":
                    return true;
                default:
                    return false;
            }
        }

        private int updateGeistObject(
            Token t,
            int Layer,
            string parent,
            int numLine,
            List<Token> tokens,
            int pos,
            string caller = "root",
            bool isRetCall = false,
            bool isConstructor = false)
        {
            Object GeistObj = getObjectData(t);
            string objVar = GeistObj != null ? GeistObj.Var : null;
            Dictionary<int, int> attributeMap = new Dictionary<int, int>();

            Token dot = tokens[pos++];
            if (dot.Type != TokenType.Dot)
            {
                ThrowError("Expected '.' after the class name.", dot, numLine);
                return pos;
            }

            Token Attribute = tokens[pos++];
            if (!isValidAttribute(Attribute))
            {
                ThrowError("Invalid attribute.", Attribute, numLine);
                return pos;
            }

            Token equal = tokens[pos++];
            if (equal.Type != TokenType.Assign)
            {
                ThrowError("Expected '=' after the Object attribute.", equal, numLine);
                return pos;
            }

            Arithmetic arithmetic = InitializeArithmetic(tokens, pos, parent, Layer, numLine);
            Token val = arithmetic.Val;
            pos = arithmetic.NewPos;

            for (int i = 0; i < GeistObj.Titles.Length; i++)
            {
                if (GeistObj.Titles[i] == Attribute.Text)
                {
                    GeistObj.Attributes[i] = int.Parse(val.Text);
                    objects[objVar].AttributeMap[i] = int.Parse(val.Text);

                    Terminal.WriteLine($"{GeistObj.Var}.{GeistObj.Titles[i]}: {GeistObj.Attributes[i]}");
                    break;
                }
            }

            Token semi = tokens[pos++];
            if (semi.Type != TokenType.Semicolon)
            {
                ThrowError("Expected ';' after the Object attribute assignment.", semi, numLine);
                return pos;
            }

            /*Terminal.WriteLine($"GeistObj.Type: {GeistObj.Type}");
            Terminal.WriteLine($"GeistObj.Name: {GeistObj.Name}");
            Terminal.WriteLine($"GeistObj.Var:  {GeistObj.Var}");*/
            return pos;
        }

        private int createGeistObject(
            Token t,
            int Layer,
            string parent,
            int numLine,
            List<Token> tokens,
            int pos,
            string caller = "root",
            bool isRetCall = false,
            bool isConstructor = false)
        {
            //Comming Soon
            //Implementation of for "let var = new Window("Title");"

            //Terminal.Write($"Creating Geist object: {t.Text}\n");
            pos++;

            Token varName = tokens[pos - 4];

            Token lParen = tokens[pos++];
            if (lParen.Type != TokenType.LParen)
            {
                ThrowError("Expected '(' after the function name.", lParen, numLine);
                return pos;
            }



            Arithmetic arithmetic = InitializeArithmetic(tokens, pos, parent, Layer, numLine);
            Token v = arithmetic.Val;
            pos = arithmetic.NewPos;
            String val = "";

            if (v.Type == TokenType.Number)
            {
                ThrowError($"The {t.Text} Name can't be a number.", v, numLine);
                return pos + 1;
            }

            if (v.Type == TokenType.StringLiteral)
                val = v.Text;
            else if (v.Type == TokenType.Identifier)
            {
                Value var2 = new Value();
                if (IsVarName(v) && !IsLocalVarName(parent, v))
                {
                    var2 = GetOrCreate(vars, v.Text);
                }
                else if (IsLocalVarName(parent, v))
                {
                    var2 = GetOrCreate(funcs, parent).LocalVars[v.Text];
                }
                else if (IsFuncName(v))
                {
                    Arithmetic ret = GetReturn(v, Layer, parent, numLine, tokens, pos, "print", true);
                    var2 = TokenToValue(ret.Val);
                    pos = ret.NewPos;
                }

                if ((var2.Parent != parent && var2.Layer > Layer) || var2.Layer > Layer)
                {
                    ThrowError("This variable is not accessible in the current scope.", v, numLine);
                    return pos;
                }



                if (!var2.IsString)
                {
                    ThrowError($"The {t.Text} Name can't be a number.", v, numLine);
                    return pos + 1;
                }
                val = var2.Str;
            }
            pos--;

            //if (val == "")
            //    val = t.Text; 

            if (val == "")
                val = tokens[pos - 5].Text;

            setGeistObjectData(varName.Text, val, t.Text);
            //Terminal.WriteLine($"{t.Text} Name: {val}");

            Token rParen = tokens[pos++];
            if (rParen.Type != TokenType.RParen)
            {
                ThrowError("Expected ')' to close the function arguments.", rParen, numLine);
                return pos;
            }

            Token semi = tokens[pos++];
            if (semi.Type != TokenType.Semicolon && !isRetCall && caller == "print")
            {
                ThrowError("Expected ';' after the function call.", semi, numLine);
                return pos;
            }

            return pos;
        }

        private int ExecuteFunction(
            Token t,
            int Layer,
            string parent,
            int numLine,
            List<Token> tokens,
            int pos,
            string caller = "root",
            bool isRetCall = false,
            bool isConstructor = false)
        {
            int newLayer = Layer;
            newLayer++;
            bool isClassFunc = false;

            var func = GetOrCreate(funcs, t.Text);
            func.Body.Add(new Token(TokenType.End, ""));

            if (IsClassName(parent) && !isConstructor)
            {
                Token funcName = tokens[pos++];
                isClassFunc = true;
                func.AssignFrom(GetOrCreateClassFunc(parent, funcName.Text).Func);
            }
            else if (IsClassName(parent) && isConstructor)
            {
                isClassFunc = true;
                func.AssignFrom(GetOrCreateClassFunc(parent, "constructor").Func);
            }

            func.HasReturned = false;
            func.ReturnValue = new Token(TokenType.End, "");

            var args = new List<Token>();

            if ((func.Parent != parent && func.Layer > Layer) || func.Layer > Layer)
            {
                ThrowError("This function is not accessible in the current scope.", t, numLine);
                return pos;
            }

            Token lParen = tokens[pos++];
            if (isConstructor) lParen = tokens[pos++];
            if (lParen.Type != TokenType.LParen)
            {
                ThrowError("Expected '(' after the function name.", lParen, numLine);
                return pos;
            }

            Token p = tokens[pos++];
            while (p.Type != TokenType.LParen &&
                   p.Type != TokenType.RParen &&
                   p.Type != TokenType.LBrace &&
                   p.Type != TokenType.RBrace)
            {
                Token arg = p;
                if (arg.Type != TokenType.Comma)
                {
                    if (IsTokenType(arg) &&
                        arg.Type != TokenType.Number &&
                        arg.Type != TokenType.StringLiteral)
                    {
                        ThrowError("The variable name is invalid.", arg, numLine);
                        return pos;
                    }
                    args.Add(arg);
                }
                p = tokens[pos++];
            }
            pos--;

            Token rParen = tokens[pos++];
            if (rParen.Type != TokenType.RParen)
            {
                ThrowError("Expected ')' to close the function arguments.", rParen, numLine);
                return pos;
            }

            Token semi = tokens[pos++];
            if (semi.Type != TokenType.Semicolon && !isRetCall && caller == "print")
            {
                ThrowError("Expected ';' after the function call.", semi, numLine);
                return pos;
            }

            if (args.Count != func.Params.Count)
            {
                ThrowError("The function was called with the wrong number of arguments.", new Token(TokenType.End, func.Name), numLine);
                return pos;
            }

            for (int i = 0; i < args.Count; i++)
            {
                Token arg = args[i];
                bool isConst = false;
                string varStr = "";
                long varNum = 0;

                if (!IsVarName(arg) &&
                    arg.Type != TokenType.StringLiteral &&
                    arg.Type != TokenType.Number)
                {
                    ThrowError("One or more arguments are invalid or undefined.", args[i], numLine);
                    return pos;
                }
                else if (arg.Type == TokenType.Identifier || IsVarName(arg))
                {
                    var v = GetOrCreate(vars, arg.Text);
                    if ((v.Parent != parent && v.Layer > Layer) || v.Layer > Layer)
                    {
                        ThrowError("This variable is not accessible in the current scope.", arg, numLine);
                        return pos;
                    }
                    isConst = v.IsConst;

                    if (v.IsString) varStr = v.Str;
                    else varNum = v.Number;
                }
                else if (arg.Type == TokenType.StringLiteral)
                {
                    varStr = arg.Text;
                }
                else if (arg.Type == TokenType.Number)
                {
                    varNum = long.Parse(arg.Text);
                }

                if (!isClassFunc)
                {
                    func.LocalVars[func.Params[i].Text] = Value.HandleVal(
                        func.Name, newLayer, isConst, varStr, varNum);
                }
                else
                {
                    var cv = GetOrCreateClassVar(parent, func.Params[i].Text);
                    cv.Value = Value.HandleVal(func.Name, newLayer, isConst, varStr, varNum, new Token(TokenType.Empty, parent));
                }
            }

            ExecuteTokens(func.Body, func.Name, newLayer, numLine);

            return pos;
        }

        private Arithmetic GetReturn(
            Token t,
            int Layer,
            string parent,
            int numLine,
            List<Token> tokens,
            int pos,
            string caller = "root",
            bool isRetCall = false)
        {
            int newPos = pos;
            if (IsFuncName(t))
            {
                newPos = ExecuteFunction(t, Layer, parent, numLine, tokens, pos, caller, isRetCall);

                var func = GetOrCreate(funcs, t.Text);
                if (func.HasReturned)
                {
                    return new Arithmetic { Val = func.ReturnValue, NewPos = newPos };
                }
                else
                {
                    ThrowError("The function has not returned a value yet.", t, numLine);
                    return new Arithmetic { Val = new Token(TokenType.End, ""), NewPos = newPos };
                }
            }
            else
            {
                ThrowError("The function does not exist or is not accessible in the current scope.", t, numLine);
                return new Arithmetic { Val = new Token(TokenType.End, ""), NewPos = newPos };
            }
        }

        private void ThrowError(string error, Token token, int numLine)
        {
            string errorMsg = error + " '" + token.Text + "' (Line " + numLine + ")\n";
            Terminal.Write(errorMsg);
        }

        private Token HandleArithmetic(List<Token> operations)
        {
            var input = new List<Token>(operations);

            while (true)
            {
                int depth = 0;
                bool changed = false;

                for (int i = 0; i < input.Count; i++)
                {
                    if (input[i].Type == TokenType.LParen)
                    {
                        depth++;
                    }
                    else if (input[i].Type == TokenType.RParen)
                    {
                        depth--;

                        if (depth == 0)
                        {
                            var inner = input.GetRange(1, i - 1);
                            Token result = HandleArithmetic(inner);

                            input.RemoveRange(0, i + 1);
                            input.Insert(0, result);

                            changed = true;
                            break;
                        }
                    }
                }

                if (!changed)
                    break;
            }

            var expr = new List<Token>();
            foreach (var t in input)
            {
                if (t.Type == TokenType.LParen || t.Type == TokenType.RParen)
                    continue;
                expr.Add(t);
            }

            if (expr.Count == 1)
                return expr[0];

            if (expr.Count == 0)
                return new Token(TokenType.End, "");

            if (expr.Count == 1)
                return expr[0];

            int Precedence(TokenType tt)
            {
                switch (tt)
                {
                    case TokenType.Multiply:
                    case TokenType.Divide:
                    case TokenType.Modulo:
                        return 3;

                    case TokenType.Plus:
                    case TokenType.Minus:
                        return 2;

                    case TokenType.Less:
                    case TokenType.Greater:
                    case TokenType.LessEqual:
                    case TokenType.GreaterEqual:
                    case TokenType.Equal:
                    case TokenType.NotEqual:
                        return 1;

                    case TokenType.And:
                        return 0;

                    case TokenType.Or:
                        return 0;

                    default:
                        return -1;
                }
            }

            Token Apply(Token lhs, Token op, Token rhs)
            {
                long ToNumber(Token tk) => long.Parse(tk.Text);

                if (op.Type == TokenType.Plus &&
                    (lhs.Type == TokenType.StringLiteral || rhs.Type == TokenType.StringLiteral))
                {
                    string lText = lhs.Type != TokenType.StringLiteral ? long.Parse(lhs.Text).ToString() : lhs.Text;
                    string rText = rhs.Type != TokenType.StringLiteral ? long.Parse(rhs.Text).ToString() : rhs.Text;

                    return new Token(TokenType.StringLiteral, lText + rText);
                }

                if (op.Type == TokenType.Equal ||
                    op.Type == TokenType.NotEqual ||
                    op.Type == TokenType.Less ||
                    op.Type == TokenType.Greater ||
                    op.Type == TokenType.LessEqual ||
                    op.Type == TokenType.GreaterEqual)
                {
                    long a = ToNumber(lhs);
                    long b = ToNumber(rhs);
                    bool r = false;

                    switch (op.Type)
                    {
                        case TokenType.Equal: r = (a == b); break;
                        case TokenType.NotEqual: r = (a != b); break;
                        case TokenType.Less: r = (a < b); break;
                        case TokenType.Greater: r = (a > b); break;
                        case TokenType.LessEqual: r = (a <= b); break;
                        case TokenType.GreaterEqual: r = (a >= b); break;
                    }

                    return new Token(TokenType.Number, r ? "1" : "0");
                }

                if (op.Type == TokenType.And || op.Type == TokenType.Or)
                {
                    long a = ToNumber(lhs);
                    long b = ToNumber(rhs);
                    bool r;

                    if (op.Type == TokenType.And)
                        r = (a != 0 && b != 0);
                    else
                        r = (a != 0 || b != 0);

                    return new Token(TokenType.Number, r ? "1" : "0");
                }

                long an, bn;
                try
                {
                    an = ToNumber(lhs);
                    bn = ToNumber(rhs);
                }
                catch
                {
                    throw new Exception("This operation requires numeric values.");
                }

                var result = new Token { Type = TokenType.Number };

                switch (op.Type)
                {
                    case TokenType.Plus:
                        result.Text = (an + bn).ToString();
                        break;

                    case TokenType.Minus:
                        result.Text = (an - bn).ToString();
                        break;

                    case TokenType.Multiply:
                        result.Text = (an * bn).ToString();
                        break;

                    case TokenType.Divide:
                        if (bn == 0)
                            throw new Exception("Division by zero is not allowed.");
                        result.Text = (an / bn).ToString();
                        break;

                    case TokenType.Modulo:
                        if (bn == 0)
                            throw new Exception("Modulo by zero is not allowed.");
                        result.Text = (an % bn).ToString();
                        break;

                    default:
                        throw new Exception("The operator is not supported.");
                }

                return result;
            }

            var values = new List<Token>();
            var ops = new List<Token>();

            foreach (var t in expr)
            {
                if (t.Type == TokenType.Number || t.Type == TokenType.StringLiteral)
                {
                    values.Add(t);
                }
                else if (
                    t.Type == TokenType.Plus ||
                    t.Type == TokenType.Minus ||
                    t.Type == TokenType.Multiply ||
                    t.Type == TokenType.Divide ||
                    t.Type == TokenType.Modulo ||
                    t.Type == TokenType.Equal ||
                    t.Type == TokenType.NotEqual ||
                    t.Type == TokenType.Less ||
                    t.Type == TokenType.Greater ||
                    t.Type == TokenType.LessEqual ||
                    t.Type == TokenType.GreaterEqual ||
                    t.Type == TokenType.And ||
                    t.Type == TokenType.Or)
                {
                    while (ops.Count > 0 &&
                           Precedence(ops[ops.Count - 1].Type) >= Precedence(t.Type))
                    {
                        if (values.Count < 2 || ops.Count == 0)
                            throw new Exception("Invalid expression structure.");

                        Token rhs = values[values.Count - 1]; values.RemoveAt(values.Count - 1);
                        Token lhs = values[values.Count - 1]; values.RemoveAt(values.Count - 1);
                        Token op = ops[ops.Count - 1]; ops.RemoveAt(ops.Count - 1);

                        values.Add(Apply(lhs, op, rhs));
                    }

                    ops.Add(t);
                }
                else
                {
                    ThrowError("Invalid token in expression: ", t, 0);
                }
            }

            while (ops.Count > 0)
            {
                if (values.Count == 0)
                    return null;

                if (values.Count < 2)
                    return values[0];

                Token rhs = values[values.Count - 1]; values.RemoveAt(values.Count - 1);
                Token lhs = values[values.Count - 1]; values.RemoveAt(values.Count - 1);
                Token op = ops[ops.Count - 1]; ops.RemoveAt(ops.Count - 1);

                values.Add(Apply(lhs, op, rhs));
            }

            if (values.Count == 0)
                return new Token(TokenType.End, "");

            return values[values.Count - 1];
        }

        private Arithmetic InitializeArithmetic(
            List<Token> tokens,
            int pos,
            string parent,
            int Layer,
            int numLine,
            Token className = null,
            Token varName = null)
        {
            if (className == null) className = new Token(TokenType.Empty, "root");
            if (varName == null) varName = new Token(TokenType.Empty, "");

            var operations = new List<Token>();

            Token val = tokens[pos++];
            while (val.Type != TokenType.Semicolon)
            {
                Token operation = val;

                if (IsVarName(val) ||
                    IsLocalVarName(parent, val) ||
                    IsFuncName(val) ||
                    IsClassVariable(parent, val) ||
                    IsClassVariable(className, val))
                {
                    Value v = null;
                    if (IsVarName(val))
                        v = GetOrCreate(vars, val.Text);
                    else if (IsLocalVarName(parent, val))
                        v = GetOrCreate(funcs, parent).LocalVars[val.Text];
                    else if (IsFuncName(val))
                    {
                        Arithmetic ret = GetReturn(val, Layer, parent, numLine, tokens, pos, "arithmetic", true);
                        v = TokenToValue(ret.Val);
                        pos = ret.NewPos;
                        pos--;
                    }
                    else if (IsClassVariable(parent, val))
                    {
                        v = GetOrCreateClassVar(parent, val.Text).Value;
                    }
                    else if (IsClassVariable(className, val))
                    {
                        v = GetOrCreateClassVar(className.Text, val.Text).Value;
                    }

                    if (v.IsString)
                        operation = new Token(TokenType.StringLiteral, v.Str);
                    else
                        operation = new Token(TokenType.Number, v.Number.ToString());
                }

                operations.Add(operation);
                val = tokens[pos++];
            }

            Token result = HandleArithmetic(operations);
            pos--;

            return new Arithmetic { Val = result, NewPos = pos };
        }

        private Token ResolveToken(Token t, string parent)
        {
            if (t.Type == TokenType.Identifier)
            {
                if (IsVarName(t))
                {
                    var v = GetOrCreate(vars, t.Text);
                    if (v.IsString)
                        return new Token(TokenType.StringLiteral, v.Str);
                    return new Token(TokenType.Number, v.Number.ToString());
                }
                else if (IsLocalVarName(parent, t))
                {
                    var v = GetOrCreate(funcs, parent).LocalVars[t.Text];
                    if (v.IsString)
                        return new Token(TokenType.StringLiteral, v.Str);
                    return new Token(TokenType.Number, v.Number.ToString());
                }
                else
                    throw new Exception("Unknown variable in condition: " + t.Text);
            }

            return t;
        }

        private bool EvaluateCondition(string parent, List<Token> exprIn)
        {
            var expr = new List<Token>(exprIn);

            for (int i = 0; i < expr.Count; i++)
                expr[i] = ResolveToken(expr[i], parent);

            while (expr.Count >= 2 &&
                   expr[0].Type == TokenType.LParen &&
                   expr[expr.Count - 1].Type == TokenType.RParen)
            {
                int depth = 0;
                bool remove = true;

                for (int i = 0; i < expr.Count; i++)
                {
                    if (expr[i].Type == TokenType.LParen)
                        depth++;

                    if (expr[i].Type == TokenType.RParen)
                        depth--;

                    if (depth == 0 && i != expr.Count - 1)
                    {
                        remove = false;
                        break;
                    }
                }

                if (!remove)
                    break;

                expr.RemoveAt(0);
                expr.RemoveAt(expr.Count - 1);
            }

            // OR
            {
                int depth = 0;
                for (int i = 0; i < expr.Count; i++)
                {
                    if (expr[i].Type == TokenType.LParen) depth++;
                    else if (expr[i].Type == TokenType.RParen) depth--;

                    if (depth == 0 && expr[i].Type == TokenType.Or)
                    {
                        var left = expr.GetRange(0, i);
                        var right = expr.GetRange(i + 1, expr.Count - i - 1);

                        return EvaluateCondition(parent, left) ||
                               EvaluateCondition(parent, right);
                    }
                }
            }

            // AND
            {
                int depth = 0;
                for (int i = 0; i < expr.Count; i++)
                {
                    if (expr[i].Type == TokenType.LParen) depth++;
                    else if (expr[i].Type == TokenType.RParen) depth--;

                    if (depth == 0 && expr[i].Type == TokenType.And)
                    {
                        var left = expr.GetRange(0, i);
                        var right = expr.GetRange(i + 1, expr.Count - i - 1);

                        return EvaluateCondition(parent, left) &&
                               EvaluateCondition(parent, right);
                    }
                }
            }

            int depth2 = 0;
            for (int i = 0; i < expr.Count; i++)
            {
                if (expr[i].Type == TokenType.LParen)
                    depth2++;
                else if (expr[i].Type == TokenType.RParen)
                    depth2--;

                if (depth2 != 0)
                    continue;

                TokenType op = expr[i].Type;

                if (op == TokenType.Equal ||
                    op == TokenType.NotEqual ||
                    op == TokenType.Less ||
                    op == TokenType.Greater ||
                    op == TokenType.LessEqual ||
                    op == TokenType.GreaterEqual)
                {
                    var left = expr.GetRange(0, i);
                    var right = expr.GetRange(i + 1, expr.Count - i - 1);

                    Token lhs = HandleArithmetic(left);
                    Token rhs = HandleArithmetic(right);

                    if (lhs.Type == TokenType.StringLiteral ||
                        rhs.Type == TokenType.StringLiteral)
                    {
                        switch (op)
                        {
                            case TokenType.Equal:
                                return lhs.Text == rhs.Text;
                            case TokenType.NotEqual:
                                return lhs.Text != rhs.Text;
                            default:
                                throw new Exception("Only == and != are allowed for strings.");
                        }
                    }

                    long a = long.Parse(lhs.Text);
                    long b = long.Parse(rhs.Text);

                    switch (op)
                    {
                        case TokenType.Equal: return a == b;
                        case TokenType.NotEqual: return a != b;
                        case TokenType.Less: return a < b;
                        case TokenType.Greater: return a > b;
                        case TokenType.LessEqual: return a <= b;
                        case TokenType.GreaterEqual: return a >= b;
                    }
                }
            }

            Token result = HandleArithmetic(expr);

            if (result.Type == TokenType.StringLiteral)
                return !string.IsNullOrEmpty(result.Text);

            return long.Parse(result.Text) != 0;
        }

        private bool EvaluateFlatCondition(string parent, List<Token> expr)
        {
            long ToInt(string s)
            {
                if (string.IsNullOrEmpty(s))
                    throw new Exception("Empty value in condition");

                foreach (char c in s)
                    if (!char.IsDigit(c) && c != '-')
                        throw new Exception("Non-numeric value in condition: " + s);

                return long.Parse(s);
            }

            if (expr.Count == 1)
                return ToInt(expr[0].Text) != 0;

            for (int i = 1; i + 1 < expr.Count; i++)
            {
                Token op = expr[i];

                long left = ToInt(expr[i - 1].Text);
                long right = ToInt(expr[i + 1].Text);

                switch (op.Type)
                {
                    case TokenType.Equal: return left == right;
                    case TokenType.NotEqual: return left != right;
                    case TokenType.Less: return left < right;
                    case TokenType.Greater: return left > right;
                    case TokenType.LessEqual: return left <= right;
                    case TokenType.GreaterEqual: return left >= right;
                    case TokenType.And: return left != 0 && right != 0;
                    case TokenType.Or: return left != 0 || right != 0;
                }
            }

            throw new Exception("Invalid condition expression.");
        }

        private ConditionResult InitializeCondition(List<Token> tokens, int pos, string parent)
        {
            var expression = new List<Token>();
            int parenDepth = 1;

            while (pos < tokens.Count)
            {
                Token t = tokens[pos++];

                if (t.Type == TokenType.LParen)
                    parenDepth++;

                if (t.Type == TokenType.RParen)
                {
                    parenDepth--;
                    if (parenDepth == 0)
                        break;
                }

                if (IsVarName(t))
                {
                    var v = GetOrCreate(vars, t.Text);
                    if (v.IsString)
                        expression.Add(new Token(TokenType.StringLiteral, v.Str));
                    else
                        expression.Add(new Token(TokenType.Number, v.Number.ToString()));
                }
                else if (IsLocalVarName(parent, t))
                {
                    var v = GetOrCreate(funcs, parent).LocalVars[t.Text];
                    if (v.IsString)
                        expression.Add(new Token(TokenType.StringLiteral, v.Str));
                    else
                        expression.Add(new Token(TokenType.Number, v.Number.ToString()));
                }
                else
                {
                    expression.Add(t);
                }
            }

            bool result = EvaluateCondition(parent, expression);

            return new ConditionResult { Value = result, NewPos = pos };
        }

        private int RegisterVar(
            List<Token> tokens,
            int pos,
            int Layer,
            string parent,
            int numLine,
            Token t,
            Token classOutName = null)
        {
            if (classOutName == null) classOutName = new Token(TokenType.Empty, "root");

            bool isConst = false;
            string par = parent;
            int lay = Layer;
            Token name = new Token();
            string varStr = "";
            long varNum = 0;

            if (!IsVarName(t) && !IsLocalVarName(parent, t) &&
                !IsClassVariable(parent, t))
            {
                if (t.Type == TokenType.Const)
                    isConst = true;
                name = tokens[pos++];

                if (IsTokenType(name))
                {
                    ThrowError("The variable name is invalid.", Combine(new List<Token> { tokens[pos - 2], tokens[pos - 1], tokens[pos] }), numLine);
                    return pos;
                }
            }
            else if ((IsVarName(t) && GetOrCreate(vars, t.Text).IsConst) ||
                     (IsLocalVarName(parent, t) && GetOrCreate(funcs, parent).LocalVars[t.Text].IsConst))
            {
                ThrowError("Cannot modify the value of a constant.", Combine(new List<Token> { t, name }), numLine);
                return pos;
            }
            else if (IsClassVariable(parent, t) &&
                     t.Type != TokenType.Let && t.Type != TokenType.Const)
            {
                Token dot = tokens[pos++];
                if (dot.Type != TokenType.Dot)
                {
                    ThrowError("Expected '.' after the class name.", dot, numLine);
                    return pos;
                }

                if (IsClassVariable(parent, t))
                {
                    name = t;
                    par = t.Text;
                    lay = Layer;
                }
                else
                {
                    ThrowError("Expected a class Variable after the class name.", tokens[pos], numLine);
                    return pos;
                }
            }
            else
            {
                name = t;
                if (IsVarName(t))
                {
                    par = GetOrCreate(vars, name.Text).Parent;
                    lay = GetOrCreate(vars, name.Text).Layer;
                }
                else if (IsLocalVarName(parent, t))
                {
                    par = GetOrCreate(funcs, parent).LocalVars[t.Text].Parent;
                    lay = GetOrCreate(funcs, parent).LocalVars[t.Text].Layer;
                }
                else if (IsClassName(parent))
                {
                    par = GetOrCreateClassVar(parent, t.Text).Value.Parent;
                    lay = GetOrCreateClassVar(parent, t.Text).Value.Layer;
                }
            }

            if (name.Type == TokenType.Let || name.Type == TokenType.Const)
                name = tokens[pos++];

            Token op = tokens[pos++];

            if (op.Type == TokenType.Assign)
            {
                if (tokens[pos].Type == TokenType.New)
                {
                    pos++;
                    Token className = tokens[pos++];

                    if (!IsClassName(className) && !isGeistObj(className))
                    {
                        ThrowError("Unknown class.", className, numLine);
                        return pos;
                    }

                    
                    if ((IsVarName(name) || !IsVarName(name)) && !IsLocalVarName(parent, name) && !isGeistObj(className))
                    {
                        vars[name.Text] = Value.HandleVal(
                            par, 
                            lay, 
                            isConst, 
                            varStr, 
                            varNum, 
                            className
                        );
                    }
                    else if (IsLocalVarName(parent, name) && !isGeistObj(className))
                    {
                        GetOrCreate(funcs, parent).LocalVars[name.Text] = Value.HandleVal(
                            par, 
                            lay, 
                            isConst, 
                            varStr, 
                            varNum, 
                            className
                        );
                    }

                    if (!isGeistObj(className))
                        pos = ExecuteFunction(
                            tokens[pos - 1],
                            Layer + 1,
                            className.Text,
                            numLine,
                            tokens,
                            pos - 1,
                            "constructor",
                            false,
                            true
                        );
                    else
                        pos = createGeistObject(
                            tokens[pos - 1],
                            Layer + 1,
                            className.Text,
                            numLine,
                            tokens,
                            pos - 1,
                            "constructor",
                            false,
                            true
                        );

                    if (isGeistObj(className))
                    {
                        vars[name.Text] = Value.HandleVal(
                            par,
                            lay,
                            isConst,
                            formatGeistObjectData(name),
                            0L,
                            className, 
                            className
                        );
                    }

                    return pos;
                }
                else
                {
                    Arithmetic arithmetic = InitializeArithmetic(tokens, pos, parent, Layer, numLine, classOutName, name);
                    Token val = arithmetic.Val;
                    pos = arithmetic.NewPos;

                    if (val.Type == TokenType.Number)
                        varNum = long.Parse(val.Text);
                    else if (val.Type == TokenType.StringLiteral)
                        varStr = val.Text;
                }
            }
            else if (op.Type == TokenType.And || op.Type == TokenType.Or ||
                     op.Type == TokenType.Equal || op.Type == TokenType.NotEqual ||
                     op.Type == TokenType.Less || op.Type == TokenType.Greater ||
                     op.Type == TokenType.LessEqual || op.Type == TokenType.GreaterEqual)
            {
                var condition = new List<Token> { name, op };

                while (pos < tokens.Count)
                {
                    Token next = tokens[pos++];
                    if (next.Type == TokenType.Semicolon)
                        break;
                    condition.Add(next);
                }

                bool result = EvaluateCondition(parent, condition);
                varStr = result ? "True" : "False";
            }
            else if (op.Type == TokenType.Plus || op.Type == TokenType.Minus)
            {
                Token next = tokens[pos++];
                if (next.Type == op.Type)
                {
                    Value v = new Value();
                    string opName = "";
                    string operation = "";

                    if (IsVarName(t))
                        v = GetOrCreate(vars, name.Text);
                    else if (IsLocalVarName(parent, t))
                        v = GetOrCreate(funcs, parent).LocalVars[t.Text];

                    if (next.Type == TokenType.Plus)
                    {
                        opName = "Increment";
                        operation = "++";
                    }
                    else if (next.Type == TokenType.Minus)
                    {
                        opName = "Decrement";
                        operation = "--";
                    }

                    if (v.IsString)
                    {
                        ThrowError("Cannot " + opName + " a string value using '" + operation + "'.", name, numLine);
                        return pos;
                    }

                    long val = v.Number;
                    isConst = v.IsConst;

                    if (op.Type == TokenType.Plus)
                        varNum = ++val;
                    else if (op.Type == TokenType.Minus)
                        varNum = --val;
                }
                else
                {
                    ThrowError("Expected '++' or '--'. Both operators must match.", Combine(new List<Token> { op, next }), numLine);
                    return pos;
                }
            }
            else if (op.Type == TokenType.Plus ||
                     op.Type == TokenType.Minus ||
                     op.Type == TokenType.Multiply ||
                     op.Type == TokenType.Divide ||
                     op.Type == TokenType.Modulo)
            {
                Value v;

                Token equal = tokens[pos++];
                if (equal.Type != TokenType.Assign)
                {
                    ThrowError("Expected '=' after the operator.", equal, numLine);
                    return pos;
                }

                if (IsVarName(t))
                    v = GetOrCreate(vars, name.Text);
                else if (IsLocalVarName(parent, t))
                    v = GetOrCreate(funcs, parent).LocalVars[t.Text];
                else
                {
                    ThrowError("The variable is not defined.", name, numLine);
                    return pos;
                }

                if (v.IsString)
                {
                    ThrowError("Cannot perform arithmetic operations on a string value.", name, numLine);
                    return pos;
                }

                Arithmetic arithmetic = InitializeArithmetic(tokens, pos, parent, Layer, numLine, classOutName, name);
                pos = arithmetic.NewPos;

                long argVal = long.Parse(arithmetic.Val.Text);

                if (op.Type == TokenType.Plus)
                    varNum = v.Number + argVal;
                else if (op.Type == TokenType.Minus)
                    varNum = v.Number - argVal;
                else if (op.Type == TokenType.Multiply)
                    varNum = v.Number * argVal;
                else if (op.Type == TokenType.Divide)
                {
                    if (argVal == 0)
                    {
                        ThrowError("Division by zero is not allowed.", arithmetic.Val, numLine);
                        return pos;
                    }
                    varNum = v.Number / argVal;
                }
                else if (op.Type == TokenType.Modulo)
                {
                    if (argVal == 0)
                    {
                        ThrowError("Modulo by zero is not allowed.", arithmetic.Val, numLine);
                        return pos;
                    }
                    varNum = v.Number % argVal;
                }
            }
            else
            {
                ThrowError("Expected '=' or '++' or '--' after the variable name.", op, numLine);
                return pos;
            }

            if (!IsClassName(parent))
            {
                if ((IsVarName(name) || !IsVarName(name)) && !IsLocalVarName(parent, name))
                {
                    vars[name.Text] = Value.HandleVal(par, lay, isConst, varStr, varNum);
                }
                else if (IsLocalVarName(parent, name))
                {
                    GetOrCreate(funcs, parent).LocalVars[name.Text] = Value.HandleVal(par, lay, isConst, varStr, varNum);
                }
            }
            else if (IsClassName(parent))
            {
                var cls = GetOrCreate(classes, parent);
                cls.Variables[name.Text] = new ClassVariable
                {
                    Value = Value.HandleVal(par, lay, isConst, varStr, varNum),
                    Access = AccessModifier.Private
                };
            }
            else
            {
                ThrowError("This variable couldn't be saved", name, numLine);
                return pos;
            }

            Token semi2 = tokens[pos++];
            if (semi2.Type != TokenType.Semicolon)
            {
                ThrowError("Unexpected Character6", semi2, numLine);
                return pos;
            }

            return pos;
        }

        private int RegisterFunc(
            List<Token> tokens,
            int pos,
            int Layer,
            string parent,
            int numLine,
            bool isConstructor = false)
        {
            Token name;
            if (isConstructor)
                name = new Token(TokenType.Empty, parent);
            else
                name = tokens[pos++];

            if (IsTokenType(name) && !isConstructor)
            {
                ThrowError("The function name is invalid.", name, numLine);
                return pos;
            }
            else if (IsFuncName(name))
            {
                ThrowError("A function with this name already exists.", name, numLine);
                return pos;
            }

            var parameters = new List<Token>();
            var funcBody = new List<Token>();

            Token lParen = tokens[pos++];
            if (lParen.Type != TokenType.LParen)
            {
                ThrowError("Expected '(' after the function name.", lParen, numLine);
                return pos;
            }

            Token p = tokens[pos++];
            while (p.Type != TokenType.LParen &&
                   p.Type != TokenType.RParen &&
                   p.Type != TokenType.LBrace &&
                   p.Type != TokenType.RBrace)
            {
                Token arg = p;
                if (arg.Type != TokenType.Comma)
                {
                    if (IsTokenType(arg))
                    {
                        ThrowError("This variable name is invalid", arg, numLine);
                        return pos;
                    }
                    parameters.Add(arg);
                }
                p = tokens[pos++];
            }
            pos--;

            Token rParen = tokens[pos++];
            if (rParen.Type != TokenType.RParen)
            {
                ThrowError("Expected ')' to close the function parameter list.", rParen, numLine);
                return pos;
            }

            Token lBrace = tokens[pos++];
            if (lBrace.Type != TokenType.LBrace)
            {
                ThrowError("Expected '{' to begin the function body.", lBrace, numLine);
                return pos;
            }

            int braceDepth = 1;
            while (braceDepth > 0)
            {
                Token b = tokens[pos++];

                if (b.Type == TokenType.LBrace)
                {
                    braceDepth++;
                }
                else if (b.Type == TokenType.RBrace)
                {
                    braceDepth--;
                    if (braceDepth == 0)
                        break;
                }

                funcBody.Add(b);
            }

            if (isConstructor)
            {
                var cls = GetOrCreate(classes, parent);
                cls.Functions["constructor"] = new ClassFunction
                {
                    Func = new Func(Layer, parent, name.Text, parameters, funcBody),
                    Access = AccessModifier.Public
                };
            }
            else if (IsClassName(parent))
            {
                var cls = GetOrCreate(classes, parent);
                cls.Functions[name.Text] = new ClassFunction
                {
                    Func = new Func(Layer, parent, name.Text, parameters, funcBody),
                    Access = AccessModifier.Private
                };
            }
            else if (parent == "root" || IsFuncName(new Token(TokenType.Empty, parent)))
            {
                funcs[name.Text] = new Func(Layer, parent, name.Text, parameters, funcBody);
            }

            return pos;
        }

        public List<string> GeistScriptCode = new List<string>();

        public void Execute(string source)
        {
            var lex = new Lexer(source);
            var tokens = new List<Token>();

            while (true)
            {
                Token t = lex.Next();
                tokens.Add(t);
                if (t.Type == TokenType.End)
                    break;
            }

            ExecuteTokens(tokens);
        }

        public void ExecuteTokens(List<Token> tokens, string parent = "root", int layer = 0, int numLine = 0)
        {
            int Layer = layer;
            int pos = 0;

            while (pos < tokens.Count)
            {
                numLine++;
                Token t = tokens[pos++];
                if (t.Type == TokenType.End)
                    break;

                if (parent != "root" && GetOrCreate(funcs, parent).HasReturned)
                    return;

                if (isGeistObjVar(parent, t)) {
                    pos = updateGeistObject(t, Layer, parent, numLine, tokens, pos);
                }
                else if (IsFuncName(t))
                {
                    pos = ExecuteFunction(t, Layer, parent, numLine, tokens, pos);
                }
                else if (IsClassName(GetOrCreate(vars, t.Text).ClassName))
                {
                    Token clsName = GetOrCreate(vars, t.Text).ClassName;
                    Token dot = tokens[pos++];
                    if (dot.Type != TokenType.Dot)
                    {
                        ThrowError("Expected '.' after the class name.", dot, numLine);
                        break;
                    }

                    Token func = tokens[pos];
                    if (IsClassFunction(clsName, func))
                    {
                        pos = ExecuteFunction(clsName, layer + 1, clsName.Text, numLine, tokens, pos);
                    }
                    else
                    {
                        ThrowError("Expected a class function after the class name.", tokens[pos], numLine);
                        break;
                    }
                }
                else if (t.Type == TokenType.Return)
                {
                    Arithmetic arithmetic = InitializeArithmetic(tokens, pos, parent, layer, numLine);

                    GetOrCreate(funcs, parent).ReturnValue = arithmetic.Val;
                    GetOrCreate(funcs, parent).HasReturned = true;

                    return;
                }
                else if (t.Type == TokenType.Print)
                {
                    Token lParen = tokens[pos++];
                    if (lParen.Type != TokenType.LParen)
                    {
                        ThrowError("Expected '(' at the beginning of a print statement.", lParen, numLine);
                        break;
                    }

                    Arithmetic arithmetic = InitializeArithmetic(tokens, pos, parent, Layer, numLine);
                    Token v = arithmetic.Val;
                    pos = arithmetic.NewPos;

                    if (v.Type == TokenType.StringLiteral)
                        Terminal.Write(v.Text);
                    else if (v.Type == TokenType.Number)
                        Terminal.Write(v.Text);
                    else if (v.Type == TokenType.Identifier)
                    {
                        Value var2 = new Value();
                        if (IsVarName(v) && !IsLocalVarName(parent, v))
                        {
                            var2 = GetOrCreate(vars, v.Text);
                        }
                        else if (IsLocalVarName(parent, v))
                        {
                            var2 = GetOrCreate(funcs, parent).LocalVars[v.Text];
                        }
                        else if (IsFuncName(v))
                        {
                            Arithmetic ret = GetReturn(v, Layer, parent, numLine, tokens, pos, "print", true);
                            var2 = TokenToValue(ret.Val);
                            pos = ret.NewPos;
                        }

                        if ((var2.Parent != parent && var2.Layer > Layer) || var2.Layer > Layer)
                        {
                            ThrowError("This variable is not accessible in the current scope.", v, numLine);
                            break;
                        }
                        if (var2.IsString) Terminal.Write(var2.Str);
                        else Terminal.Write(var2.Number.ToString());
                    }
                    pos--;

                    Token rParen = tokens[pos++];
                    if (rParen.Type != TokenType.RParen)
                    {
                        ThrowError("Expected ')' to close the print statement.", rParen, numLine);
                        break;
                    }

                    Token semi = tokens[pos++];
                    if (semi.Type != TokenType.Semicolon)
                    {
                        ThrowError("Expected ';' after the print statement.", semi, numLine);
                        break;
                    }
                    Terminal.Write("\n");
                }
                else if (t.Type == TokenType.If)
                {
                    Token lp = tokens[pos++];
                    if (lp.Type != TokenType.LParen)
                    {
                        ThrowError("Expected '('.", lp, numLine);
                        break;
                    }

                    var condition = new List<Token>();
                    int parenDepth = 1;

                    while (pos < tokens.Count && parenDepth > 0)
                    {
                        Token tok = tokens[pos++];

                        if (tok.Type == TokenType.LParen)
                            parenDepth++;

                        if (tok.Type == TokenType.RParen)
                        {
                            parenDepth--;
                            if (parenDepth == 0)
                                break;
                        }

                        condition.Add(tok);
                    }

                    Token lb = tokens[pos++];
                    if (lb.Type != TokenType.LBrace)
                        ThrowError("Expected '{'.", lb, numLine);

                    var body = new List<Token>();
                    int braceDepth = 1;

                    while (pos < tokens.Count && braceDepth > 0)
                    {
                        Token tok = tokens[pos++];

                        if (tok.Type == TokenType.LBrace)
                            braceDepth++;

                        if (tok.Type == TokenType.RBrace)
                        {
                            braceDepth--;
                            if (braceDepth == 0)
                                break;
                        }

                        body.Add(tok);
                    }

                    bool executed = false;

                    if (EvaluateCondition(parent, condition))
                    {
                        ExecuteTokens(body, parent, Layer + 1, numLine);
                        executed = true;
                    }

                    while (pos < tokens.Count)
                    {
                        Token next = tokens[pos];

                        if (next.Type != TokenType.Else)
                            break;

                        pos++; // consume 'else'

                        if (pos < tokens.Count && tokens[pos].Type == TokenType.If)
                        {
                            pos++; // consume 'if'

                            Token lp2 = tokens[pos++];
                            if (lp2.Type != TokenType.LParen)
                                ThrowError("Expected '('.", lp2, numLine);

                            var cond2 = new List<Token>();
                            int depth2 = 1;

                            while (pos < tokens.Count && depth2 > 0)
                            {
                                Token tok = tokens[pos++];

                                if (tok.Type == TokenType.LParen)
                                    depth2++;

                                if (tok.Type == TokenType.RParen)
                                {
                                    depth2--;
                                    if (depth2 == 0)
                                        break;
                                }

                                cond2.Add(tok);
                            }

                            Token lb2 = tokens[pos++];
                            if (lb2.Type != TokenType.LBrace)
                                ThrowError("Expected '{'.", lb2, numLine);

                            var body2 = new List<Token>();
                            int brace2 = 1;

                            while (pos < tokens.Count && brace2 > 0)
                            {
                                Token tok = tokens[pos++];

                                if (tok.Type == TokenType.LBrace)
                                    brace2++;

                                if (tok.Type == TokenType.RBrace)
                                {
                                    brace2--;
                                    if (brace2 == 0)
                                        break;
                                }

                                body2.Add(tok);
                            }

                            if (!executed && EvaluateCondition(parent, cond2))
                            {
                                ExecuteTokens(body2, parent, Layer + 1, numLine);
                                executed = true;
                            }
                        }
                        else
                        {
                            Token lb3 = tokens[pos++];
                            if (lb3.Type != TokenType.LBrace)
                                ThrowError("Expected '{'.", lb3, numLine);

                            var body3 = new List<Token>();
                            int brace3 = 1;

                            while (pos < tokens.Count && brace3 > 0)
                            {
                                Token tok = tokens[pos++];

                                if (tok.Type == TokenType.LBrace)
                                    brace3++;

                                if (tok.Type == TokenType.RBrace)
                                {
                                    brace3--;
                                    if (brace3 == 0)
                                        break;
                                }

                                body3.Add(tok);
                            }

                            if (!executed)
                            {
                                ExecuteTokens(body3, parent, Layer + 1, numLine);
                                executed = true;
                            }

                            break;
                        }
                    }
                }
                else if (t.Type == TokenType.While)
                {
                    Token lp = tokens[pos++];
                    if (lp.Type != TokenType.LParen)
                        ThrowError("Expected '('.", lp, numLine);

                    int depth = 1;
                    var condExpr = new List<Token>();

                    while (pos < tokens.Count)
                    {
                        Token x = tokens[pos++];

                        if (x.Type == TokenType.LParen)
                            depth++;

                        if (x.Type == TokenType.RParen)
                        {
                            depth--;
                            if (depth == 0)
                                break;
                        }

                        condExpr.Add(x);
                    }

                    Token lb = tokens[pos++];
                    if (lb.Type != TokenType.LBrace)
                        ThrowError("Expected '{'.", lb, numLine);

                    var body = new List<Token>();
                    int braceDepth = 1;

                    while (pos < tokens.Count)
                    {
                        Token x = tokens[pos++];

                        if (x.Type == TokenType.LBrace)
                            braceDepth++;

                        if (x.Type == TokenType.RBrace)
                        {
                            braceDepth--;
                            if (braceDepth == 0)
                                break;
                        }

                        body.Add(x);
                    }

                    while (true)
                    {
                        bool cond = EvaluateCondition(parent, condExpr);

                        if (!cond)
                            break;

                        ExecuteTokens(body, parent, Layer + 1, numLine);
                    }
                }
                else if (t.Type == TokenType.For)
                {
                    Token lp = tokens[pos++];
                    if (lp.Type != TokenType.LParen)
                        ThrowError("Expected '('.", lp, numLine);

                    var init = new List<Token>();
                    var cond = new List<Token>();
                    var update = new List<Token>();
                    int part = 0;

                    while (pos < tokens.Count)
                    {
                        Token x = tokens[pos++];

                        if (x.Type == TokenType.RParen)
                            break;

                        if (x.Type == TokenType.Semicolon)
                        {
                            part++;
                            continue;
                        }

                        if (part == 0) init.Add(x);
                        else if (part == 1) cond.Add(x);
                        else if (part == 2) update.Add(x);
                    }

                    if (pos >= tokens.Count)
                        ThrowError("Unexpected end in for-loop header.", pos < tokens.Count ? tokens[pos] : new Token(TokenType.End, ""), numLine);

                    Token lb = tokens[pos++];
                    if (lb.Type != TokenType.LBrace)
                        ThrowError("Expected '{'.", lb, numLine);

                    var body = new List<Token>();
                    int depth = 1;

                    while (pos < tokens.Count && depth > 0)
                    {
                        Token x = tokens[pos++];

                        if (x.Type == TokenType.LBrace) depth++;
                        if (x.Type == TokenType.RBrace) depth--;

                        if (depth > 0)
                            body.Add(x);
                    }

                    if (depth != 0)
                        ThrowError("Missing closing '}' in for-loop.", pos < tokens.Count ? tokens[pos] : new Token(TokenType.End, ""), numLine);

                    init.Add(new Token(TokenType.Semicolon, ";"));
                    init.Add(new Token(TokenType.End, ""));

                    update.Add(new Token(TokenType.Semicolon, ";"));
                    update.Add(new Token(TokenType.End, ""));

                    body.Add(new Token(TokenType.End, ""));

                    ExecuteTokens(init, parent, Layer + 1, numLine);

                    while (EvaluateCondition(parent, cond))
                    {
                        ExecuteTokens(body, parent, Layer + 1, numLine);
                        ExecuteTokens(update, parent, Layer + 1, numLine);
                    }
                }
                else if (t.Type == TokenType.Function)
                {
                    pos = RegisterFunc(tokens, pos, Layer, parent, numLine);
                }
                else if (t.Type == TokenType.NewClass)
                {
                    Token name = tokens[pos++];
                    if (IsTokenType(name))
                    {
                        ThrowError("The class name is invalid.", name, numLine);
                        break;
                    }
                    else if (classes.ContainsKey(name.Text))
                    {
                        ThrowError("A class with this name already exists.", name, numLine);
                        break;
                    }

                    classes[name.Text] = new Class(name.Text, parent, layer + 1);

                    Token lBrace = tokens[pos++];
                    if (lBrace.Type != TokenType.LBrace)
                    {
                        ThrowError("Expected '{' to begin the class body.", lBrace, numLine);
                        break;
                    }

                    int braceDepth = 1;
                    while (braceDepth > 0)
                    {
                        int tmpPos = pos;
                        tmpPos++;
                        Token b = tokens[pos++];
                        Token innerName = tokens[tmpPos++];
                        _ = innerName; // unused, kept for parity with the original C++ code (also unused there)

                        if (b.Type == TokenType.LBrace)
                        {
                            braceDepth++;
                        }
                        else if (b.Type == TokenType.RBrace)
                        {
                            braceDepth--;
                            if (braceDepth == 0)
                                break;
                        }
                        else if (b.Text == name.Text)
                        {
                            pos = RegisterFunc(tokens, pos, layer + 1, name.Text, numLine, true);
                        }
                        else if (b.Type == TokenType.Function)
                        {
                            pos = RegisterFunc(tokens, pos, layer + 1, name.Text, numLine);
                        }
                        else if (b.Type == TokenType.Let ||
                                 b.Type == TokenType.Const ||
                                 IsVarName(b))
                        {
                            Terminal.Write("CLASS: " + name.Text + "\n");
                            pos = RegisterVar(tokens, pos, layer + 1, name.Text, numLine, b, name);
                        }
                    }
                }
                else if ((t.Type == TokenType.Let ||
                          t.Type == TokenType.Const ||
                          IsVarName(t)) &&
                         !IsFuncName(t) &&
                         t.Type != TokenType.Function)
                {
                    pos = RegisterVar(tokens, pos, Layer, parent, numLine, t);
                }
                else
                {
                    if (!IsTokenType(t))
                        ThrowError("This statement or expression is not recognized.", t, numLine);
                }
            }
        }

        public string GetFileContent()
        {
            return "";
        }

        public void DownloadExample()
        {
            string fileName = "script.gsScript";
            using (var outfile = new StreamWriter(fileName))
            {
                outfile.WriteLine("my text here!");
            }
            Terminal.Write("Example script created: " + fileName + "\n");
        }
    }
}