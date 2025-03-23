using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class QuestLangParser
{
    static List<(string, string)> tokenSpecs = new List<(string, string)>
    {
        ("Comentario", @"//.*//"),
        ("Espacamento", @"\s+"),
        ("Comando_de_Movimento", @"move_up|move_down|move_left|move_right"),
        ("Comando_de_Acao", @"jump|attack|defend"),
        ("Estrutura_de_Controle", @"if|else|while|for"),
        ("Numerico", @"\d+"),
        ("Identificador", @"hero|enemy|treasure|trap"),
        ("Operador", @"[+\-]"),
        ("Parenteses", @"[()]"),
        ("Chaves", @"[{}]"),
        ("Ponto_Virgula", @";"),
        ("Desconhecido", @".") 
    };

    static List<(string, string)> Token_Form(string input)
    {
        List<(string, string)> tokens = new List<(string, string)>();
        string regra = string.Join("|", tokenSpecs.ConvertAll(ts => $"(?<{ts.Item1}>{ts.Item2})"));
        foreach (Match match in Regex.Matches(input, regra))
        {
            foreach (var spec in tokenSpecs)
            {
                if (match.Groups[spec.Item1].Success)
                {
                    tokens.Add((spec.Item1, match.Value));
                    break;
                }
            }
        }
        return tokens.FindAll(t => t.Item1 != "Espacamento" && t.Item1 != "Comentario");
    }

    static bool Parse(List<(string, string)> tokens)
    {
        if (tokens.Count == 0)
        {
            Console.WriteLine("Erro: Nenhuma entrada fornecida.");
            return false;
        }

        int index = 0;
        bool result = ParseCheck(tokens, ref index);
        if (index < tokens.Count)
        {
            Console.WriteLine($"Erro sintático próximo a: {tokens[index].Item2}");
            return false;
        }
        return result;
    }

    static bool ParseCheck(List<(string, string)> tokens, ref int index)
    {
        if (index >= tokens.Count) return false;

        if (tokens[index].Item1 == "Comando_de_Movimento" || tokens[index].Item1 == "Comando_de_Acao")
        {
            index++;
            return true;
        }
        else if (tokens[index].Item2 == "if")
        {
            index++;
            return ParseIfCheck(tokens, ref index);
        }
        else if (tokens[index].Item2 == "while")
        {
            index++;
            return ParseWhileCheck(tokens, ref index);
        }
        else if (tokens[index].Item2 == "for")
        {
            index++;
            return ParseForCheck(tokens, ref index);
        }

        return false;
    }

    static bool ParseIfCheck(List<(string, string)> tokens, ref int index)
    {
        if (!ParseControlStructure(tokens, ref index)) return false;

        if (index < tokens.Count && tokens[index].Item2 == "else")
        {
            index++;
            return ParseBlock(tokens, ref index);
        }
        return true;
    }

    static bool ParseWhileCheck(List<(string, string)> tokens, ref int index)
    {
        return ParseControlStructure(tokens, ref index);
    }

    static bool ParseForCheck(List<(string, string)> tokens, ref int index)
    {
        if (index >= tokens.Count || tokens[index].Item2 != "(") return false;
        index++;
        if (!ParseExpression(tokens, ref index)) return false;
        if (index >= tokens.Count || tokens[index].Item2 != ";") return false;
        index++;
        if (!ParseExpression(tokens, ref index)) return false;
        if (index >= tokens.Count || tokens[index].Item2 != ";") return false;
        index++;
        if (!ParseExpression(tokens, ref index)) return false;
        if (index >= tokens.Count || tokens[index].Item2 != ")") return false;
        index++;
        return ParseBlock(tokens, ref index);
    }

    static bool ParseControlStructure(List<(string, string)> tokens, ref int index)
    {
        if (index >= tokens.Count || tokens[index].Item2 != "(") return false;
        index++;
        if (!ParseExpression(tokens, ref index)) return false;
        if (index >= tokens.Count || tokens[index].Item2 != ")") return false;
        index++;
        return ParseBlock(tokens, ref index);
    }

    static bool ParseExpression(List<(string, string)> tokens, ref int index)
    {
        if (index >= tokens.Count) return false;

        if (tokens[index].Item1 == "Identificador" || tokens[index].Item1 == "Numerico")
        {
            index++;
            if (index < tokens.Count && tokens[index].Item1 == "Operador")
            {
                index++;
                return ParseExpression(tokens, ref index);
            }
            return true;
        }
        return false;
    }

    static bool ParseBlock(List<(string, string)> tokens, ref int index)
    {
        if (index >= tokens.Count || tokens[index].Item2 != "{") return false;
        index++;
        while (index < tokens.Count && tokens[index].Item2 != "}")
        {
            if (!ParseCheck(tokens, ref index)) return false;
        }
        if (index >= tokens.Count || tokens[index].Item2 != "}") return false;
        index++;
        return true;
    }

    static void Main()
    {
        while (true)
        {
            Console.Write("Digite um comando QuestLang (ou 'encerrar' para sair): ");
            string entrada = Console.ReadLine();
            if (entrada.ToLower() == "encerrar") break;
            List<(string, string)> tokens = Token_Form(entrada);
            if (Parse(tokens))
                Console.WriteLine("Comando válido!");
            else
                Console.WriteLine("Erro na sintaxe!");
        }
    }
}
