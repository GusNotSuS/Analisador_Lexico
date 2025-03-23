using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class AnalisadorLexico
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
    ("Operador", @"[+\-*/]"),
    ("Operador_Logico", @"&&|\|\||!"),
    ("Parenteses", @"[()]"),
    ("Chaves", @"[{}]"),
    ("Desconhecido", @"[^\s]+")
};


    static string tokenRegex = string.Join("|", tokenSpecs.ConvertAll(t => $"(?<{t.Item1}>{t.Item2})"));

    static (List<(string, string)>, List<string>) AnalisadorLexicoFunc(string codigo)
    {
        List<(string, string)> tokens = new List<(string, string)>();
        List<string> errors = new List<string>();

        foreach (Match match in Regex.Matches(codigo, tokenRegex))
        {
            foreach (var spec in tokenSpecs)
            {
                if (match.Groups[spec.Item1].Success)
                {
                    string kind = spec.Item1;
                    string value = match.Value;

                    if (kind == "Espacamento")
                        continue;
                    else if (kind == "Comentario")
                        tokens.Add((kind, value));
                    else if (kind == "Desconhecido")
                        errors.Add($"Token desconhecido: {value}");
                    else
                        tokens.Add((kind, value));

                    break;
                }
            }
        }

        return (tokens, errors);
    }

    static void Main()
    {
        while (true)
        {
            Console.Write("Digite um comando QuestLang (ou 'encerrar' para sair): ");
            string entrada = Console.ReadLine();
            if (entrada.ToLower() == "encerrar")
                break;

            var (tokens, errors) = AnalisadorLexicoFunc(entrada);

            if (tokens.Count > 0)
                Console.WriteLine("Tokens reconhecidos: " + string.Join(", ", tokens));
            if (errors.Count > 0)
                Console.WriteLine("Erros encontrados: " + string.Join(", ", errors));
        }
    }
}
